using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace AmoaebaUtils
{
public class RegisterComponentVar<T,V> : MonoBehaviour 
    where T : Component
    where V : ScriptVar<T>
{
    [SerializeField]
    private V ScriptVar;
    private void Awake()
    {
        T toRegister = GetComponent<T>();
        Assert.IsNotNull(toRegister, "Component not present in RegisterVar " + this.name);
        Assert.IsNotNull(ScriptVar, "ScriptVar not present in RegisterVar " + this.name);
        ScriptVar.Value = toRegister;
        Destroy(this);
    }
}
}