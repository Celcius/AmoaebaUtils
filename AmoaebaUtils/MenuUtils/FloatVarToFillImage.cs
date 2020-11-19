using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AmoaebaUtils
{
public class FloatVarToFillImage : MonoBehaviour
{
    [SerializeField]
    private FloatVar fillVar;

    [SerializeField]
    private Image fillImage;

    private void Start()
    {
        fillVar.OnChange += OnValueChange;
        OnValueChange(0, fillVar.Value);
    }

    private void OnDestroy() 
    {
        fillVar.OnChange -= OnValueChange;
    }

    private void OnValueChange(float oldVal, float newVal)
    {
        fillImage.fillAmount = Mathf.Clamp(newVal, 0, 1.0f);
    }
}
}
