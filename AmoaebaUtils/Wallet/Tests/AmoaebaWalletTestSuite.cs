using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;

namespace AmoaebaUtils
{

public class AmoaebaWalletTestSuite
{
    public class TestProduct : IProduct
    {
        public string type;
        public int key;

        public TestProduct(int key, string type)
        {
            this.key = key;
            this.type = type;
        }
        
        public override string GetProductType()
        {
            return type;
        }

        public override int GetProductId()
        {
            return key;
        }

    }

    private SimpleWallet wallet;

    [SetUp]
    public void Setup()
    {
        wallet = new SimpleWallet();
    }

    [TearDown]
    public void Teardown()
    {
       wallet.ClearWallet();
    }

    [UnityTest]
    public IEnumerator TestInitConditions()
    {
        IProduct[] products = wallet.GetProducts();
        IProduct[] ownedProducts = wallet.GetOwnedProducts();
    
        Assert.IsTrue(products.Length == 0, " Wallet has initial products");
        Assert.IsTrue(ownedProducts.Length == 0, " Wallet has initial owned products");
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator RegisterProducts()
    {
        yield return TestInitConditions();

        TestProduct p =  new TestProduct(1, "a");
        TestProduct p2 =  new TestProduct(2, "b");
        TestProduct p3 =  new TestProduct(3, "a");
        TestProduct p4 =  new TestProduct(4, "b");

        IProduct[] toRegister = new IProduct[]{p, p2};
        
        wallet.RegisterProducts(toRegister, true);
        wallet.RegisterProduct(p3);
        wallet.RegisterProduct(p4);

        Assert.IsTrue(wallet.IsProductRegistered(p), " Product not registered ");
        Assert.IsTrue(wallet.IsProductRegistered(p2), " Product not registered ");
        Assert.IsTrue(wallet.IsProductRegistered(p3), " Product not registered ");
        Assert.IsTrue(wallet.IsProductRegistered(p4), " Product not registered ");
        Assert.IsTrue(wallet.GetProducts().Length == 4, " Unexpected amount of registered products ");
    }

    [UnityTest]
    public IEnumerator CleanRegisterProducts()
    {
        yield return TestInitConditions();

        TestProduct p =  new TestProduct(1, "a");
        TestProduct p2 =  new TestProduct(2, "b");
        TestProduct p3 =  new TestProduct(3, "a");
        TestProduct p4 =  new TestProduct(4, "b");
        TestProduct p5 =  new TestProduct(5, "b");

        IProduct[] toRegister = new IProduct[]{p, p2, p3, p4};
        
        wallet.RegisterProducts(toRegister, true);

        Assert.IsTrue(wallet.IsProductRegistered(p4), " Product not registered ");
        Assert.IsFalse(wallet.IsProductRegistered(p5), " Product registered ");
        Assert.IsTrue(wallet.GetProducts().Length == 4, " Unexpected amount of registered products ");

        toRegister = new IProduct[]{p, p2};

        wallet.RegisterProducts(toRegister, true);
        wallet.RegisterProduct(p3);

        Assert.IsFalse(wallet.IsProductRegistered(p4), " Product registered ");
        Assert.IsFalse(wallet.IsProductRegistered(p5), " Product registered ");
        Assert.IsTrue(wallet.GetProducts().Length == 3, " Unexpected amount of registered products ");

        toRegister = new IProduct[]{p4, p5};
        wallet.RegisterProducts(toRegister, false);

        Assert.IsTrue(wallet.IsProductRegistered(p4), " Product not registered ");
        Assert.IsTrue(wallet.IsProductRegistered(p5), " Product not registered ");
        Assert.IsTrue(wallet.GetProducts().Length == 5, " Unexpected amount of registered products ");
    }

    
/*
        public abstract void ClearWallet();

    public abstract void RegisterProducts(IProduct[] products, bool clearWallet);
    public abstract void RegisterProduct(IProduct product);
    public abstract bool IsProductRegistered(IProduct product);

    public abstract void ClearWalletAmounts();
    
    public abstract uint GetAmount(IProduct product);
    public abstract uint GetAmount(int productId);
    public abstract bool ChangeAmount(IProduct product, int amount, bool canOverflow = false);
    public abstract bool ChangeAmount(int productId, int amount, bool canOverflow = false);

    public abstract IProduct[] GetProducts();
    public abstract IProduct[] GetProductsByType(string type);
    public abstract IProduct[] GetOwnedProducts();
    public abstract IProduct[] GetOwnedProductsByType(string type);



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
    public IEnumerator TestDefaultStoreRetrieveValue()
{
        string toStore ="hey";
        string key ="key";

        int toStore2 = 25;
        string key2 ="key2";

        TestStruct toStore3 = new TestStruct(45, "asdasd");
        string key3 ="key3";

        bool toStore4 = true;
        string key4 ="key4";


        string toOverride = "Override";
        int toOverride2 = 234;
        TestStruct toOverride3 = new TestStruct(23543, "asdasf3e");

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

        Assert.IsFalse(persistance.HasStoredData(), "Persistance has no stored data");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");
    
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
        persistance.LoadDefaultUser();

        Assert.IsTrue(toOverride.CompareTo(persistance.GetValue<string>(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toOverride2 == persistance.GetValue<int>(key2), "Invalid store/retrieve");
        Assert.IsTrue(toOverride3.Same(persistance.GetValue<TestStruct>(key3)), "Invalid store/retrieve");
        Assert.IsTrue(toOverride4 == persistance.GetValue<bool>(key4), "Invalid store/retrieve");

        Assert.IsTrue(persistance.HasStoredData(), "Persistance has no stored data");
        Assert.IsTrue(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");

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

        TestStruct toStore3 = new TestStruct(45, "asdasd");
        string key3 ="key3";

        bool toStore4 = true;
        string key4 ="key4";


        string toOverride = "Override";
        int toOverride2 = 234;
        TestStruct toOverride3 = new TestStruct(23543, "asdasf3e");

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
    public IEnumerator TestSwapUser()
    {
        string user = "User123423";
        string toStore ="hey";
        string key ="key";

        string toStore2 = "hey2";
        string key2 ="key2";

        TestInitConditions();

        persistance.LoadDefaultUser();

        persistance.StoreValue<string>(key, toStore);
        persistance.StoreValue<string>(key2, toStore2);
      
        Assert.IsTrue(persistance.Contains(key), "Invalid key");
        Assert.IsTrue(persistance.Contains(key2), "Invalid key");

        Assert.IsTrue(toStore.CompareTo(persistance.GetValue<string>(key)) == 0, "Invalid store/retrieve");
        Assert.IsTrue(toStore2.CompareTo(persistance.GetValue<string>(key2)) == 0, "Invalid store/retrieve");

        persistance.LoadUser(user);
        
        Assert.IsFalse(persistance.Contains(key), "Invalid key");
        Assert.IsFalse(persistance.Contains(key2), "Invalid key");

        persistance.StoreValue<string>(key, toStore);
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
        Assert.IsTrue(toStore2.CompareTo(persistance.GetValue<string>(key2)) == 0, "Invalid store/retrieve");

        Assert.IsTrue(persistance.HasStoredData(), "Persistance has no stored data");
        Assert.IsFalse(persistance.HasUserStoredData(UserPersistance.DEFAULT_USER), "Persistance has default stored data");
        Assert.IsTrue(persistance.HasUserStoredData(user), "Persistance has no user stored data");

        yield return null;
    }*/


}
}