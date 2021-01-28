using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public abstract class PooledCommandFactory<T> where T : PooledCommand, new()
{
    public PooledCommandFactory() {}
    public T MakeEmptyCommand(CommandPool pool)
    {
        T command = new T();
        command.SetCommandPool(pool);
        return command;
    }
}
}