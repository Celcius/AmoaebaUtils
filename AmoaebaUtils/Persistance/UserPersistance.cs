using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MessagePack;
using MessagePack.Resolvers;

namespace  AmoaebaUtils
{

public class UserPersistance
{
    public delegate void UserPersistanceReloaded();

    public event UserPersistanceReloaded OnUserPersistanceReloaded;

    [System.Serializable]
    private class UserPersistanceStorage
    {
        public string[] keys;
        public byte[][] values;

        public UserPersistanceStorage(string[] keys, byte[][] values)
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

    private Dictionary<string, byte[]> cachedStoredData = new Dictionary<string, byte[]>();
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

    public bool StoreValue<T>(string key, T value)  
    {
        Assert.IsTrue(hasLoaded, "UserPersistance has not loaded");

        try 
        {
            byte[] bytes = MessagePackSerializer.Serialize<T>(value);
            cachedStoredData[key] = bytes;
            return true;
        } 
        catch(Exception e)
        {
            Debug.Log("Unable to Store Value " + value);
        }
                    return false;
    }
        /*

            if (value is ScriptableObject)
            {
                serializedValue = SerializeValue(JsonUtility.ToJson(value, false));
            }
            else serializedValue = SerializeValue(value);

    }

            protected virtual T DeserializeValue<T>(byte[] serializedValue)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(typeof(T)))
            {
                var jsonValue = MessagePackSerializer.Deserialize<string>(serializedValue);

                object res = ScriptableObject.CreateInstance(typeof(T));
                JsonUtility.FromJsonOverwrite(jsonValue, res);
                return (T)res;
            }
            else
            {
                return MessagePackSerializer.Deserialize<T>(serializedValue);

            }
        }*/

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

        try 
        {
            byte[] value = cachedStoredData[key];
            T retVal = MessagePackSerializer.Deserialize<T>(value);
            return retVal;
        }
        catch(Exception e)
        {
            Debug.Log("Unable to Deserialize " + key +" dump: " + e.Message);
        }
        return invalidRet;
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

        byte[][] values = new byte[cachedStoredData.Values.Count][];
        cachedStoredData.Values.CopyTo(values, 0);

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
