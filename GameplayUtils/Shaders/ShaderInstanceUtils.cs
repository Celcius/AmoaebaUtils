using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public static class ShaderInstanceUtils
{
    public const string darknessOuterColor = "_outerColor";
    public const string darknessInnerColor = "_innerColor";
    public const string darkessRadius = "_innerRadius";
    public const string darknessBorder = "_border";
    public const string darknessXOffset = "_xOffset";
    public const string darknessYOffset = "_yOffset";
    public const string darknessOpacity = "_opacity";


    public static Color GetDarknessInnerColor(Material material)
    {
        const string colorStr = darknessInnerColor;
        return GetColor(material, colorStr);
    }

    public static void SetDarknessInnerColor(Color color, Material material)
    {
        const string colorStr = darknessInnerColor;
        material.SetColor(colorStr, color);
    }

    public static Color GetDarknessOuterColor(Material material)
    {
        const string colorStr = darknessOuterColor;
        return GetColor(material, colorStr);
    }

    public static void GetDarknessOuterColor(Color color, Material material)
    {
        const string colorStr = darknessOuterColor;
        material.SetColor(colorStr, color);
    }

    public static void SetDarknessMaterialPosition(float xPos, float yPos, Material material)
    {   
        const string posXStr = darknessXOffset;
        const string posYStr = darknessYOffset;
        
        material.SetFloat(posXStr, xPos);
        material.SetFloat(posYStr, yPos);
    }

    public static float GetDarknessPosX(Material material)
    {
        return GetFloat(material, darknessXOffset);
    }

    public static float GetDarknessPosY(Material material)
    {
        return GetFloat(material, darknessYOffset);
    }

    public static float GetDarknessRadius(Material material)
    {
        return GetFloat(material, darkessRadius);
    }

    public static float GetDarknessBorder(Material material)
    {
        return GetFloat(material, darknessBorder);
    }

    public static float GetDarknessOpacity(Material material)
    {
        return GetFloat(material, darknessOpacity);
    }

    public static void SetDarknessPosX(Material material, float val)
    {
        SetFloat(material, darknessXOffset, val);
    }

    public static void SetDarknessPosY(Material material, float val)
    {
        SetFloat(material, darknessYOffset, val);
    }

    public static void SetDarknessRadius(Material material, float val)
    {
        SetFloat(material, darkessRadius, val);
    }

    public static void SetDarknessBorder(Material material, float val)
    {
        SetFloat(material, darknessBorder, val);
    }

    public static void SetDarknessOpacity(Material material, float val)
    {
        SetFloat(material, darknessOpacity, val);
    }


    private static Color GetColor(Material material, string param)
    {
        return material.GetColor(param);
    }

    private static void SetColor(Material material, string param, Color color)
    {
        material.SetColor(param,color);
    }

    private static float GetFloat(Material material, string param)
    {
        return material.GetFloat(param);
    }

    private static void SetFloat(Material material, string param, float val)
    {
        material.SetFloat(param, val);
    }
        
}
}