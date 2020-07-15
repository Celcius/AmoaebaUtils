using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AmoaebaUtils
{
[RequireComponent(typeof(Image))]
public class FadeInFadeOutAnim : CurveAnim
{
    private Image image;

    protected override void Start()
    {
        base.Start();
        image = GetComponent<Image>();
    }

    protected override void OnChange(float evaluatedVal)
    {
        Color color = image.color;
        color.a  = evaluatedVal;
        image.color = color;
    }
}
}