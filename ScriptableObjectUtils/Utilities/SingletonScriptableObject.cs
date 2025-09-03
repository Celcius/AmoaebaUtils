using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{

    static T _instance = null;
    public static T Instance
    {
        get
        {
            if (!_instance) {
                T[] instances = Resources.FindObjectsOfTypeAll<T>();
                if(instances.Length > 1)
                {
                    Debug.LogError("More than one instance for singleton " + typeof(T));
                }
                _instance = instances.Length > 0 ? instances[0] : null;
            }
            return _instance;
        }
    }
}
