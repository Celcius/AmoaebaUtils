using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public abstract class ScriptableState : ScriptableObject
{
    [SerializeField]
    private ScriptableStateMachine machine;
    
    private CoroutineRunner runner;

    protected abstract void OnEnterState();
    protected abstract void OnLeaveState();

    public void WillEnterState(CoroutineRunner runner)
    {
        this.runner = runner;
        OnEnterState();
    }

    public void WillLeaveState()
    {
        runner.StopAllCoroutines();
        OnLeaveState();
    }
}
}