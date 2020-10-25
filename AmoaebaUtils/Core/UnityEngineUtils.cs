using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR     
using UnityEditor;
#endif

namespace AmoaebaUtils
{
public static class UnityEngineUtils
{
    public static float AnimationCurveDuration(AnimationCurve curve)
    {
        return (curve.keys.Length == 0)? 0.0f : curve.keys[curve.keys.Length-1].time;
    }

    public static Vector2 WorldOrthographicSize(Camera cam, bool considerOrientation = true)
    {
        Vector2 bounds = new Vector2(2*cam.orthographicSize, 2*cam.orthographicSize*cam.aspect);
        bounds = (Input.deviceOrientation == DeviceOrientation.Portrait || 
                 Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) ?
                 new Vector2(Mathf.Min(bounds.x, bounds.y), Mathf.Max(bounds.x, bounds.y)) : 
                 new Vector2(Mathf.Max(bounds.x, bounds.y), Mathf.Min(bounds.x, bounds.y));
        return bounds;
    }

    public static bool IsInPlayModeOrAboutToPlay()
    {
#if UNITY_EDITOR        
        return (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode);
#else
        return Application.isPlaying ;        
#endif        
    }

    public static bool WillChangeToPlayMode()
    {
#if UNITY_EDITOR        
        return EditorApplication.isPlayingOrWillChangePlaymode;
#else
        return false;        
#endif        
    }

    public static Color InvertColor(Color rgbColor) 
    {
        float h, s, v;
        Color.RGBToHSV(rgbColor, out h, out s, out v);
        return Color.HSVToRGB((h + 0.5f) % 1, s, v);
    }

    public static Color NegativeColor(Color color)
    {
        return new Color(1.0f-color.r, 1.0f-color.g, 1.0f-color.b);
    }
}
}