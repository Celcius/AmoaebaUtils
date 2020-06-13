using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class ApplyToFloatVarOnCollision : ApplyToVarOnCollision<float, FloatVar>
{
    [SerializeField]
    private float inc;

    protected override void Apply(Transform col)
    {
        var.Value += inc;
    }
}
}