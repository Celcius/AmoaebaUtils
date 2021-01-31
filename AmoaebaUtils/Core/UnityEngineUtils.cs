using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR     
using UnityEditor;
#endif

namespace AmoaebaUtils
{
public static class UnityEngineUtils
{
    private static System.Random random = new System.Random((int)DateTime.Now.ToBinary());
    public static float AnimationCurveDuration(AnimationCurve curve)
    {
        return (curve.keys.Length == 0)? 0.0f : curve.keys[curve.keys.Length-1].time;
    }

    public static Vector2 WorldOrthographicSize(Camera cam, bool considerOrientation = true)
    {
        Vector2 bounds = new Vector2(2*cam.orthographicSize, 2*cam.orthographicSize*cam.aspect);
        bounds = (Input.deviceOrientation == DeviceOrientation.Portrait || 
                 Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) ?
                 new Vector2(Mathf.Min(bounds.x, bounds.y), Mathf.Max(bounds.x, bounds.y)) : 
                 new Vector2(Mathf.Max(bounds.x, bounds.y), Mathf.Min(bounds.x, bounds.y));
        return bounds;
    }

    public static bool IsInPlayModeOrAboutToPlay()
    {
#if UNITY_EDITOR        
        return (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode);
#else
        return Application.isPlaying ;        
#endif        
    }

    public static bool WillChangeToPlayMode()
    {
#if UNITY_EDITOR        
        return EditorApplication.isPlayingOrWillChangePlaymode;
#else
        return false;        
#endif        
    }

    public static Color InvertColor(Color rgbColor) 
    {
        float h, s, v;
        Color.RGBToHSV(rgbColor, out h, out s, out v);
        return Color.HSVToRGB((h + 0.5f) % 1, s, v);
    }

    public static Color NegativeColor(Color color)
    {
        return new Color(1.0f-color.r, 1.0f-color.g, 1.0f-color.b);
    }

    public static string RandomString(int len)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string ret = "";
        for(int i = 0; i < len; i++)
        {
            ret += chars[random.Next(0, chars.Length)]; 
        }
        return ret;
    }

    public static T GetRandomEnumElement<T>() where T : System.Enum
    {        
        Array elements = System.Enum.GetValues(typeof(T));
        return (T) elements.GetValue(random.Next(0, elements.Length));
    }

    public static HashSet<T> GetEnumHash<T>() where T : System.Enum
    {
        System.Array all = System.Enum.GetValues(typeof(T));
    
        HashSet<T> allHash = new HashSet<T>();

        foreach(T item in all)
        {
            allHash.Add(item);
        }
        return allHash;
    }

    public static string CreateAnagram(string word)
    {
        string anagram = "";
        
        List<int> indexes = new List<int>();
        for(int i = 0; i < word.Length; i++)
        {
            indexes.Add(i);
        }

        for(int i = 0; i < word.Length; i++)
        {
            int index = random.Next(0, indexes.Count);
            anagram += word[indexes[index]];
            indexes.RemoveAt(index);
        }

        return anagram;
    }

#if UNITY_EDITOR     

    public static string[] GetGUIDSForType<T>() where T: UnityEngine.Object
    {
        return AssetDatabase.FindAssets("t:" + typeof(T).ToString());
    }


    public static T[] GetAllOfType<T>() where T : UnityEngine.Object
    {
       List<T> objects = new List<T>();
       string[] guids = GetGUIDSForType<T>();
       foreach(string guid in guids)
       {
           string path = AssetDatabase.GUIDToAssetPath(guid);
           T entity = AssetDatabase.LoadAssetAtPath<T>(path);
           if(entity != null)
           {
               objects.Add(entity);
           }
       }
       return objects.ToArray();
    }


    public static void SelectFirstObjectOfType<T>() where T : UnityEngine.Object
    {
       string[] guids = GetGUIDSForType<T>();
       foreach(string guid in guids)
       {
           string path = AssetDatabase.GUIDToAssetPath(guid);
           T entity = AssetDatabase.LoadAssetAtPath<T>(path);
           if(entity != null)
           {
               Selection.activeObject = entity;
               EditorUtility.FocusProjectWindow(); 
               return;
           }
       }
    }
#endif
}
}