using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif 

//https://docs.unity3d.com/ScriptReference/PlayerSettings.GetPreloadedAssets.html

namespace AmoaebaUtils 
{
public abstract class BootScriptableObject : ScriptableObject
{
    protected virtual void Awake()
    {
        /*
#if UNITY_EDITOR        
        var preloadedAssets = UnityEditor.PlayerSettings.GetPreloadedAssets().ToList();
        preloadedAssets.Add(this);
        UnityEditor.PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
#endif*/
    }

    protected virtual void OnEnable()
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
            if(!SceneManager.GetActiveScene().isLoaded)
            {
                SceneManager.sceneLoaded -= this.OnSceneLoadedBoot;
                SceneManager.sceneLoaded += this.OnSceneLoadedBoot;
            }
            else
            {
                OnBoot();
            }
        }
    }
    
    private void OnSceneLoadedBoot(Scene newScene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= this.OnSceneLoadedBoot;
        OnBoot();
    }

    protected virtual void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= OnEditorUpdate;
        OnStop();
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
    public abstract void OnBoot();
    public abstract void OnStop();


    public void OnDestroy()
    {
        /*
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
*/
    }
}
}
