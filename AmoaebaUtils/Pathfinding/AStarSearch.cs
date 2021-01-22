using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Threading.Tasks;

// based on http://theory.stanford.edu/~amitp/GameProgramming/ImplementationNotes.html#sketch

namespace AmoaebaUtils
{
public class AStarSearch<T>
{
    private List<AStarSearchNode<T>> searchOpenNodes = new List<AStarSearchNode<T>>();
    private List<AStarSearchNode<T>> searchClosedNodes = new List<AStarSearchNode<T>>();

    private HashSet<T> openNodes = new HashSet<T>();
    private HashSet<T> closedNodes = new HashSet<T>();
    private Dictionary<T, AStarSearchNode<T>> searchNodes = new Dictionary<T, AStarSearchNode<T>>();

    public List<T> visitedNodes = new List<T>();
    public T[] VisitedNodes => visitedNodes.ToArray();

    public T[] path = new T[0];
    public T[] Path => path;

    public async void PerformSearchAsync(T origin,
                                         T destination,
                                         AStarMapFeeder<T> feeder,
                                         Action<T[]> onFinishCallbak)
    {
        Task taskA = Task.Run(() => { PerformSearch(origin, destination, feeder); });

        await taskA;

        onFinishCallbak?.Invoke(path);
    }

    public T[] PerformSearch(T origin, 
                             T destination, 
                             AStarMapFeeder<T> feeder)
    {

        ClearSearch();
        AddOpenNode(origin);

        while(searchOpenNodes.Count > 0 && !feeder.SameNode(searchOpenNodes[0].node, destination))
        {
            AStarSearchNode<T> current = searchOpenNodes[0];
            RemoveOpenNode(current);
            AddClosedNode(current);
            
            visitedNodes.Add(current.node);

            T[] neighbours = feeder.GetNeighbours(current.node);
            foreach(T neighbour in neighbours)
            {   
                float cost = current.pathCost + feeder.GetMoveCost(current.node,  neighbour);

                AStarSearchNode<T> neighbourSearch = searchNodes.ContainsKey(neighbour)? 
                                                     searchNodes[neighbour] : 
                                                     new AStarSearchNode<T>(neighbour, 
                                                                            cost, 
                                                                            cost + feeder.GetDistanceEstimation(neighbour, destination),
                                                                            current.node);
                bool inOpen = openNodes.Contains(neighbour);
                bool inClosed = closedNodes.Contains(neighbour);
                bool smallerCost = cost < neighbourSearch.pathCost;

                if(inOpen && smallerCost)
                {
                    RemoveOpenNode(neighbourSearch);
                    
                } 
                else if(inClosed && smallerCost)
                {

                    RemoveClosedNode(neighbourSearch);
                }
                else if(!inOpen && !inClosed)
                {
                    AddOpenNode(neighbourSearch);
                }
            }
        }

        if(searchOpenNodes.Count > 0)
        {
            path = GetParentPathFrom(searchOpenNodes[0], origin, feeder);
        }

        return path;
    }

    public void ClearSearch()
    {
        searchOpenNodes.Clear();
        searchClosedNodes.Clear();
        openNodes.Clear();
        closedNodes.Clear();
        searchNodes.Clear();
        visitedNodes.Clear();
        path = new T[0];
    }

    private T[] GetParentPathFrom(AStarSearchNode<T> searchNode, T origin, AStarMapFeeder<T> feeder)
    {
        List<T> parentList = new List<T>();
        do
        {
            parentList.Add(searchNode.node);

            if(searchNodes.ContainsKey(searchNode.parent))
            {
                searchNode = searchNodes[searchNode.parent];
            }
            
    
        }
        while(!feeder.SameNode(searchNode.node, origin) 
              && !feeder.SameNode(searchNode.node, searchNode.parent));

        if(feeder.SameNode(searchNode.node, origin))
        {
            parentList.Add(searchNode.node);
        }

        parentList.Reverse();
        return parentList.ToArray();
    }
    
    private void AddOpenNode(T node)
    {
        AStarSearchNode<T> searchNode = new AStarSearchNode<T>(node);
        AddOpenNode(searchNode);
    }

    private void AddOpenNode(AStarSearchNode<T> searchNode)
    {
        searchOpenNodes.Add(searchNode);
        searchNodes.Add(searchNode.node, searchNode);
        openNodes.Add(searchNode.node);
        
        
        searchOpenNodes.Sort((AStarSearchNode<T> searchNode1, 
                              AStarSearchNode<T> searchNode2) =>
        {
            return searchNode1.queueRank.CompareTo(searchNode2.queueRank);
        });
    }

    private void RemoveOpenNode(AStarSearchNode<T> searchNode)
    {
        searchOpenNodes.Remove(searchNode);
        openNodes.Remove(searchNode.node);
    }

    private void AddClosedNode(AStarSearchNode<T> searchNode)
    {
        searchClosedNodes.Add(searchNode);
        closedNodes.Add(searchNode.node);
    }

    private void RemoveClosedNode(AStarSearchNode<T> searchNode)
    {
        searchClosedNodes.Remove(searchNode);
        closedNodes.Remove(searchNode.node);
    }
}
}