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

    public virtual T Value
    {
        get { return value; }
        set 
        { 
            T oldVal = this.value;
            this.value = value; 
            InvokeChangeEvent(oldVal, value);
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
        if(!Application.isPlaying)
        {
            Reset();            
        }
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
