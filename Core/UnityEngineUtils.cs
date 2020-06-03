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

    public static Vector2 WorldOrthographicSize(Camera cam)
    {
        return new Vector2(2*cam.orthographicSize, 2*cam.orthographicSize*cam.aspect);
    }
}
}