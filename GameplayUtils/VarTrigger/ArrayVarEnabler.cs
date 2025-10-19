using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class ArrayVarEnabler<T,V> : MonoBehaviour where V : ArrayVar<T>
{
    [SerializeField]
    private V arrayVar;

    [SerializeField]
    private bool disableOnEmpty = true;

    private void Start()
    {
        arrayVar.OnChange += OnValueChange;
        OnValueChange(null, arrayVar.Value);
    }

    private void OnDestroy() 
    {
        arrayVar.OnChange -= OnValueChange;
    }

    private void OnValueChange(T[] oldVal, T[] newVal)
    {
        bool isEnabled = disableOnEmpty? 
                        (arrayVar.Count() > 0) : 
                        (arrayVar.Count() <= 0);
        gameObject.SetActive(isEnabled);
    }
}
