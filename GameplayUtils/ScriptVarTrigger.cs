using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class ScriptVarTrigger<T,V> : MonoBehaviour where V : ScriptVar<T>
{
    [SerializeField]
    protected V var;

    [SerializeField]
    protected T valueOnCollision;

    [SerializeField]
    protected bool shootOnce = true;
    protected bool hasShot;

    private void OnTriggerEnter(Collider other) 
    {
        if(!CheckValidity(other, true))
        {
            return;
        }
        OnTrigger();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(!CheckValidity(other, true))
        {
            return;
        }
        OnTrigger();
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(!CheckValidity(other, false))
        {
            return;
        }
        OnTriggerExit();
    }

    private void OnTriggerExit(Collider other) 
    {
        if(!CheckValidity(other, false))
        {
            return;
        }
        OnTriggerExit();
    }

    protected virtual bool CheckValidity(Collider other, bool isEnter)
    {
        return true;
    }

    protected virtual bool CheckValidity(Collider2D other, bool isEnter)
    {
        return true;
    }

    public virtual void OnTrigger()
    {
        if(hasShot && shootOnce)
        {
            return;
        }
        var.Value = valueOnCollision;
        hasShot = true;
    }

    public virtual void OnTriggerExit()
    {     

    }
}
}