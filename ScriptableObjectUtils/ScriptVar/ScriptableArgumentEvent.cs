using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  AmoaebaUtils
{
public class ScriptableArgumentEvent<T> : ScriptableEvent
{

    public delegate void Event(T arg);

    public event Event OnEvent;

    public void Invoke(T arg)
    {
        OnEvent?.Invoke(arg);
    }
}
}
