using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class SimpleWallet : IWallet
{
    private Dictionary<string, List<IProduct>> productsByType = new Dictionary<string, List<IProduct>>();
    private Dictionary<int, ProductAmount> productAmounts = new Dictionary<int, ProductAmount>();
    private ISet<IProduct> registeredProducts = new HashSet<IProduct>(new IProductEqualityComparer());

    public override void ClearWallet()
    {
        productsByType.Clear();
        productAmounts.Clear();
        registeredProducts.Clear();
    }

    public override void RegisterProducts(IProduct[] products, bool clearWallet)
    {
        if(clearWallet)
        {
            ClearWallet();
        }

        foreach(IProduct product in products)
        {
           RegisterProduct(product);
        }
    }

    public override void RegisterProduct(IProduct product)
    {
        if(product == null)
        {
            Debug.LogError("Null product being registered to Wallet");
            return;
        }

        string type = product.GetProductType();
        int id = product.GetProductId();
        
        if(IsProductRegistered(product))
        {
            Debug.LogError("Trying to register duplicate products for ID " + id);
            return;
        }

        productAmounts[id] = new ProductAmount(product, 0);

        if(!productsByType.ContainsKey(type))
        {
            productsByType[type] = new List<IProduct>();
        }
        productsByType[type].Add(product);

        registeredProducts.Add(product);
    }

    public override bool IsProductRegistered(IProduct product)
    {
        if(product == null)
        {
            return false;
        }

        return registeredProducts.Contains(product);
    }

    public override bool ChangeAmount(IProduct product, int amount, bool canOverflow = false)
    {
        if(product == null)
        {
            Debug.LogError("Null product to change amount from Wallet");
            return false;
        }

        return ChangeAmount(product.GetProductId(), amount, canOverflow);
    }

    public override bool ChangeAmount(int productId, int amount, bool canOverflow = false)
    {
        return DeltaOperation(productId, amount, canOverflow);
    }

    private bool DeltaOperation(int productId, int delta, bool canOverflow)
    {
        if(!productAmounts.ContainsKey(productId))
        {
            Debug.LogError("Product not registered for id " + productId);
            return false;
        }

        ProductAmount stored = productAmounts[productId];

        bool canPerform = canOverflow || delta > 0 || delta >= stored.amount;
        
        if(canPerform)
        {
            stored.amount = (uint)(Mathf.Max(stored.amount + delta, 0));
//            productAmounts[productId] = stored;
        } 
        else if(!canOverflow)
        {
            Debug.LogError("Change to product + " + productId +" overflowed negatively");
        }
        return canPerform;
    }

    public override void ClearWalletAmounts()
    {
        int len = productAmounts.Keys.Count;
        Dictionary<int, ProductAmount>.KeyCollection keys = productAmounts.Keys;

        foreach(int key in keys)
        {
            productAmounts[key] = new ProductAmount(productAmounts[key].product, 0);
        }
    }
    
    public override IProduct[] GetProducts()
    {
        IProduct[] products = new IProduct[registeredProducts.Count];
        registeredProducts.CopyTo(products,0);
        return products;
    }

    public override IProduct[] GetProductsByType(string type)
    {
        if(productsByType.ContainsKey(type))
        {
            return productsByType[type].ToArray();
        }

        Debug.LogError ("Product Type"  + type + " does not exist");
        return null;
    }

    public override IProduct[] GetOwnedProducts()
    {
        return GetOwnedProducts(registeredProducts);
    }

    public override IProduct[] GetOwnedProductsByType(string type)
    {
        if(productsByType.ContainsKey(type))
        {
            return GetOwnedProducts(productsByType[type]);
        } 

        Debug.LogError ("Product Type"  + type + " does not exist");
        return null;
    }

    private IProduct[] GetOwnedProducts(IEnumerable<IProduct> productSet)
    {
            List<IProduct> owned = new List<IProduct>();

            foreach(IProduct product in productSet)
            {
                if(GetAmount(product) > 0)
                {
                    owned.Add(product);
                }
            }
            return owned.ToArray();

    }

    public override uint GetAmount(IProduct product)
    {
        if(product == null) 
        {
            Debug.LogError("Retrieving amount for null product");
            return 0;
        }

        return GetAmount(product.GetProductId());
    }

    public override uint GetAmount(int productId)
    {
        if(productAmounts.ContainsKey(productId))
        {
            return productAmounts[productId].amount;
        }
        
        Debug.LogError ("Product "  + productId + " not properly registered");
        return 0;
    }
}
}