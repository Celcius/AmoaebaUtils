using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class CommandPoolScriptable : ScriptableObject
{

    private CommandPool commandPool = new CommandPool();
    public CommandPool Pool => commandPool;
}
}