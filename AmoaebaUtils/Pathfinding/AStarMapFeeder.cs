using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AmoaebaUtils
{
public interface AStarMapFeeder<T>
{
    T[] GetNeighbours(T pos);
    float GetMoveCost(T origin, T dest);
    
    float GetDistanceEstimation(T origin, T dest);

    bool SameNode(T node1, T node2);
}
}