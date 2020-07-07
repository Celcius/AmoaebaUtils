using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ActionCommandObject
{
    private const float DEFAULT_WEIGHT = 0;

    protected abstract void OnAction();
    protected virtual void OnActionInterrupt() {}
    public virtual float GetWeight() { return DEFAULT_WEIGHT; }
    public virtual bool CanPerformAction() { return true; }

       public async void PerformAction()
   {
       if(!CanPerformAction())
       {
           OnActionInterrupt();
           return;
       }
       OnAction();
   }

}
