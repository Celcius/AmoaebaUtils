using AmoaebaUtils;
using UnityEngine;

namespace AmoaebaUtils
{

public class ProjectedShadowReuseVar : ShadowProjector 
{
    [SerializeField]
    TransformVar ReuseShadow;

    protected override void Awake()
    {
        if(ReuseShadow == null || ReuseShadow.Value == null)
        {
            
            ShadowParent = Instantiate(ShadowPrefab);
            ShadowRenderer = ShadowParent.GetComponentInChildren<SpriteRenderer>();
            ReuseShadow.Value = ShadowParent;
        }
        else
        {
            ShadowParent = ReuseShadow.Value;
            ShadowRenderer = ReuseShadow.Value.GetComponentInChildren<SpriteRenderer>();
        }   
    }
}   
}