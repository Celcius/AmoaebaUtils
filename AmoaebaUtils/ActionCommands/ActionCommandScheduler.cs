﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class ActionCommandScheduler
{
    public delegate void OnCommandWillStart(ActionCommandObject command);
    public delegate void OnCommandDidFinish(ActionCommandObject command);

    public delegate void OnCountChange();

    public event OnCommandWillStart OnCommandWillStartEvent;
    public event OnCommandDidFinish OnCommandDidFinishEvent;
    public event OnCountChange OnCountChangeEvent;


    public class ActionCommandComparer : IComparer<ActionCommandObject>
    {
        public int Compare(ActionCommandObject x, ActionCommandObject y)
        {
            return (int) Mathf.Sign(x.GetWeight() - y.GetWeight());
        }
    }

    private List<ActionCommandObject> commands = new List<ActionCommandObject>();
    private IComparer<ActionCommandObject> comparer;


    public ActionCommandObject[] GetCommands()
    {
        return commands.ToArray();
    }

    public int GetCommandCount()
    {
        return commands.Count;
    }
    public ActionCommandScheduler() : this(new ActionCommandComparer()) {}
    
    public ActionCommandScheduler(IComparer<ActionCommandObject> comparer)
    {
        comparer = comparer;
    }

    public void PerformAllActions()
    {
        lock (commands)
        {
            while(commands.Count > 0)
            {
                ActionCommandObject command = commands[0];
                OnCommandWillStartEvent?.Invoke(command);
                command.PerformAction();
                
                commands = commands.GetRange(1, commands.Count-1);
                Sort();
                OnCommandDidFinishEvent?.Invoke(command);
                OnCountChangeEvent?.Invoke();
            }
        }
    }

    private void Sort()
    {
        commands.Sort(comparer);
    }

    public void AddActionCommand(ActionCommandObject command)
    {
        lock (commands)
        {
            commands.Add(command);
            Sort();
        }
        OnCountChangeEvent?.Invoke();
    }

    public void ClearActions()
    {
        commands.Clear();
        OnCountChangeEvent?.Invoke();
    }
}
}