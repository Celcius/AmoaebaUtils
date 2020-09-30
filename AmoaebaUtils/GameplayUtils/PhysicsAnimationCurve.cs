using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
[Serializable]
public class PhysicsAnimationCurve
{
    public enum PhysicsType
    {
        None,
        Position,
        PositionOffset,
        Velocity,
    }

    public float Duration
    {
        get {
            return UnityEngineUtils.AnimationCurveDuration(Curve)*TimeMultiplier;
        }
    }

    public AnimationCurve Curve = new AnimationCurve();
    public PhysicsType CurveType = PhysicsType.None;

    public float Offset = 0.0f;
    public float TimeOffset = 0.0f;
    public float AxisMultiplier = 1.0f;
    public float TimeMultiplier = 1.0f;

    public float Evaluate(float prevValue, float elapsedTime, float deltaTime)
    {
        if(CurveType == PhysicsType.None)
        {
            return prevValue;
        }

        elapsedTime = (elapsedTime + TimeOffset) / TimeMultiplier;
        float evaluatedValue = Curve.Evaluate(elapsedTime) * AxisMultiplier + Offset;

        switch (CurveType)
        {
            case  PhysicsType.None:
                return prevValue;
            case  PhysicsType.Position:
                return evaluatedValue;
            case  PhysicsType.PositionOffset:
                return prevValue + evaluatedValue*AxisMultiplier;
            case  PhysicsType.Velocity:
                return prevValue + evaluatedValue * deltaTime*AxisMultiplier;
        }

        Debug.LogError("Fell through unexpected switch case at animation curve");
        return prevValue;
    }
}
}
