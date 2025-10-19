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

    public enum WrapMode 
    {
        Repeat, 
        Clamp,
    }

    public AnimationCurve Curve = new AnimationCurve();
    public PhysicsType CurveType = PhysicsType.None;

    public WrapMode wrapMode = WrapMode.Repeat;
    public float Offset = 0.0f;
    public float TimeOffset = 0.0f;
    public float AxisMultiplier = 1.0f;
    public float TimeMultiplier = 1.0f;

    public float Evaluate(float prevValue, float elapsedTime, float deltaTime = 0)
    {
        if(CurveType == PhysicsType.None)
        {
            return prevValue;
        }

        elapsedTime = (elapsedTime + TimeOffset) / TimeMultiplier;
        switch(wrapMode)
        {
            case WrapMode.Clamp:
                elapsedTime = Mathf.Clamp01(elapsedTime);
                break;
            case WrapMode.Repeat:
                break;
        }
        float evaluatedValue = Curve.Evaluate(elapsedTime);

        switch (CurveType)
        {
            case  PhysicsType.None:
                return prevValue;
            case  PhysicsType.Position:
                return evaluatedValue * AxisMultiplier + Offset;
            case  PhysicsType.PositionOffset:
                return prevValue + evaluatedValue * AxisMultiplier + Offset;
            case  PhysicsType.Velocity:
                return prevValue + evaluatedValue * deltaTime * AxisMultiplier + Offset;
        }

        Debug.LogError("Fell through unexpected switch case at animation curve");
        return prevValue;
    }
}
}
