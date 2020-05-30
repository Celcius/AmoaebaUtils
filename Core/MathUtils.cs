﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public static class MathUtils
{
    private static int SumBelow(int n)
    {
        int res = 0;
        for(int i = n; i > 0; i--)
        {
            res += i;
        }
        return res;
    }

    private static int Factorial(int n)
    {
        int res = 1;
        for(int i = n; i > 1; i--)
        {
            res *= i;
        }
        return res;
    }
    
    private static float ModulateIndex(float index, float total, bool center, bool alternate)
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