using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
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

   
    [UnityTest]
    public IEnumerator AddAmounts()
    {
        yield return TestInitConditions();

        TestProduct p =  new TestProduct(1, "a");
        TestProduct p2 =  new TestProduct(2, "b");
        TestProduct p3 =  new TestProduct(3, "a");
        TestProduct p4 =  new TestProduct(4, "b");

        IProduct[] toRegister = new IProduct[]{p, p2, p4};
        
        wallet.RegisterProducts(toRegister, true);

        Assert.IsTrue(wallet.GetAmount(p) == 0, "Unexpected Amount");
        Assert.IsTrue(wallet.GetAmount(p2) == 0, "Unexpected Amount");
        LogAssert.Expect(LogType.Error, "Product 3 not properly registered");
        Assert.IsTrue(wallet.GetAmount(p3) == 0, "Unexpected Amount");
        Assert.IsTrue(wallet.GetAmount(p4) == 0, "Unexpected Amount");
        Assert.IsFalse(wallet.IsProductRegistered(p3), "Unexpected product registered");

        bool didAddP1 = wallet.ChangeAmount(p, 1);
        bool didAddP2 = wallet.ChangeAmount(p2, 23);
        LogAssert.Expect(LogType.Error, "Product not registered for id 3");
        bool didAddP3 = wallet.ChangeAmount(p3, 99);
        bool didAddP4 = wallet.ChangeAmount(p4, 0);

        Assert.IsTrue(didAddP1, "Did Not add amount");
        Assert.IsTrue(didAddP2, "Did Not add amount");        
        Assert.IsFalse(didAddP3, "Added amount to unexpected product");
        Assert.IsTrue(didAddP4, "Did Not add amount");


        Assert.IsTrue(wallet.GetAmount(p) == 1,"Unexpected amount change");
        Assert.IsTrue(wallet.GetAmount(p2) == 23,"Unexpected amount change");
        LogAssert.Expect(LogType.Error, "Product 3 not properly registered");
        Assert.IsTrue(wallet.GetAmount(p3) == 0,"Unexpected amount change");
        Assert.IsTrue(wallet.GetAmount(p4) == 0,"Unexpected amount change");

        
     
    }

    [UnityTest]
    public IEnumerator RemoveAmount()
    {
        yield return TestInitConditions();

        TestProduct p =  new TestProduct(1, "a");
        TestProduct p2 =  new TestProduct(2, "b");
        TestProduct p3 =  new TestProduct(3, "a");
        TestProduct p4 =  new TestProduct(4, "b");
        TestProduct p5 =  new TestProduct(5, "b");

        IProduct[] toRegister = new IProduct[]{p, p2, p4, p5};
        
        wallet.RegisterProducts(toRegister, true);

        wallet.ChangeAmount(p, 1);
        wallet.ChangeAmount(p2, 23);
        LogAssert.Expect(LogType.Error, "Product not registered for id 3");
        wallet.ChangeAmount(p3, 99);
        wallet.ChangeAmount(p4, 10);
        wallet.ChangeAmount(p5, 10);

        bool didAddP1 = wallet.ChangeAmount(p, -1);
        bool didAddP2 = wallet.ChangeAmount(p2, -10);
        LogAssert.Expect(LogType.Error, "Product not registered for id 3");
        bool didAddP3 = wallet.ChangeAmount(p3, -20);
        bool didAddP4 = wallet.ChangeAmount(p4, -20, true);
        LogAssert.Expect(LogType.Error, "Change to product 5 overflowed negatively");
        bool didAddP5 = wallet.ChangeAmount(p5, -20, false);
        
        
        Assert.IsTrue(didAddP1, "Did Not remove amount");
        Assert.IsTrue(didAddP2, "Did Not remove amount");        
        Assert.IsFalse(didAddP3, "Added remove to unexpected product");
        Assert.IsTrue(didAddP4, "Did Not remove amount");
        Assert.IsFalse(didAddP5, "Did Not remove amount");

        Assert.IsTrue(wallet.GetAmount(p) == 0,"Unexpected amount change");
        Assert.IsTrue(wallet.GetAmount(p2) == 13,"Unexpected amount change");
        LogAssert.Expect(LogType.Error, "Product 3 not properly registered");
        Assert.IsTrue(wallet.GetAmount(p3) == 0,"Unexpected amount change");
        Assert.IsTrue(wallet.GetAmount(p4) == 0,"Unexpected amount change");
        Assert.IsTrue(wallet.GetAmount(p5) == 10,"Unexpected amount change");
    }


    [UnityTest]
    public IEnumerator GetAmount()
    {
        yield return TestInitConditions();
        TestProduct p =  new TestProduct(1, "a");
        TestProduct p2 =  new TestProduct(2, "b");
        IProduct[] toRegister = new IProduct[]{p};
        
        wallet.RegisterProducts(toRegister, true);

        wallet.ChangeAmount(p, 1);
        
        uint amount1 = wallet.GetAmount(p);
        uint amount2 = wallet.GetAmount(p.GetProductId());

        LogAssert.Expect(LogType.Error, "Product 2 not properly registered");
        uint amount3 = wallet.GetAmount(p2);
        LogAssert.Expect(LogType.Error, "Product 2 not properly registered");
        uint amount4 = wallet.GetAmount(p2.GetProductId());

        Assert.True(amount1 == 1, "Unexpected Amount");
        Assert.True(amount2 == 1, "Unexpected Amount");
        Assert.True(amount3 == 0, "Unexpected Amount");
        Assert.True(amount4 == 0, "Unexpected Amount");
        
    }

    [UnityTest]
    public IEnumerator AddDuplicateProducts()
    {
        yield return TestInitConditions();
           yield return TestInitConditions();
        TestProduct p =  new TestProduct(1, "a");
        TestProduct p2 =  new TestProduct(p.GetProductId(), "b");
        
        IProduct[] toRegister = new IProduct[]{p,p,p2};

        LogAssert.Expect(LogType.Error, "Trying to register duplicate products for ID 1");
        LogAssert.Expect(LogType.Error, "Trying to register duplicate products for ID 1");
        wallet.RegisterProducts(toRegister, true);
        LogAssert.Expect(LogType.Error, "Trying to register duplicate products for ID 1");
        wallet.RegisterProduct(p);
        LogAssert.Expect(LogType.Error, "Trying to register duplicate products for ID 1");
        wallet.RegisterProduct(p2);
    }

    [UnityTest]
    public IEnumerator CleanAmount()
    {
        yield return TestInitConditions();

        
        TestProduct p =  new TestProduct(1, "a");
        TestProduct p2 =  new TestProduct(2, "b");
        TestProduct p3 =  new TestProduct(3, "a");

        IProduct[] toRegister = new IProduct[]{p, p2, p3};
        
        wallet.RegisterProducts(toRegister, true);
        wallet.ChangeAmount(p, 1);
        wallet.ChangeAmount(p2, 23);
        wallet.ChangeAmount(p3, 99);
        wallet.ChangeAmount(p3, -50);

        Assert.IsTrue(wallet.GetAmount(p) == 1, "Unexpected Amount");
        Assert.IsTrue(wallet.GetAmount(p2) == 23, "Unexpected Amount");
        Assert.IsTrue(wallet.GetAmount(p3) == 49, "Unexpected Amount");
        
        wallet.ClearWalletAmounts();
        
        Assert.IsTrue(wallet.IsProductRegistered(p), "Product not registered after clean");
        Assert.IsTrue(wallet.IsProductRegistered(p2), "Product not registered after clean");
        Assert.IsTrue(wallet.IsProductRegistered(p3), "Product not registered after clean");
        Assert.IsTrue(wallet.GetAmount(p) == 0, "Unexpected Amount");
        Assert.IsTrue(wallet.GetAmount(p2) == 0, "Unexpected Amount");
        Assert.IsTrue(wallet.GetAmount(p3) == 0, "Unexpected Amount");
    }


    [UnityTest]
    public IEnumerator GetOwnedProducts()
    {
        yield return TestInitConditions();

        TestProduct p =  new TestProduct(1, "a");
        TestProduct p2 =  new TestProduct(2, "b");
        TestProduct p3 =  new TestProduct(3, "a");
        TestProduct p4 =  new TestProduct(4, "b");
        TestProduct p5 =  new TestProduct(5, "a");
        TestProduct p6 =  new TestProduct(6, "b");


        IProduct[] toRegister = new IProduct[]{p, p2, p3, p4};
        
        wallet.RegisterProducts(toRegister, true);

        wallet.ChangeAmount(p, 1);
        wallet.ChangeAmount(p2, 0);
        wallet.ChangeAmount(p3, 3);
        wallet.ChangeAmount(p3, -2);
        wallet.ChangeAmount(p4, 4);
        wallet.ChangeAmount(p4, -4);
        LogAssert.Expect(LogType.Error, "Product not registered for id 5");
        wallet.ChangeAmount(p5, 5);
        LogAssert.Expect(LogType.Error, "Product not registered for id 6");
        wallet.ChangeAmount(p6, 6);

        IProduct[] products = wallet.GetOwnedProducts();
        ISet<IProduct> productSet = new HashSet<IProduct>(products, new IProductEqualityComparer());

        Assert.IsTrue(productSet.Contains(p), "Product not owned");
        Assert.IsFalse(productSet.Contains(p2), "Product owned");
        Assert.IsTrue(productSet.Contains(p3), "Product not owned");
        Assert.IsFalse(productSet.Contains(p4), "Product owned");
        Assert.IsFalse(productSet.Contains(p5), "Product owned");
        Assert.IsFalse(productSet.Contains(p6), "Product owned");
    }

    
    [UnityTest]
    public IEnumerator GetProductsByType()
    {
        yield return TestInitConditions();

         yield return TestInitConditions();

        TestProduct p =  new TestProduct(1, "a");
        TestProduct p2 =  new TestProduct(2, "b");
        TestProduct p3 =  new TestProduct(3, "a");
        TestProduct p4 =  new TestProduct(4, "b");
        TestProduct p5 =  new TestProduct(5, "a");
        TestProduct p6 =  new TestProduct(6, "b");

        IProduct[] toRegister = new IProduct[]{p, p2, p3, p4};
        
        wallet.RegisterProducts(toRegister, true);
        
        
        IProduct[] productsA = wallet.GetProductsByType("a");
        ISet<IProduct> productSetA = new HashSet<IProduct>(productsA, new IProductEqualityComparer());
        IProduct[] productsB = wallet.GetProductsByType("b");
        ISet<IProduct> productSetB = new HashSet<IProduct>(productsB, new IProductEqualityComparer());

        Assert.IsTrue(productSetA.Count == 2, "Wrong amount of products in set A");
        Assert.IsTrue(productSetA.Contains(p), "Product not owned");
        Assert.IsFalse(productSetA.Contains(p2), "Product owned in set A");
        Assert.IsTrue(productSetA.Contains(p3), "Product not owned");
        Assert.IsFalse(productSetA.Contains(p4), "Product owned in set A");
        Assert.IsFalse(productSetA.Contains(p5), "Product owned in set A");
        Assert.IsFalse(productSetA.Contains(p6), "Product owned in set A");
        
        Assert.IsTrue(productSetB.Count == 2, "Wrong amount of products in set B");
        Assert.IsFalse(productSetB.Contains(p), "Product owned in set B");
        Assert.IsTrue(productSetB.Contains(p2), "Product owned in set B");
        Assert.IsFalse(productSetB.Contains(p3), "Product owned in set B");
        Assert.IsTrue(productSetB.Contains(p4), "Product owned in set B");
        Assert.IsFalse(productSetB.Contains(p5), "Product not owned in set B");
        Assert.IsFalse(productSetB.Contains(p6), "Product owned in set B");

    }

    [UnityTest]
    public IEnumerator GetOwnedProductsByType()
    {
        yield return TestInitConditions();

        TestProduct p =  new TestProduct(1, "a");
        TestProduct p2 =  new TestProduct(2, "b");
        TestProduct p3 =  new TestProduct(3, "a");
        TestProduct p4 =  new TestProduct(4, "b");
        TestProduct p5 =  new TestProduct(5, "a");
        TestProduct p6 =  new TestProduct(6, "b");
        TestProduct p7 =  new TestProduct(7, "a");

        IProduct[] toRegister = new IProduct[]{p, p2, p3, p4, p6};
        
        wallet.RegisterProducts(toRegister, true);

        wallet.ChangeAmount(p, 1);
        wallet.ChangeAmount(p2, 0);
        wallet.ChangeAmount(p3, 3);
        wallet.ChangeAmount(p3, -2);
        wallet.ChangeAmount(p4, 4);
        wallet.ChangeAmount(p4, -4);
        LogAssert.Expect(LogType.Error, "Product not registered for id 5");
        wallet.ChangeAmount(p5, 5);
        wallet.ChangeAmount(p6, 6);
        LogAssert.Expect(LogType.Error, "Product not registered for id 7");
        wallet.ChangeAmount(p7, 7);


        IProduct[] productsA = wallet.GetOwnedProductsByType("a");
        ISet<IProduct> productSetA = new HashSet<IProduct>(productsA, new IProductEqualityComparer());
        IProduct[] productsB = wallet.GetOwnedProductsByType("b");
        ISet<IProduct> productSetB = new HashSet<IProduct>(productsB, new IProductEqualityComparer());

        Assert.IsTrue(productSetA.Count == 2, "Wrong amount of products in set A");
        Assert.IsTrue(productSetA.Contains(p), "Product not owned");
        Assert.IsFalse(productSetA.Contains(p2), "Product owned in set A");
        Assert.IsTrue(productSetA.Contains(p3), "Product not owned");
        Assert.IsFalse(productSetA.Contains(p4), "Product owned in set A");
        Assert.IsFalse(productSetA.Contains(p5), "Product owned in set A");
        Assert.IsFalse(productSetA.Contains(p6), "Product owned in set A");
        Assert.IsFalse(productSetA.Contains(p7), "Product owned in set A");
        
        Assert.IsTrue(productSetB.Count == 1, "Wrong amount of products in set B");
        Assert.IsFalse(productSetB.Contains(p), "Product owned in set B");
        Assert.IsFalse(productSetB.Contains(p2), "Product owned in set B");
        Assert.IsFalse(productSetB.Contains(p3), "Product owned in set B");
        Assert.IsFalse(productSetB.Contains(p4), "Product owned in set B");
        Assert.IsFalse(productSetB.Contains(p5), "Product owned in set B");
        Assert.IsTrue(productSetB.Contains(p6), "Product not owned in set B");
        Assert.IsFalse(productSetB.Contains(p7), "Product owned in set B");
    }
}
}
