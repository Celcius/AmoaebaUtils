using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AmoaebaUtils
{
public class UpdateTextOnFloatVarChange : UpdateTextOnVarChange<FloatVar, float>
{
    [SerializeField]
    private Color animationNegativeColorTo;

    protected override Color GetAnimationColorTo(float oldval, float newVal)
    {
        return (newVal >= oldval)? base.GetAnimationColorTo(oldval, newVal) : animationNegativeColorTo;
    }
    
}
}
