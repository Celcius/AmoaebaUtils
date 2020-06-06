using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace AmoaebaUtils
{
public abstract class ApplyToVarOnCollision<T,V> : ApplyOnCollision
    where V : ScriptVar<T>
{
    [SerializeField]
    protected V var;

    protected virtual void Awake()
    {
        Assert.IsNotNull(var, "Collision Var not assigned to " + this);
    }
}
}