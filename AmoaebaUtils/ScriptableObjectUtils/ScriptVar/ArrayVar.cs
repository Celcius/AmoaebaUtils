using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class ArrayVar<T> : ScriptVar<T[]>, ICollectionVar<T>
{
    public const int INVALID_INDEX = -1;

    public virtual void Add(T component)
    {
        T[] oldValue = value;
        if(value == null || value.Length == 0)
        {
            value = new T[1]{component};
        }
        else 
        {
            T[] newArr = new T[value.Length + 1];
            value.CopyTo(newArr, 0);
            newArr[value.Length] = (T)component;
            value = newArr;
        }

        InvokeChangeEvent(oldValue, value);
    }

    public virtual bool Remove(T component)
    {
        int index = GetIndexFor(component);

        if (index == INVALID_INDEX)
        {
            return false;
        }

        T[] oldValue = value;
        T[] dest = new T[value.Length - 1];

        if (index > 0)
        {
            Array.Copy(value, 0, dest, 0, index);
        }

        if (index < value.Length - 1)
        {
            Array.Copy(value, index + 1, dest, index, value.Length - index - 1);
        }
        value = dest;

        InvokeChangeEvent(oldValue, value);
        return true;
    }

    public virtual void Clear()
    {
        T[] oldValue = value;
        value = new T[0];

        InvokeChangeEvent(oldValue, value);
    }

    public int GetIndexFor(T component)
    {
        if (this.value == null || this.value.Length == 0)
        {
            return INVALID_INDEX;
        }

        for (int i = 0; i < value.Length; i++)
        {
            T t = value[i];

            if (System.Object.Equals(t, component))
            {
                return i;
            }
        }

        return INVALID_INDEX;
    }

    public virtual bool Contains(T component)
    {
        return GetIndexFor(component) != INVALID_INDEX;
    }

    public virtual void Sort(IComparer comparer)
    {
        Array.Sort(value, comparer);
    }

    public int Count()
    {
        return value == null? 0 : value.Length;
    }
}
}