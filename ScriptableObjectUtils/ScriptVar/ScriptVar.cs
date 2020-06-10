using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptVar<T> : ScriptableObject
{
    public delegate void OnValueChange(T oldValue, T newValue);
    public event OnValueChange OnChange;

    [SerializeField]
    private T setupValue;

    [SerializeField]
    protected T value;

    public T Value
    {
        get { return value; }
        set 
        { 
            InvokeChangeEvent(this.value, value);
            this.value = value; 
        }
    }

    protected void InvokeChangeEvent(T oldVal, T newVal)
    {
        if(OnChange != null) 
        { 
            OnChange(oldVal, newVal); 
        } 
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Reset();
    }
#endif

    public void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
        value = setupValue;
    }

}
