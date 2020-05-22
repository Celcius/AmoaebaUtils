using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  AmoaebaUtils
{
public class LinearTransformMovement : TransformMovement
{
    [SerializeField]
    private Vector3 linearMovement;

    protected override void Move()
    {
        transform.position += Vector3.Scale(linearMovement, AxisMultipliers) * Time.deltaTime;
    }
}
}