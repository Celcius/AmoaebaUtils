using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class LockableSystem : SingletonScriptableObject<LockableSystem>
{
    [SerializeField]
    private LockableArrayVar lockables;

    [SerializeField]
    private LockableVar currentlyLocked;

    [HideInInspector]
    public bool LockNextOnRemove = true;
    
    private void OnEnable()
    {
        Clear();
    }

    public bool IsLocked(Lockable lockable)
    {
        return GetLocked() == lockable;
    }

    public bool HasLockOn()
    {
        return GetLocked() != null;
    }

    public Lockable GetLocked()
    {
        return currentlyLocked.Value;
    }

    public void UnlockOn()
    {
        if(currentlyLocked.Value != null)
        {
            currentlyLocked.Value.OnUnlock();
        }
        currentlyLocked.Value = null;
    }

    public void CycleLock(Vector3 lockAnchor)
    {
        if(HasLockOn())
        {
            UnlockOn();
        }
        else
        {
            LockOnNext(lockAnchor);
        }
    }

    public void Remove(Lockable lockable)
    {
        if(GetLocked() == lockable)
        {
            if(LockNextOnRemove)
            {
                LockOnNext(lockable.transform.position);
            } else {
                UnlockOn();
            }
        }
        lockables.Remove(lockable);
    }

    public void Add(Lockable lockable)
    {
        lockables.Add(lockable);
    }
    
    public void Clear()
    {
        UnlockOn();
        lockables.Clear();
    }

    public void LockOnNext(Vector3 lockAnchor)
    {
        LockOnByOffset(1, lockAnchor);
    }

    public void LockOnPrevious(Vector3 lockAnchor)
    {
        LockOnByOffset(-1, lockAnchor);
    }

    private void LockOnByOffset(int offset, Vector3 lockAnchor)
    {
        if(!HasLockOn())
        {
            lockables.SortByDistance(lockAnchor);
        }
        
        Lockable newLock = null;

        for(int nextOffset = offset, iterations = 0; 
            iterations < lockables.Value.Length; 
            nextOffset += offset, iterations++)
        {
            newLock = lockables.GetNextByOffset(GetLocked(), nextOffset);
            if(newLock != GetLocked() && newLock != null && newLock.CanLock())
            {
                break; // not found
            }
        }
        
        UnlockOn();

        if(newLock != null)
        {
            currentlyLocked.Value = newLock;
            newLock.OnLock();
        }
        else
        {
            Debug.Log("Nothing to lockOn");
        }
    }
}
