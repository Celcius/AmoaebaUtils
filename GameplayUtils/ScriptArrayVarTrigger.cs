using System;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{

public class ScriptArrayVarTrigger<T, V> : ScriptVarTrigger<T[], V> where V : ArrayVar<T>
{
    [SerializeField]
    protected bool loadComponentOnStart = true;
    [SerializeField]
    protected bool removeOnExit = true;
    protected T component;

    protected virtual void Start()
    {
        if(loadComponentOnStart)
        {
            component = GetComponent<T>();
        }
    }
    public override void OnTrigger()
    {
        if(component != null)
        {
            var.Add(component);
        }
    }

    public override void OnTriggerExit()
    {     
        if(component != null && removeOnExit)
        {
            var.Remove(component);
        }
    }
}
}