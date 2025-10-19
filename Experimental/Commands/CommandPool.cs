using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public class CommandPool
{
    
    Dictionary<System.Type, Queue<PooledCommand>> pool = new Dictionary<System.Type, Queue<PooledCommand>>();
    HashSet<PooledCommand> requested = new HashSet<PooledCommand>();

    public const int DefaultInitSize = 5;
    private int initsize = DefaultInitSize;

    public CommandPool()
    {
        this.initsize = DefaultInitSize;
    }

    public CommandPool(int initSize)
    {
        this.initsize = initSize;
    }

    public int RequestedCommandCount() 
    {
        return  requested.Count;
    }
    
    public int AvailableCommandCount()
    {
        int total = 0;
        foreach(System.Type t in pool.Keys)
        {
            total += pool[t].Count;
        }
        return total;
    }

    public int AvailableCommandCountOfType<T>()
    {
        System.Type type = typeof(T).GetType();
        if(pool.ContainsKey(type))
        {
            return pool[type].Count;
        }
        return 0;
    }
    public T GetFromPool<T, F>() where T : PooledCommand, new() where F : PooledCommandFactory<T>, new()
    {
        System.Type type = typeof(T).GetType();
        if(!pool.ContainsKey(type))
        {
            pool[type] = new Queue<PooledCommand>();
        }
        
        if(pool[type].Count <= 0)
        {
            FillPool<T>(type, initsize, new F());
        }

        T command = (T)pool[type].Dequeue();
        requested.Add(command);
        return command;
    }

    public void ReturnToPool(PooledCommand command)
    {
        requested.Remove(command);
        pool[command.GetType()].Enqueue(command);
    }   

    private void FillPool<T>(System.Type type, int size, PooledCommandFactory<T> factory) where T: PooledCommand, new()
    {
        for(int i = 0; i < size; i++)
        {
            pool[type].Enqueue(factory.MakeEmptyCommand(this));
        }

    }
}
}
