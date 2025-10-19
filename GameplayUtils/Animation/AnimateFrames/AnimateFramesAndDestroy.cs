using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AmoaebaUtils
{
public class AnimateFramesAndDestroy : AnimateFrames
{
    protected override void FinishedAnimating()
    {
#if UNITY_EDITOR        
        if(Application.isPlaying)
        {
#endif
            Destroy(this.gameObject);
#if UNITY_EDITOR       
        }
#endif
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(AnimateFramesAndDestroy))]
public class AnimateFramesAndDestroyEditor : AnimateFramesEditor
{
}
#endif
}