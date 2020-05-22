using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class InstantiateOnCollision : ApplyOnCollision
{
    [SerializeField]
    private Transform toInstantiate;

    // Start is called before the first frame update
    protected override void Apply(Transform col)
    {
        GameObject.Instantiate(toInstantiate, transform.position, Quaternion.identity);
    }
}
}
