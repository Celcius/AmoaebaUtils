using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public interface Command 
{
    bool Execute(Action callback = null);
    bool Undo(Action callback = null);
}
}