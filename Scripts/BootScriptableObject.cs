using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif 

using System.Linq;
using System;

//https://docs.unity3d.com/ScriptReference/PlayerSettings.GetPreloadedAssets.html

namespace AmoaebaUtils 
{
public class BootScriptableObject : ScriptableObject
{
    protected void Awake()
    {
#if UNITY_EDITOR        
        var preloadedAssets = UnityEditor.PlayerSettings.GetPreloadedAssets().ToList();
        preloadedAssets.Add(this);
        UnityEditor.PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
#endif
    }

    protected void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= OnEditorUpdate;
        if(!Application.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.update += OnEditorUpdate;
        }
        else
#endif
        if(Application.isPlaying)
        {
            OnBoot();
        }
    }

    protected void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= OnEditorUpdate;
#endif
    }

     public void OnEditorUpdate()
     {
        if(Application.isPlaying)
        {
            OnBoot();
#if UNITY_EDITOR
    EditorApplication.update -= OnEditorUpdate;
#endif
        }
     }
    public virtual void OnBoot(){}

    public void OnDestroy()
    {
#if UNITY_EDITOR        
        UnityEngine.Object[] preloadedAssets = UnityEditor.PlayerSettings.GetPreloadedAssets();
        List<UnityEngine.Object> newPreloadedAssets = new List<UnityEngine.Object>();
        foreach(UnityEngine.Object ob in preloadedAssets)
        {
            if(ob != this && ob != null)
            {
                newPreloadedAssets.Add(ob);
            }
        }
        UnityEditor.PlayerSettings.SetPreloadedAssets(newPreloadedAssets.ToArray());
#endif
    }
}
}
