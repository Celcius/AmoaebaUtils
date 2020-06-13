using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class ApplyForceToBody2DOnCollision : ApplyToVarOnCollision<Rigidbody2D, Rigidbody2DVar>
{
    public enum DirectionType
    {
        Direct,
        MagnitudeToDirection,
        MagnitudeToInverseDirection,
        MagnitudeInverseHorizontal,
        MagnitudeInverseVertical,
        MagnitudePositive,
        MagnitudePositiveHorizontal,
        MagnitudePositiveVertical,
        MagnitudeNegative,
        MagnitudeNegativeHorizontal,
        MagnitudeNegativeVertical,
    }
    
    [SerializeField]
    private Vector2 force;

    [SerializeField]
    private ForceMode2D mode;

    [SerializeField]
    private DirectionType directionType;

    [SerializeField]
    private bool isWorldRelative = false;
    
    protected override void Apply(Transform otherTransform)
    {
        if(var.Value == null)
        {
            return;
        }

        Vector2 forceToApply = (transform.position - otherTransform.position).normalized * force.magnitude;
        switch (directionType)
        {
            case DirectionType.Direct:
                forceToApply = force;
                break;

            case DirectionType.MagnitudeToDirection:
                break;

            case DirectionType.MagnitudeToInverseDirection:
                forceToApply = (otherTransform.position - transform.position).normalized * force.magnitude;
                break;

            case DirectionType.MagnitudeInverseHorizontal:
                forceToApply.x = -forceToApply.x;
                break;

            case DirectionType.MagnitudeInverseVertical:
                forceToApply.y = -forceToApply.y;
                break;
                    
            case DirectionType.MagnitudePositive:
                forceToApply.x = Mathf.Abs(forceToApply.y);
                forceToApply.y = Mathf.Abs(forceToApply.y);
                break;

            case DirectionType.MagnitudePositiveHorizontal:
                forceToApply.x = Mathf.Abs(forceToApply.y);
                break;

            case DirectionType.MagnitudePositiveVertical:
                forceToApply.y = Mathf.Abs(forceToApply.y);
                break;

            case DirectionType.MagnitudeNegative:
                forceToApply.x = -Mathf.Abs(forceToApply.y);
                forceToApply.y = -Mathf.Abs(forceToApply.y);
                break;

            case DirectionType.MagnitudeNegativeHorizontal:
                forceToApply.x = -Mathf.Abs(forceToApply.y);
                break;

            case DirectionType.MagnitudeNegativeVertical:
                forceToApply.y = -Mathf.Abs(forceToApply.y);
                break;
        }

        if(isWorldRelative)
        {
            forceToApply = (Vector2)transform.TransformPoint((Vector2)forceToApply);
        }
        var.Value.AddForce(forceToApply, mode);
    }
}
}