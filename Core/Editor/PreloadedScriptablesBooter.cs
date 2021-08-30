using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// https://medium.com/@fiftytwo/fast-singleton-approach-in-unity-fdba0b5309d5

namespace AmoaebaUtils {

#if UNITY_EDITOR
 [InitializeOnLoad]
 #endif
public class PreloadedScriptablesBooter
{
    static PreloadedScriptablesBooter()
    {
#if UNITY_2018_2_OR_NEWER
            PlayerSettings.GetPreloadedAssets();
#else
            var findProperty = typeof(PlayerSettings).GetMethod("FindProperty",
                BindingFlags.NonPublic | BindingFlags.Static);
            var preloadedAssetsProp = findProperty.Invoke(null,
                new object[] { "preloadedAssets" }) as SerializedProperty;
            var assets = new UnityEngine.Object[preloadedAssetsProp.arraySize];
            for (int i = assets.Length; --i >= 0;)
            {
                var assetProp = preloadedAssetsProp.GetArrayElementAtIndex(i);
                // Property access forces Unity Editor to load asset
                assets[i] = assetProp.objectReferenceValue;
            }
#endif
    }
}
}
