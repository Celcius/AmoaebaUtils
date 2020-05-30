using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public static class UnityEngineUtils
{
    public static float AnimationCurveDuration(AnimationCurve curve)
    {
        return (curve.keys.Length == 0)? 0.0f : curve.keys[curve.keys.Length-1].time;
    }
}
}