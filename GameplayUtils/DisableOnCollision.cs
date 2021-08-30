using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class DisableOnCollision : DestroyOnCollision
{
    protected override void Apply(Transform transform)
    {
        GameObject toDisable = (destructionType == DestructionTarget.Self? gameObject : transform.gameObject);
        toDisable.SetActive(false);
    } 
}
}