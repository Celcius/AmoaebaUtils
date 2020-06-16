using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace AmoaebaUtils
{
public class AmoaebaPersistanceTestSuite
{
    private UserPersistance persistance;
    private bool calledCallback = false;
    private string saveApp = "AmoaebaUtils_TestSuite";

    struct TestStruct
    {
        public int x;
        public string a;

        public bool Same(TestStruct y)
        {
            return y.x == x && a.CompareTo(y.a) == 0;
        }
    }

    [SetUp]
    public void Setup()
    {
        persistance = new UserPersistance(saveApp);
        persistance.ClearAppData();
        persistance.OnUserPersistanceReloaded += ReloadCallback;
    }
    
    private void ReloadCallback()
    {
        calledCallback = true;
    }

    [TearDown]
    public void Teardown()
    {
        persistance.ClearAppData();
        calledCallback = false;
    }

    private void TestInitConditions()
    {
        Assert.IsFalse(persistance.HasLoaded, "Persistance loaded at start of test");
        Assert.IsFalse(calledCallback, "Callback called at start of test");
        Assert.IsTrue(persistance.LoadedUserId.CompareTo(UserPersistance.DEFAULT_USER) == 0, "Persistance with wrong user at start of test");
        Assert.IsFalse(persistance.HasStoredData(), "Persistance has stored data at start");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has stored default user data at start");
    }

    [UnityTest]
    public IEnumerator TestSavePath()
    {
        TestInitConditions();

        string user = "TestUser";
        string savePath = persistance.GetSavePath(user);
        string expected = Application.persistentDataPath + "/" + saveApp + "/" + user + UserPersistance.EXT;

        Assert.IsTrue(savePath.CompareTo(expected) == 0, " Incorrect SavePath");
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestAppPath()
    {
        TestInitConditions();

        string appPath = persistance.GetAppPath();
        string expected = Application.persistentDataPath + "/" + saveApp;

        Assert.IsTrue(appPath.CompareTo(expected) == 0, " Incorrect AppPath");
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestLoadDefaultUser()
    {
        TestInitConditions();

        persistance.LoadDefaultUser();
        Assert.IsTrue(persistance.HasLoaded, "Persistance not loaded TestLoadDefaultUser");
        Assert.IsTrue(persistance.LoadedUserId.CompareTo(UserPersistance.DEFAULT_USER) == 0, "Persistance with wrong user at TestLoadDefaultUser");
        Assert.IsTrue(calledCallback, "Persistance did not call callback");
        Assert.IsFalse(persistance.HasStoredData(), "Persistance has stored data");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has stored data");

        yield return null;
    }

   [UnityTest]
    public IEnumerator TestLoadUser()
    {
        TestInitConditions();

        string user = "TestUser1";
        persistance.LoadUser(user);

        Assert.IsTrue(persistance.HasLoaded, "Persistance not loaded TestLoadUser");
        Assert.IsTrue(persistance.LoadedUserId.CompareTo(user) == 0, "Persistance with wrong user at TestLoadUser");
        Assert.IsTrue(calledCallback, "Persistance did not call callback");
        Assert.IsFalse(persistance.HasStoredData(), "Persistance has stored data");
        Assert.IsFalse(persistance.HasUserStoredData(user), "Persistance has stored data");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestClearAndLoad()
    {
        TestInitConditions();

        yield return TestLoadDefaultUser();
        
        persistance.ClearAppData();

        Assert.IsFalse(persistance.HasLoaded, "Persistance not loaded ");
        calledCallback = false;

        persistance.LoadDefaultUser();

        Assert.IsTrue(persistance.HasLoaded, "Persistance not loaded TestLoadUser");
        Assert.IsTrue(persistance.LoadedUserId.CompareTo(UserPersistance.DEFAULT_USER) == 0, "Persistance with wrong user at TestLoadUser");
        Assert.IsTrue(calledCallback, "Persistance did not call callback");


        yield return null;
    }


    [UnityTest]
    public IEnumerator TestDefaultSave()
    {
        TestInitConditions();

        yield return TestLoadDefaultUser();

        persistance.Save();

        Assert.IsTrue(persistance.HasLoaded, "Persistance not loaded TestLoadUser");
        Assert.IsTrue(persistance.LoadedUserId.CompareTo(UserPersistance.DEFAULT_USER) == 0, "Persistance with wrong user at TestLoadUser");
        Assert.IsTrue(calledCallback, "Persistance did not call callback");
        Assert.IsTrue(persistance.HasStoredData(), "Persistance has stored data");
        Assert.IsTrue(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has stored data");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestUserSave()
    {
        TestInitConditions();

        string user = "TestUser1";
        persistance.LoadUser(user);
        persistance.Save();

        Assert.IsTrue(persistance.HasLoaded, "Persistance not loaded TestLoadUser");
        Assert.IsTrue(persistance.LoadedUserId.CompareTo(user) == 0, "Persistance with wrong user at TestLoadUser");
        Assert.IsTrue(calledCallback, "Persistance did not call callback");
        Assert.IsTrue(persistance.HasStoredData(), "Persistance has stored data");
        Assert.IsTrue(persistance.HasUserStoredData(user), "Persistance has stored data");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestDefaultStoreRetrieveString()
    {
        string toStore ="hey";
        string key ="key";

        string toStore2 ="hey2";
        string key2 ="key2";

        string toOverride = "Override";

        Assert.IsFalse(persistance.Contains(key), "Invalid key");
        Assert.IsFalse(persistance.Contains(key2), "Invalid key");
        TestInitConditions();

        persistance.LoadDefaultUser();
        persistance.StoreString(key, toStore);
        persistance.StoreString(key2, toStore2);

        Assert.IsTrue(persistance.Contains(key), "Invalid key");
        Assert.IsTrue(persistance.Contains(key2), "Invalid key");
        
        Assert.IsTrue(toStore.CompareTo(persistance.GetString(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2.CompareTo(persistance.GetString(key2)) == 0, "Invalid store/retrieve");

        persistance.StoreString(key, toOverride);

        Assert.IsTrue(toOverride.CompareTo(persistance.GetString(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2.CompareTo(persistance.GetString(key2)) == 0, "Invalid store/retrieve");
    
        Assert.IsFalse(persistance.HasStoredData(), "Persistance has stored data");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");

        persistance.Save();

        persistance = new UserPersistance(saveApp);
        persistance.LoadDefaultUser();

        Assert.IsTrue(toOverride.CompareTo(persistance.GetString(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2.CompareTo(persistance.GetString(key2)) == 0, "Invalid store/retrieve");
    
        Assert.IsTrue(persistance.HasStoredData(), "Persistance has stored data");
        Assert.IsTrue(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");

        yield return null;
    }

    
    [UnityTest]
    public IEnumerator TestUserStoreRetrieveString()
    {
        string user = "User1";
        string toStore ="hey";
        string key ="key";

        string toStore2 ="hey2";
        string key2 ="key2";

        string toOverride = "Override";

        Assert.IsFalse(persistance.Contains(key), "Invalid key");
        Assert.IsFalse(persistance.Contains(key2), "Invalid key");

        TestInitConditions();

        persistance.LoadUser(user);
        persistance.StoreString(key, toStore);
        persistance.StoreString(key2, toStore2);

        Assert.IsTrue(persistance.Contains(key), "Invalid key");
        Assert.IsTrue(persistance.Contains(key2), "Invalid key");
        
        Assert.IsTrue(toStore.CompareTo(persistance.GetString(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2.CompareTo(persistance.GetString(key2)) == 0, "Invalid store/retrieve");

        persistance.StoreString(key, toOverride);

        Assert.IsTrue(toOverride.CompareTo(persistance.GetString(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2.CompareTo(persistance.GetString(key2)) == 0, "Invalid store/retrieve");
    
        Assert.IsFalse(persistance.HasStoredData(), "Persistance has stored data");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");
        Assert.IsFalse(persistance.HasUserStoredData(user), "Persistance has user stored data");

        persistance.Save();

        persistance = new UserPersistance(saveApp);
        persistance.LoadUser(user);

        Assert.IsTrue(toOverride.CompareTo(persistance.GetString(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2.CompareTo(persistance.GetString(key2)) == 0, "Invalid store/retrieve");
    
        Assert.IsTrue(persistance.HasStoredData(), "Persistance has stored data");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");
        Assert.IsTrue(persistance.HasUserStoredData(user), "Persistance has user stored data");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestDefaultStoreRetrieveValue()
    {
        string toStore ="hey";
        string key ="key";

        int toStore2 = 25;
        string key2 ="key2";

        TestStruct toStore3;
        toStore3.x = 45;
        toStore3.a = "asdasd";
        string key3 ="key3";

        bool toStore4 = true;
        string key4 ="key4";


        string toOverride = "Override";
        int toOverride2 = 234;
        TestStruct toOverride3;
        toOverride3.x = 23543;
        toOverride3.a = "asdasf3e";

        bool toOverride4 = false;

        TestInitConditions();

        persistance.LoadDefaultUser();

        persistance.StoreValue<string>(key, toStore);
        persistance.StoreValue<int>(key2, toStore2);
        persistance.StoreValue<TestStruct>(key3, toStore3);
        persistance.StoreValue<bool>(key4, toStore4);
    
        Assert.IsTrue(persistance.Contains(key), "Invalid key");
        Assert.IsTrue(persistance.Contains(key2), "Invalid key");
        Assert.IsTrue(persistance.Contains(key3), "Invalid key");
        Assert.IsTrue(persistance.Contains(key4), "Invalid key");
    
        Assert.IsTrue(toStore.CompareTo(persistance.GetValue<string>(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2 == persistance.GetValue<int>(key2), "Invalid store/retrieve");
        Assert.IsTrue(toStore3.Same(persistance.GetValue<TestStruct>(key3)), "Invalid store/retrieve");
        Assert.IsTrue(toStore4 == persistance.GetValue<bool>(key4), "Invalid store/retrieve");
        
        Assert.IsFalse(persistance.HasStoredData(), "Persistance has no stored data");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");

        persistance.StoreValue<string>(key, toOverride);
        persistance.StoreValue<int>(key2, toOverride2);
        persistance.StoreValue<TestStruct>(key3, toOverride3);
        persistance.StoreValue<bool>(key3, toOverride4);
        persistance.Save();

        persistance = new UserPersistance(saveApp);
        persistance.LoadDefaultUser();

        Assert.IsTrue(toOverride.CompareTo(persistance.GetValue<string>(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toOverride2 == persistance.GetValue<int>(key2), "Invalid store/retrieve");
        Assert.IsTrue(toOverride3.Same(persistance.GetValue<TestStruct>(key3)), "Invalid store/retrieve");
        Assert.IsTrue(toOverride4 == persistance.GetValue<bool>(key4), "Invalid store/retrieve");

        Assert.IsTrue(persistance.HasStoredData(), "Persistance has no stored data");
        Assert.IsTrue(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has no default stored data");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestUserStoreRetrieveValue()
    {
        string user ="UserA";

        string toStore ="hey";
        string key ="key";

        int toStore2 = 25;
        string key2 ="key2";

        TestStruct toStore3;
        toStore3.x = 45;
        toStore3.a = "asdasd";
        string key3 ="key3";

        bool toStore4 = true;
        string key4 ="key4";


        string toOverride = "Override";
        int toOverride2 = 234;
        TestStruct toOverride3;
        toOverride3.x = 23543;
        toOverride3.a = "asdasf3e";

        bool toOverride4 = false;

        TestInitConditions();

        persistance.LoadUser(user);

        persistance.StoreValue<string>(key, toStore);
        persistance.StoreValue<int>(key2, toStore2);
        persistance.StoreValue<TestStruct>(key3, toStore3);
        persistance.StoreValue<bool>(key4, toStore4);
    
        Assert.IsTrue(persistance.Contains(key), "Invalid key");
        Assert.IsTrue(persistance.Contains(key2), "Invalid key");
        Assert.IsTrue(persistance.Contains(key3), "Invalid key");
        Assert.IsTrue(persistance.Contains(key4), "Invalid key");

        Assert.IsFalse(persistance.HasStoredData(), "Persistance has no stored data");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");
        Assert.IsFalse(persistance.HasUserStoredData(user), "Persistance has no user stored data");
    
        Assert.IsTrue(toStore.CompareTo(persistance.GetValue<string>(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2 == persistance.GetValue<int>(key2), "Invalid store/retrieve");
        Assert.IsTrue(toStore3.Same(persistance.GetValue<TestStruct>(key3)), "Invalid store/retrieve");
        Assert.IsTrue(toStore4 == persistance.GetValue<bool>(key4), "Invalid store/retrieve");

        persistance.StoreValue<string>(key, toOverride);
        persistance.StoreValue<int>(key2, toOverride2);
        persistance.StoreValue<TestStruct>(key3, toOverride3);
        persistance.StoreValue<bool>(key4, toOverride4);
        persistance.Save();

        persistance = new UserPersistance(saveApp);
        persistance.LoadUser(user);

        Assert.IsTrue(toOverride.CompareTo(persistance.GetValue<string>(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toOverride2 == persistance.GetValue<int>(key2), "Invalid store/retrieve");
        Assert.IsTrue(toOverride3.Same(persistance.GetValue<TestStruct>(key3)), "Invalid store/retrieve");
        Assert.IsTrue(toOverride4 == persistance.GetValue<bool>(key4), "Invalid store/retrieve");

        Assert.IsTrue(persistance.HasStoredData(), "Persistance has no stored data");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");
        Assert.IsTrue(persistance.HasUserStoredData(user), "Persistance has no user stored data");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestStringToValueStoreRetrieve()
    {
        string toStore ="hey";
        string key ="key";

        string toStore2 = "hey2";
        string key2 ="key2";

        string toOverride = "bla";
        string toOverride2 = "bl2";

        TestInitConditions();

        persistance.LoadDefaultUser();

        persistance.StoreString(key, toStore);
        persistance.StoreValue<string>(key2, toStore2);
      
        Assert.IsTrue(persistance.Contains(key), "Invalid key");
        Assert.IsTrue(persistance.Contains(key2), "Invalid key");

        Assert.IsTrue(toStore.CompareTo(persistance.GetValue<string>(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2.CompareTo(persistance.GetString(key2)) == 0, "Invalid store/retrieve");

        
        persistance.StoreValue<string>(key, toOverride);
        persistance.StoreString(key2, toOverride2);

        persistance.Save();
        persistance = new UserPersistance(saveApp);
        persistance.LoadDefaultUser();

        Assert.IsTrue(persistance.Contains(key), "Invalid key");
        Assert.IsTrue(toOverride2.Contains(key2), "Invalid key");

        Assert.IsTrue(toOverride.CompareTo(persistance.GetValue<string>(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2.CompareTo(persistance.GetString(key2)) == 0, "Invalid store/retrieve");

        yield return null;
    }

     [UnityTest]
    public IEnumerator TestSwapUser()
    {
        string user = "User123423";
        string toStore ="hey";
        string key ="key";

        string toStore2 = "hey2";
        string key2 ="key2";

        TestInitConditions();

        persistance.LoadDefaultUser();

        persistance.StoreString(key, toStore);
        persistance.StoreValue<string>(key2, toStore2);
      
        Assert.IsTrue(persistance.Contains(key), "Invalid key");
        Assert.IsTrue(persistance.Contains(key2), "Invalid key");

        Assert.IsTrue(toStore.CompareTo(persistance.GetValue<string>(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2.CompareTo(persistance.GetString(key2)) == 0, "Invalid store/retrieve");

        persistance.LoadUser(user);
        
        Assert.IsFalse(persistance.Contains(key), "Invalid key");
        Assert.IsFalse(persistance.Contains(key2), "Invalid key");

        persistance.StoreString(key, toStore);
        persistance.StoreValue<string>(key2, toStore2);
        persistance.Save();

        persistance.LoadDefaultUser();
        Assert.IsFalse(persistance.Contains(key), "Invalid key");
        Assert.IsFalse(persistance.Contains(key2), "Invalid key");

        persistance = new UserPersistance(saveApp);
        persistance.LoadUser(user);
              
        Assert.IsTrue(persistance.Contains(key), "Invalid key");
        Assert.IsTrue(persistance.Contains(key2), "Invalid key");

        Assert.IsTrue(toStore.CompareTo(persistance.GetValue<string>(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2.CompareTo(persistance.GetString(key2)) == 0, "Invalid store/retrieve");

        Assert.IsTrue(persistance.HasStoredData(), "Persistance has no stored data");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");
        Assert.IsTrue(persistance.HasUserStoredData(user), "Persistance has no user stored data");

        yield return null;
    }
}
}