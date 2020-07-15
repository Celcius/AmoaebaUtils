using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace AmoaebaUtils
{
public class RegisterArrayComponentVar<T,V> : MonoBehaviour 
    where T : Component
    where V : ArrayVar<T>
{
    [SerializeField]
    private V ArrayVar;
    private void Awake()
    {
        T toRegister = GetComponent<T>();
        Assert.IsNotNull(toRegister, "Component not present in RegisterVar " + this.name);
        Assert.IsNotNull(ArrayVar, "ArrayVar not present in RegisterVar " + this.name);
        ArrayVar.Add(toRegister);
        Destroy(this);
    }
}
}