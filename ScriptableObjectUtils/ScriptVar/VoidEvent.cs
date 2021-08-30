using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class VoidEvent : ScriptableEvent
{
    public delegate void Event();

    public event Event OnEvent;

    public void Invoke()
    {
        OnEvent?.Invoke();
    }
}
}
