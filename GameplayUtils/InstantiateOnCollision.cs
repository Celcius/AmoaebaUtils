using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class InstantiateOnCollision : ApplyOnCollision
{
    [SerializeField]
    private Transform toInstantiate;

    [SerializeField]
    private bool useOtherPos = false;

    protected override void Apply(Transform col)
    {
        GameObject.Instantiate(toInstantiate, useOtherPos? col.position : transform.position, Quaternion.identity);
    }
}
}
