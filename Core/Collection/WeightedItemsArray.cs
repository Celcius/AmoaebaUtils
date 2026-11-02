using UnityEngine;
using System;

namespace AmoaebaUtils
{

[Serializable]
public struct WeightedItem<T>
{
    public T Item;
    public float Weight;
}

public class WeightedItemsArray<T>
{
    [SerializeField]
    private WeightedItem<T>[] Items;
    
    public T GetRandomWeightedItem()
    {
        if (Items == null || Items.Length == 0)
        {
            throw new InvalidOperationException("No items available.");
        }

        // Calculate total weight
        float totalWeight = 0f;

        foreach (var item in Items)
        {
            if(item.Weight < 0)
            {
                throw new InvalidOperationException("Item has negative Weight");
            }

            totalWeight += Mathf.Max(0f, item.Weight);
        }

        if (totalWeight <= 0f)
        {
            throw new InvalidOperationException("Total weight must be greater than zero.");
        }

        // Generate random value within total weight
        float randomValue = UnityEngine.Random.value * totalWeight;

        // Find selected item
        foreach (var item in Items)
        {
            randomValue -= Mathf.Max(0f, item.Weight);

            if (randomValue <= 0f)
            {
                return item.Item;
            }
        }

        return Items[Items.Length - 1].Item;
    }
}
    
}