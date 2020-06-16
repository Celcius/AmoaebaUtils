using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace  AmoaebaUtils
{

public class UserPersistance
{
    public delegate void UserPersistanceReloaded();

    public event UserPersistanceReloaded OnUserPersistanceReloaded;

    [Serializable]
    private struct SerializableWrapper<T> where T : System.Serializable
    {
        public T value;
    }


    [System.Serializable]
    private class UserPersistanceStorage
    {
        public string[] keys;
        public string[] values;

        public UserPersistanceStorage(string[] keys, string[] values)
        {
            this.keys = keys;
            this.values = values;
        }
    }
    
    public const string DEFAULT_USER = "GUEST";
    public const string EXT = ".asv";

    [SerializeField]
    private string loadedUserId = DEFAULT_USER;
    public string LoadedUserId => loadedUserId;

    private bool hasLoaded = false;
    public bool HasLoaded => hasLoaded;

    private Dictionary<string, string> cachedStoredData = new Dictionary<string, string>();
    private BinaryFormatter formatter = new BinaryFormatter();

    private string appName;

    public UserPersistance(string appName)
    {
        this.appName = appName.Trim().Replace(" ", "").Replace("/","");
    }

    public bool HasStoredData()
    {    
        string appPath = GetAppPath();
        return Directory.Exists(appPath);
    }

    public bool HasUserStoredData(string userId)
    {
        string path = GetSavePath(userId);
        return File.Exists(path);
    }

    public bool Contains(string key)
     {
         if(!hasLoaded)
         {
             return false;
         }
        return cachedStoredData.ContainsKey(key);
    }

    public void StoreString(string key, string value)                  
    {
       Assert.IsTrue(hasLoaded, "UserPersistance has not loaded");
        cachedStoredData[key] = value;
    }

    public string GetString(string key)                  
    {
        Assert.IsTrue(hasLoaded, "UserPersistance has not loaded");
        if(Contains(key))
        {
            return cachedStoredData[key];
        }

        return null;
    }

    public void StoreValue<T>(string key, T value)                  
    {
        Assert.IsTrue(hasLoaded, "UserPersistance has not loaded");

        string saveVal = "";
        Type type = typeof(T);
        if(type.IsPrimitive)
        {
            saveVal = "" + value;
        }
        else if(System.Object.ReferenceEquals(type, typeof(string)))
        {
            saveVal = value as T;
        }
        else
        {
            saveVal = JsonUtility.ToJson(value);
        }

        cachedStoredData[key] = saveVal;
    }

    public T GetValue<T>(string key)
    {
        Assert.IsTrue(hasLoaded, "UserPersistance has not loaded");
        return GetValue<T>(key, default(T));
    }     

    public T GetValue<T>(string key, T invalidRet)                  
    {
        Assert.IsTrue(hasLoaded, "UserPersistance has not loaded");
        if(!cachedStoredData.ContainsKey(key))
        {
            return invalidRet;
        }

        T savedVal = invalidRet;
        string value = cachedStoredData[key];

        Type type = typeof(T);
        if(type.IsPrimitive)
        {
            // TODO 
        }
        else if(System.Object.ReferenceEquals(type, typeof(string)))
        {
            savedVal = value;
        }
        else
        {
            savedVal = JsonUtility.ToJson(value);
        }

        return savedVal;
    }

    public string GetSavePath(string userId)
    {
        
        return GetAppPath() + "/"  + userId + EXT;
    }

    public string GetAppPath()
    {
        return Application.persistentDataPath + "/" + appName;
    }

    public void Save()
    {
        string[] keys = new string[cachedStoredData.Keys.Count];
        cachedStoredData.Keys.CopyTo(keys,0);

        string[] values = new string[cachedStoredData.Values.Count];
        cachedStoredData.Values.CopyTo(values,0);

        UserPersistanceStorage storage = new UserPersistanceStorage(keys, values);
        
        if(!HasStoredData())
        {
            Directory.CreateDirectory(GetAppPath());
        }

        string path = GetSavePath(loadedUserId);
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, storage);
        stream.Close();
    }

    public void LoadDefaultUser()
    {
        LoadUser(DEFAULT_USER);
    }

    public void LoadUser(string userId)
    {
        if(userId == null || string.Empty.CompareTo(userId) == 0)
        {
            Debug.LogError("Trying to load Empty User, fallbacking to defaultUser");
            userId = DEFAULT_USER;
        }

        string path = GetSavePath(userId);
        if(!File.Exists(path))
        {
            Debug.Log("No Save file found in " + path);
            ClearData(userId, false);
            return;
        }

        FileStream stream = new FileStream(path, FileMode.Open);
        UserPersistanceStorage storage = formatter.Deserialize(stream) as UserPersistanceStorage;
        stream.Close();

        if(storage == null)
        {
            Debug.LogError("Invalid format stored in path " + path);
            ClearData(userId, false);
            return;
        }

        cachedStoredData.Clear();
        for(int i = 0;i < storage.keys.Length;i++)
        {
            cachedStoredData.Add(storage.keys[i], storage.values[i]);
        }
        loadedUserId = userId;
        hasLoaded = true;
        OnUserPersistanceReloaded?.Invoke();
    }

    public void ClearData(string userId, bool overrideFiles = true)
    {
        loadedUserId = userId;
        cachedStoredData.Clear();
        
        if(overrideFiles)
        {
            Save();
        }
        else
        {
            string path = GetSavePath(userId);
            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }
        
        hasLoaded = true;
        OnUserPersistanceReloaded?.Invoke();
    }

    public void ClearAppData()
    {
        string path = GetAppPath();
        if(Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        hasLoaded = false;
        loadedUserId = UserPersistance.DEFAULT_USER;
    }
}
}
