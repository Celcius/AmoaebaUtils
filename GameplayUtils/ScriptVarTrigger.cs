using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class ScriptVarTrigger<T,V> : MonoBehaviour where V : ScriptVar<T>
{
    [SerializeField]
    private V var;

    [SerializeField]
    private T valueOnCollision;

    [SerializeField]
    private bool shootOnce = true;
    private bool hasShot;

    private void OnTriggerEnter(Collider other) 
    {
        OnTrigger();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        OnTrigger();
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
}
}