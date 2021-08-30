using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace AmoaebaUtils 
{

[MessagePackObject]
public struct TestStruct
{
    [Key(0)]
    public int x;
    [Key(1)]
    public string a;

    public TestStruct(int x, string a)
    {
        this.x = x;
        this.a = a;
    }

    public bool Same(TestStruct y)
    {
        return y.x == x && a.CompareTo(y.a) == 0;
    }
}
}