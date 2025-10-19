using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace AmoaebaUtils
{

public class CompositeLinearCommand : CompositeCommand
{
    public CompositeLinearCommand() {}

    int executingIndex = -1;
    public override bool Execute(Action callback = null)
    {
        if(isUndoing || isExecuting)
        {
            return false;
        }

        executingIndex = 0;
        isExecuting = true;
        StartExecuteHelperChain(() => 
        {
            callback?.Invoke();
        });
        
        return true;
    }

    protected void StartExecuteHelperChain(Action callback = null)
    {
        Task.Run(() => { ExecuteHelper(callback); });
    }

    protected void StartUndoHelperChain(Action callback = null)
    {
        Task.Run(() => { UndoHelper(callback); });
    } 

    private void ExecuteHelper(Action callback = null)
    {
        executingIndex++;
        if(executingIndex < commands.Length)
        {
            Command currentCommand = commands[executingIndex];
            currentCommand.Execute(() => { ExecuteHelper(callback); });
        }
        else
        {
           isExecuting = false;
           callback?.Invoke();
        }
    }

    public override bool Undo(Action callback = null)
    {
        if(isUndoing || isExecuting)
        {
            return false;
        }

        isUndoing = true;
        StartUndoHelperChain(() => 
        {
            callback?.Invoke();
        });

        return true;
    }

    private void UndoHelper(Action callback = null)
    {
        if(executingIndex < 0 && executingIndex < commands.Length)
        {
            callback?.Invoke();
            isUndoing = false;
        }
        else
        {
            Command currentCommand = commands[executingIndex];
            --executingIndex;
            currentCommand.Undo(() => { UndoHelper(callback); });
        }
    }

}
}
