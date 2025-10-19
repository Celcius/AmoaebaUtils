using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{

public class CompositeParallelCommand : CompositeCommand
{
    int executedCommands;

    public CompositeParallelCommand() {}
    
    public override bool Execute(Action callback = null)
    {
        Action<Command, Action> toPerform = (Command command, Action endCallback) =>
        {
            Task.Run(() => 
            { 
                command.Execute(() => 
                {
                    ++executedCommands;
                    CheckExecuted(endCallback);
                });
            });
        };

        isExecuting = CommandHelper(toPerform, callback);
        
        return isExecuting;
    }

    public override bool Undo(Action callback = null)
    {
        Action<Command, Action> toPerform = (Command command, Action endCallback) =>
        {
            Task.Run(() => 
            { 
                command.Undo(() => 
                {
                    ++executedCommands;
                    CheckExecuted(endCallback);
                });
            });
        };

        isExecuting = CommandHelper(toPerform, callback);
        
        return isExecuting;
    }

    private bool CommandHelper(Action<Command, Action> toPerform, Action callback)
    {
        if(isUndoing || isExecuting || toPerform == null)
        {
            return false;
        }

        executedCommands = 0;

        for(int i = 0; i < commands.Length; i++)
        {
            toPerform?.Invoke(commands[i], callback);
        }
        
        return true;
    }
    private void CheckExecuted(Action callback)
    {
        if(executedCommands >= commands.Length)
        {
            isExecuting = false;
            isUndoing = false;
            callback?.Invoke();
        }
    }
}
}