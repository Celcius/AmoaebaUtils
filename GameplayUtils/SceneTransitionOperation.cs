using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AmoaebaUtils
{
public abstract class SceneTransitionOperation : ScriptableObject
{
    [SerializeField]
    private bool runOnce = false;

    [NonSerialized] 
    private bool hasRun = false;

    protected abstract IEnumerator RunOperation();

    public IEnumerator Run()
    {
        if(runOnce && hasRun)
        {
            yield break;
        }

        yield return RunOperation();
        hasRun = true;
    }
}
}