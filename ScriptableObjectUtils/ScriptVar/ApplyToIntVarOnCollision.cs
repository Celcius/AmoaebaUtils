using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class ApplyToIntVarOnCollision : ApplyToVarOnCollision<int, IntVar>
{
    [SerializeField]
    private int inc;

    protected override void Apply(Transform col)
    {
        var.Value += inc;
    }
}
}