using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace AmoaebaUtils
{
public abstract class ApplyOnCollision : MonoBehaviour
{

    [SerializeField]
    private bool useLayerMask;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private bool useTag;

    [SerializeField]
    private string tag;
    
    protected virtual void OnCollisionEnter(Collision col)
    {
        if(ShouldApply(col))
        {
            Apply(col);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if(ShouldApply(col))
        {
            Apply(col);
        }
    }

    protected virtual bool ShouldApply(Collision2D col)
    {
        return ShouldApply(col.gameObject.tag, col.gameObject.layer);
    }


    protected virtual bool ShouldApply(Collision col)
    {
        return ShouldApply(col.gameObject.tag, col.gameObject.layer);
    }

    protected virtual bool ShouldApply(string objTag, int objLayer)
    {
        return (!useLayerMask && !useTag) 
            || (useLayerMask && ((layerMask & objLayer) != 0))
            || (useTag && (tag == objTag));
    }

    protected abstract void Apply(Collision col);
    protected abstract void Apply(Collision2D col);
}
}