using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class ResizeBasedOnVectorVar : MonoBehaviour
{
    [SerializeField]
    private Vector2Var sizeVec;

    [SerializeField]
    private BooledVector2 scalePercentage = new BooledVector2(new BoolVector2(false,false), Vector2.one);

    private Vector2 initialSize;

    // Start is called before the first frame update
    private void Awake()
    {
        initialSize = transform.localScale;
        
        OnBoundsChange(Vector2.zero, sizeVec.Value);
         sizeVec.OnChange += OnBoundsChange;
    }

    private void OnDestroy()
    {
        sizeVec.OnChange -= OnBoundsChange;
    }

    private void OnBoundsChange(Vector2 oldVal, Vector2 newVal)
    {
        Func<float, float> xRet = arg => arg * sizeVec.Value.x / initialSize.x;
        Func<float, float> yRet = arg => arg * sizeVec.Value.y / initialSize.y;
        
        Vector2 ratios = new Vector2(scalePercentage.EvaluateX(transform.localScale.x, xRet), 
                                     scalePercentage.EvaluateY(transform.localScale.y, yRet));
        transform.localScale = transform.localScale.z * Vector3.forward + (Vector3)ratios;
    }
}
}