using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
namespace AmoaebaUtils
{
public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        CreateAsset(asset);
    }

    public static void CreateAsset(ScriptableObject asset)
    {
        CreateAsset(asset, "/New " + (asset.GetType()).ToString());
    }

    public static void CreateAsset(ScriptableObject asset, string name)
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + name + ".asset");
        CreateAssetAtPath(asset,assetPathAndName);
    }

    public static void CreateAssetAtPath(ScriptableObject asset, string assetPathAndName)
    {
        FileUtils.GenerateFoldersForFile(assetPathAndName);
        
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = asset;
        EditorUtility.FocusProjectWindow();
    }
}
}
#endif