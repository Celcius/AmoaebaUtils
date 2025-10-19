using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public struct AStarSearchNode<T>
{
    public T node;
    public float pathCost;
    public float queueRank;
    public T parent;

    public AStarSearchNode(T node)
    {
        this.node = node;
        this.pathCost = 0;
        this.queueRank = float.MaxValue;
        this.parent = default(T);
    }

    public AStarSearchNode(T node, float cost, float rank, T parent)
    {
        this.node = node;
        this.pathCost = cost;
        this.queueRank = rank;
        this.parent = parent;    
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        AStarSearchNode<T> objNode = (AStarSearchNode<T>) obj;
        return node.Equals(objNode.node);
    }
    
    public override int GetHashCode()
    {
        return node.GetHashCode();
    }
}
}