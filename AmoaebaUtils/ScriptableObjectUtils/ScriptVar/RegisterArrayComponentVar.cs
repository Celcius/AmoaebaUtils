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
    public enum RegisterType
    {
        AwakeOnly,
        AwakeDestroy,
        EnableDisable,
        AwakeEnableDisable
    }
    
    [SerializeField]
    private V ArrayVar;

    [SerializeField]
    private RegisterType registerType = RegisterType.AwakeOnly;

    private T toRegister;
    bool registered = false;

    private void Awake()
    {
        registered = false;

        if(registerType == RegisterType.AwakeOnly 
           || registerType == RegisterType.AwakeDestroy
           || registerType == RegisterType.AwakeEnableDisable)
        {
            Register(registerType == RegisterType.AwakeOnly);
        }
    }

    private void OnEnable() 
    {
        if(registerType == RegisterType.AwakeEnableDisable || registerType == RegisterType.EnableDisable)
        {
            Register(false);
        }
    }

    private void OnDisable()
    {
        if(registerType == RegisterType.AwakeEnableDisable || registerType == RegisterType.EnableDisable)
        {
            Unregister();
        }
    }

    private void OnDestroy()
    {
        if(registerType == RegisterType.AwakeDestroy)
        {
            Unregister();
        }
    }

    public void Register(bool destroyOnRegister = false)
    {
        if(registered)
        {
            return;
        }

        toRegister = GetComponent<T>();
        Assert.IsNotNull(toRegister, "Component not present in RegisterVar " + this.name);
        Assert.IsNotNull(ArrayVar, "ArrayVar not present in RegisterVar " + this.name);
        ArrayVar.Add(toRegister);

        registered = true;

        if(UnityEngineUtils.IsInPlayModeOrAboutToPlay())
        {
            if(destroyOnRegister)
            {
                Destroy(this);
            }
        }
        
    }

    public void Unregister()
    {
        if(!registered || toRegister == null)
        {
            return;
        }

        ArrayVar.Remove(toRegister);
        registered = false;

    }
}
}