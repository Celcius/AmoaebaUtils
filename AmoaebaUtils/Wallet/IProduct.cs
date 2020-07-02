using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public abstract class IProduct
{
    public abstract int GetProductId();
    public abstract string GetProductType();
}

public class IProductEqualityComparer : IEqualityComparer<IProduct>
{
    public bool Equals(IProduct b1, IProduct b2)
    {
        return b1.GetProductId() == b2.GetProductId();
    }

    public int GetHashCode(IProduct product)
    {
        return product.GetProductId();
    }
}
}