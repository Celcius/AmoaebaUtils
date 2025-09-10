using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class IndexableStack<T> : List<T>
{

    public void Enqueue(T value)
    {
        Add(value);
    }

    public void Dequeue()
    {
        if(Count == 0)
        {
            return;
        }
        RemoveAt(Count-1);
    }

    public T Peek()
    {
        if(Count == 0)
        {
            throw new System.Exception("Peeking at empty queue");
        }

        return this[Count-1];
    }
}
}