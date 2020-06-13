using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  AmoaebaUtils
{
public class LinearTransformMovement : TransformMovement
{
    [SerializeField]
    private Vector3 linearMovement;

    [SerializeField]
    private bool absoluteMovement = true;

    protected override void Move()
    {
        Vector3 moveDir = Vector3.Scale(linearMovement, AxisMultipliers);
        moveDir =  absoluteMovement? moveDir :  transform.rotation *moveDir;
        transform.position +=  moveDir * Time.deltaTime;
    }
}
}