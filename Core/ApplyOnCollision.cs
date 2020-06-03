using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace AmoaebaUtils
{
public abstract class ApplyOnCollision : MonoBehaviour
{
    public enum CollisionType
    {
        Any,
        Collision,
        Collision2D,
        Trigger,
        Trigger2D,
    }

    [SerializeField]
    private CollisionType collisionType = CollisionType.Any;

    [SerializeField]
    private bool useLayerMask;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private bool useTag;

    [SerializeField]
    private string otherTag;
    
    protected virtual void OnCollisionEnter(Collision col)
    {
        if(ShouldApply(col.gameObject, CollisionType.Collision))
        {
            Apply(col);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if(ShouldApply(col.gameObject, CollisionType.Collision2D))
        {
            Apply(col);
        }
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        if(ShouldApply(col.gameObject, CollisionType.Trigger))
        {
            Apply(col);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if(ShouldApply(col.gameObject, CollisionType.Trigger2D))
        {
            Apply(col);
        }
    }

    protected virtual bool ShouldApply(GameObject otherObj, CollisionType inType)
    {
        return (collisionType == CollisionType.Any || collisionType == inType) 
            && ShouldApply(otherObj.tag, otherObj.layer);
    }

    protected virtual bool ShouldApply(string objTag, int objLayer)
    {
        return (!useLayerMask && !useTag) 
            || (useLayerMask && ((layerMask.value & 1 << objLayer) != 0))
            || (useTag && (otherTag == objTag));
    }

    protected virtual void Apply(Collision col)
    {
        Apply(col.transform);
    }

    protected virtual void Apply(Collision2D col)
    {
        Apply(col.transform);
    }

    protected virtual void Apply(Collider col)
    {
        Apply(col.transform);
    }

    protected virtual void Apply(Collider2D col)
    {
        Apply(col.transform);
    }

    protected abstract void Apply(Transform Transform);
}
}