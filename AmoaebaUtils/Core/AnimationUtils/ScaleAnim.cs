using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class ScaleAnim : CurveAnim
{
    [SerializeField]
    private Vector3 speedVec;
    private Vector3 initScale;

    
    protected override void Start()
    {
        base.Start();
        initScale = transform.localScale;
    }

    protected override void OnChange(float evaluatedVal)
    {
        Vector3 newScale = initScale;
        newScale.Scale(speedVec * evaluatedVal);
        transform.localScale = newScale;
    }
}
}