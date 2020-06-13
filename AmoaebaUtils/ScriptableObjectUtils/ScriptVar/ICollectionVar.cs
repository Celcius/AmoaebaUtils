using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AmoaebaUtils
{
public interface ICollectionVar<T>
{
    void Add(T component);
    bool Remove(T component);
    void Clear();
    bool Contains(T component);
    void Sort(IComparer comparer);
}
}