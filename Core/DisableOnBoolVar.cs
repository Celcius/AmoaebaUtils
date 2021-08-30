using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class DisableOnBoolVar : MonoBehaviour
{
    [SerializeField]
    private BoolVar boolVar;

    [SerializeField]
    private bool valueToDisable = false;

    [SerializeField]
    private bool OnlyCheckStart = true;

    private void Start()
    {
        if(!OnlyCheckStart)
        {
            boolVar.OnChange += OnValueChange;
        }
        
        OnValueChange(false, boolVar.Value);
    }

    private void OnDestroy() 
    {
        boolVar.OnChange -= OnValueChange;
    }

    private void OnValueChange(bool oldVal, bool newVal)
    {
        gameObject.SetActive(!(newVal == valueToDisable));
    }
}
}