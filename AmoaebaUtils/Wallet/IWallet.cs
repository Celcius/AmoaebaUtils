using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{

[Serializable]
public struct ProductAmount
{
    public IProduct product;
    public uint amount;

    public ProductAmount(IProduct product, uint amount)
    {
        this.product = product;
        this.amount = amount;
    }
}

public abstract class IWallet
{
    public abstract void ClearWallet();

    public abstract void RegisterProducts(IProduct[] products, bool clearWallet);
    public abstract void RegisterProduct(IProduct product);
    public abstract bool IsProductRegistered(IProduct product);

    public abstract void ClearWalletAmounts();
    
    public abstract uint GetAmount(IProduct product);
    public abstract uint GetAmount(int productId);
    public abstract bool ChangeAmount(IProduct product, int amount, bool canOverflow = true);
    public abstract bool ChangeAmount(int productId, int amount, bool canOverflow = true);

    public abstract IProduct[] GetProducts();
    public abstract IProduct[] GetOwnedProducts();
    public abstract IProduct[] GetProductsByType(string type);
    public abstract IProduct[] GetOwnedProductsByType(string type);

}
}