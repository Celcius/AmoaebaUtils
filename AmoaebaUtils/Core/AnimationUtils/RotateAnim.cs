using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class RotateAnim : CurveAnim
{

    [SerializeField]
    private Vector3 speedVec = Vector3.one;

    protected override void OnChange(float evaluatedVal)
    {
        transform.Rotate(evaluatedVal * speedVec, Space.Self);
    }
}
}