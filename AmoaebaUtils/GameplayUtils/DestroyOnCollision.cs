using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class DestroyOnCollision : ApplyOnCollision
{
    public enum DestructionTarget { Self, Other }
    [SerializeField]
    private DestructionTarget destructionType = DestructionTarget.Self;

    // Start is called before the first frame update
    protected override void Apply(Transform transform)
    {
        Destroy(destructionType == DestructionTarget.Self? gameObject : transform.gameObject);
    }
}
}