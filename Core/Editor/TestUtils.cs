using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public static class TestUtils
{
    public static IEnumerator WaitTime(double seconds)
    {
        DateTime startTime = DateTime.UtcNow;
        double elapsed = 0;
        do
        {
            yield return null;
            elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
        }
        while (elapsed < seconds);
    }

}
}