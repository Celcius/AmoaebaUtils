using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  AmoaebaUtils
{
[ExecuteInEditMode]
public abstract class ArrayVarIndexObserver<T, V> : MonoBehaviour where T : ArrayVar<V>
{
    [SerializeField]
    private T varToObserve;

    [SerializeField]
    private int indexToObserve;

    protected virtual void Start()
    {
        SetVar(varToObserve);
    }

    public void SetVar(T arrayVar)
    {
        if(arrayVar == null)
        {
            this.varToObserve = null;
            return;
        }

        V[] prevArray = varToObserve == null? null : varToObserve.Value;
        this.varToObserve = arrayVar;

        varToObserve.OnChange += OnChangeCallback;
        OnChangeCallback(prevArray, arrayVar.Value);
    }

    private void OnChangeCallback(V[] oldValue, V[] newValue)
    {
        if(newValue == null || newValue.Length <= indexToObserve)
        {
            OnChange(default(V));
        }

        OnChange(newValue[indexToObserve]);
    }

    public abstract void OnChange(V var);
}
}
