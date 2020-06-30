using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class LockableArrayVar : ArrayVar<Lockable>
{
    public Lockable GetNextByOffset(Lockable fromLocked, int offset)
    {
        if(value.Length == 0)
        {
            return null;
        }

        int index = fromLocked != null? fromLocked.LockedIndex : INVALID_INDEX; 
        index = index == INVALID_INDEX? 0 : index + offset;
        index = MathUtils.NegMod(index, value.Length);

        return value[index];
    }

    public override void Sort(IComparer comparer)
    {
        base.Sort(comparer);
        UpdateIndexes();
    }

    public void SortByDistance(Vector3 anchorPos)
    {
        this.Sort(new LockableArrayComparer(anchorPos));
    }

    public override void Add(Lockable component)
    {
        base.Add(component);
        UpdateIndexes();
    }

    public override bool Remove(Lockable component)
    {
        component.LockedIndex = INVALID_INDEX;
        bool ret = base.Remove(component);

        UpdateIndexes();

        return ret;
    }

    private void UpdateIndexes()
    {
        for(int i = 0; i < value.Length; i++)
        {
            value[i].LockedIndex = i;
        }
    }

    
    public override void Clear()
    {
        foreach(Lockable component in value)
        {
            component.LockedIndex = INVALID_INDEX;
        }
        base.Clear();
    }

    private class LockableArrayComparer: IComparer
    {
        private Vector3 anchorPos;
        
        public LockableArrayComparer(Vector3 anchorPos)
        {
            this.anchorPos = anchorPos;
        }

        int IComparer.Compare(object a, object b)
        {
            Lockable lockable1 = (Lockable)a;
            Lockable lockable2 = (Lockable)b;

            if(lockable1 == null && lockable2 == null)
            {
                return 0;
            } 
            
            if(lockable1 == null)
            {
                return -1;
            }

            if(lockable2 == null)
            {
                return 1;
            }

            float dist1 = Vector3.Distance(lockable1.transform.position, anchorPos);
            float dist2 = Vector3.Distance(lockable2.transform.position, anchorPos);
            return (int)Mathf.Sign(dist1 - dist2);
      }
    }
}
