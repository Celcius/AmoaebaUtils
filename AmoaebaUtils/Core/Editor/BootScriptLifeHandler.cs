using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace AmoaebaUtils
{
[InitializeOnLoad]
 public class BootScriptLifeHandler : UnityEditor.AssetModificationProcessor {
     //
 
    public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions rao)
     {
 
         BootScriptableObject ob = AssetDatabase.LoadAssetAtPath<BootScriptableObject>(assetPath);
         if(ob != null)
         {
             ob.OnDestroy();
         }
         
         return AssetDeleteResult.DidNotDelete;
     }
 
 }
}