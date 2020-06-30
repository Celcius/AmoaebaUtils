using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockable : MonoBehaviour
{
    public int LockedIndex;

    public bool IsLockedOn => LockableSystem.Instance.IsLocked(this);

    void Awake()
    {
        LockableSystem.Instance.Add(this);
    }

    void OnDestroy()
    {
        LockableSystem.Instance.Remove(this);
    }

    public virtual bool CanLock() { return true; }

    
    public virtual void OnLock() { Debug.Log("Locked On " + this.name); }

    public virtual void OnUnlock() { Debug.Log("Unlocked On " + this.name);  }
}
