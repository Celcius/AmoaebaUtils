using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AmoaebaUtils
{
    
public class AnimateFramesRandomly : AnimateFrames
{
    protected override int NextIndex()
    {
       return UnityEngine.Random.Range(0,frames.Length-1);
    }
}
}
