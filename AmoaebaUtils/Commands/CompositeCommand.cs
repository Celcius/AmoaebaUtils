using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public abstract class CompositeCommand : PooledCommand
{
    protected Command [] commands;
    protected bool isExecuting = false;
    public bool IsExecuting => isExecuting;
    protected bool isUndoing = false;
    public bool IsUndoing => isUndoing;

    public CompositeCommand() {}

    public void SetCommands(Command[] commands)
    {
        if(!isExecuting && ! isUndoing)
        {
            this.commands = commands;
        }
    }

} 
}