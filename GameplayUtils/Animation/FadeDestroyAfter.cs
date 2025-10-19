using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

namespace AmoaebaUtils
{
public class FadeDestroyAfter : DestroyAfter
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private AnimationCurve fadeTime;
    
    protected override void OnElapsed(float elapsed)
    {
        if(spriteRenderer == null || fadeTime.keys.Length == 0)
        {
            return;
        }
        float ratio = Mathf.Clamp01(elapsed/timeToDestroy);
        float alpha = fadeTime.Evaluate(ratio);
        
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
}
}