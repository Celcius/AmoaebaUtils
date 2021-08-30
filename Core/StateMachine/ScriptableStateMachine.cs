using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class ScriptableStateMachine : ScriptableObject
{
    private ScriptableState currentState = null;

    private CoroutineRunner runner = null;

    public void SetState(ScriptableState state)
    {
        if(currentState != null)
        {
            currentState.WillLeaveState();
        }
        
        currentState = state;
        if(currentState != null)
        {
            if(runner == null)
            {
                runner = CoroutineRunner.Instantiate(this.name);
                DontDestroyOnLoad(runner);
            }

            currentState.WillEnterState(runner);
        }
    }

    public void StopMachine()
    {
        SetState(null);
    }

    public bool IsRunning()
    {
        return currentState != null;
    }

    public ScriptableState GetCurrentState()
    {
        return currentState;
    }
}
}