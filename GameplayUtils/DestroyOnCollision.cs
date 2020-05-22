using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class DestroyOnCollision : ApplyOnCollision
{
    // Start is called before the first frame update
    protected override void Apply(Transform transform)
    {
        Destroy(gameObject);
    }
}
}