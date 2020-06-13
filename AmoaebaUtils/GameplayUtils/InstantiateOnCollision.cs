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

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Vector3 euler;

    protected override void Apply(Transform col)
    {
        Vector3 pos = (useOtherPos? col.position : transform.position) + offset;
        GameObject.Instantiate(toInstantiate, pos, Quaternion.Euler(euler));
    }
}
}
