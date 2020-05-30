using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public static class StructUtils
{
    public static float ModulateIndex(float index, float total, bool center, bool alternate)
    {
        int sign = alternate? index % 2 == 0? -1 : 1 : 1;
        float modulatedIndex = alternate? ((int) Mathf.Ceil(index/2.0f)) : index;

        if(center)
        {
            modulatedIndex -= alternate? sign *(total % 2 == 0? 0.5f : 0) :
                                total/2.0f - 0.5f;
        }
        return sign * modulatedIndex;
    }
}
}
