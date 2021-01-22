using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace AmoaebaUtils
{
public class TestMap : AStarMapFeeder<Vector2Int>
{
    private HashSet<Vector2Int> invalidPositions = new HashSet<Vector2Int>();
    public Vector2Int[] InvalidPositions => new List<Vector2Int>(invalidPositions).ToArray();

    Vector2Int maxBounds = new Vector2Int(0,0);

    public TestMap() {}

    public TestMap(Vector2Int[] invalidPositions) 
    {
        foreach(Vector2Int pos in invalidPositions)
        {
            this.invalidPositions.Add(pos);
        }
    }

    public TestMap(string map, char invalidCharacter)
    {
        ParseString(map, invalidCharacter);
    }

    public bool IsValidPosition(Vector2Int pos)
    {
        return !invalidPositions.Contains(pos) && IsWithinBounds(pos);
    }
    public bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= 0 &&
               pos.y >= 0 &&
               pos.x < maxBounds.x &&
               pos.y < maxBounds.y;
    }

    public void ParseString(string map, char invalidCharacter)
    {
        invalidPositions.Clear();

        Vector2Int bounds = new Vector2Int(0,0);

        string[] rows = map.Split('\n');

        if(rows.Length > 0)
        {
            int len = rows[0].Length;
            bounds.x = Mathf.Max(bounds.x, len);

            for(int y = 0; y < rows.Length; y++)
            {
                string row = rows[y];
                Assert.IsTrue(len == row.Length, $"Unexpected of columns for row {y}. Expected {len}, got {row.Length}");
                for(int x = 0; x < row.Length; x++)
                {
                    if(row[x] == invalidCharacter)
                    {
                        invalidPositions.Add(new Vector2Int(x,y));
                    }
                }
            }
        }

        bounds.y = Mathf.Max(bounds.y, rows.Length);
        maxBounds = bounds;
    }

    public void ClearMap()
    {
        invalidPositions.Clear();
    }

    public void AddInvalidPosition(Vector2Int pos)
    {
        invalidPositions.Add(pos);
    }

    public void AddInvalidPositions(Vector2Int[] positions)
    {
        foreach(Vector2Int pos in positions)
        {
            this.invalidPositions.Add(pos);
        }
    }

    public Vector2Int[] GetNeighbours(Vector2Int pos)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if(Mathf.Abs(i) == Mathf.Abs(j))
                {
                    continue;
                }

                Vector2Int neighbour = pos + new Vector2Int(i,j);
                if(IsValidPosition(neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours.ToArray();
    }

    public float GetMoveCost(Vector2Int origin, Vector2Int dest)
    {
        float offset = Mathf.Abs(dest.x - origin.x) 
                       + Mathf.Abs(dest.y - origin.y);
        return offset <= 1? offset : float.MaxValue;
    }
    
    public float GetDistanceEstimation(Vector2Int origin, Vector2Int dest)
    {
        return Vector2Int.Distance(origin, dest);
    }

    public bool SameNode(Vector2Int node1, Vector2Int node2)
    {
        return node1 == node2;
    }
}
}