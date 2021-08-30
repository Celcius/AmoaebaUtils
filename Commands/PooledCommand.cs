using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public abstract class PooledCommand : Command
{
    private CommandPool pool;
    

    public PooledCommand() {}
    public PooledCommand(CommandPool pool)
    {
        this.pool = pool;
    }

    protected void OnFinish()
    {
        if(pool != null)
        {
            pool.ReturnToPool(this);
        }
    }

    public void SetCommandPool(CommandPool pool)
    {
        this.pool = pool;
    }

    public void RemoveCommandPool()
    {
        this.pool = null;
    }

    public abstract bool Execute(Action callback = null);
    public virtual bool Execute(Action callback = null, bool returnToPoolOnFinish = true)
    {
        return Execute(() => 
        {
            callback();
            if(returnToPoolOnFinish)
            {
                OnFinish();
            }
        });
    }
    
    public abstract bool Undo(Action callback = null);
    public virtual bool Undo(Action callback = null, bool returnToPoolOnFinish = true)
    {
        return Undo(() => 
        {
            callback();
            if(returnToPoolOnFinish)
            {
                OnFinish();
            }
        });
    }
}
}