using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace AmoaebaUtils
{
public class AStarSearchTestSuite
{
    private static TestMap testMap;
    private static AStarSearch<Vector2Int> search;

    [SetUp]
    public void Setup()
    {
        testMap = new TestMap();
        search = new AStarSearch<Vector2Int>();
    }
    

    [TearDown]
    public void Teardown()
    {
        testMap.ClearMap();
        search.ClearSearch();
    }

    private static bool OneValidPath(Vector2Int[] path, Vector2Int[][] checkPaths)
    {
        foreach(Vector2Int[] toCheck in checkPaths)
        {
            if(SamePath(path,toCheck))
            {
                return true;
            }
        }
        return false;
    }
    private static bool SamePath(Vector2Int[] path1, Vector2Int[] path2)
    {
        if(path1 == null || path2 == null)
        {
            return path1 == path2;
        }

        if(path1.Length != path2.Length)
        {
            return false;
        }

        for(int i = 0; i < path1.Length; i++)
        {
            if(path1[i] != path2[i])
            {
                return false;
            }
        }

        return true;
    }

    private string PathToString(Vector2Int[] path)
    {
        string ret = "\nSTART| ";
        for(int i = 0; i < path.Length; i++)
        {
            ret += path[i];
            ret += (i < path.Length-1)? "-> " : " ";
        }

        ret = "|END\n";
        return ret;
    }
    [UnityTest]
    public IEnumerator TestEmptyMap()
    {
        Assert.IsTrue(testMap != null, "Map was Null");
        Assert.IsTrue(testMap.InvalidPositions.Length == 0, "Expected empty Map");
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestParseStringMap()
    {
        Assert.IsTrue(testMap != null, "Map was Null");
        Assert.IsTrue(testMap.InvalidPositions.Length == 0, "Expected empty Map");

        testMap.ParseString("ooo\n000\n888",'x');
        
        Assert.IsTrue(testMap.InvalidPositions.Length == 0, "Expected empty Map");

        testMap.ParseString("xoo\n00x\n888",'x');

        Assert.IsTrue(testMap.InvalidPositions.Length == 2, "Expected map with 2 invalidPositions");
        Assert.IsFalse(testMap.IsValidPosition(new Vector2Int(0,0)), "Expected (0,0) to be invalid");
        Assert.IsFalse(testMap.IsValidPosition(new Vector2Int(2,1)), "Expected (2,1) to be invalid");
        Assert.IsTrue(testMap.IsValidPosition(new Vector2Int(1,2)), "Expected (1,2) to be valid");

        testMap.ClearMap();

        Assert.IsTrue(testMap.InvalidPositions.Length == 0, "Expected empty Map");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestEmptySearch()
    {
        
        Assert.IsTrue(search != null, "Search was Null");
        Assert.IsTrue(search.VisitedNodes.Length == 0, "Expected empty Search");
        Assert.IsTrue(search.Path.Length == 0, "Expected empty Search");
        yield return null;
    }

    [UnityTest, Timeout(5)]
    public IEnumerator TestUnblockedPathAdjacentSearch()
    {
        testMap.ParseString("ooo\n000\n888",'x');
        
        Vector2Int[][] expected = new Vector2Int[][]
            {
                new Vector2Int[]{new Vector2Int(0,0), new Vector2Int(1,0)}
            };

        Vector2Int[] path = search.PerformSearch(new Vector2Int(0,0), 
                                                 new Vector2Int(1,0),
                                                 testMap);

        Assert.IsTrue(path.Length == 2, "Expected 2 point path, got " + path.Length);
        Assert.IsTrue(SamePath(path, expected[0]), "Unexpected path");

        yield return null;
    }

    [UnityTest, Timeout(5)]
    public IEnumerator TestUnblockedPathDistantSearch()
    {
        testMap.ParseString("ooo\n000\n888",'x');

        Vector2Int[][] expected = new Vector2Int[][]{
            new Vector2Int[]{new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0), new Vector2Int(2,1)},
            new Vector2Int[]{new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(2,1)},
            new Vector2Int[]{new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,1), new Vector2Int(2,1)}};

        Vector2Int[] path  = search.PerformSearch(new Vector2Int(0,0), 
                                    new Vector2Int(2,1),
                                    testMap);

        bool foundValid = OneValidPath(path, expected);

        Assert.IsTrue(path.Length == 4, "Expected 4 point path, got " + path.Length);
        Assert.IsTrue(foundValid, "Did not find expected Path, got " + PathToString(path));
                                                
        yield return null;
    }

    [UnityTest, Timeout(5)]
    public IEnumerator TestImpossiblePathDistantSearch()
    {
        testMap.ParseString("oxo\n0x0\n8x8",'x');

        Vector2Int[] expected = new Vector2Int[0];

        Vector2Int[] path  = search.PerformSearch(new Vector2Int(0,0), 
                                    new Vector2Int(2,1),
                                    testMap);

        bool foundValid = SamePath(path, expected);

        Assert.IsTrue(foundValid, "Did not find expected empty Path, got " + PathToString(path));
                                                
        yield return null;
    }

    [UnityTest, Timeout(5)]
    public IEnumerator TestBlockedPathDistantSearch()
    {
        testMap.ParseString("oxo\n0x0\n888",'x');

        Vector2Int[] expected = new Vector2Int[]{new Vector2Int(0,0), 
                                                 new Vector2Int(0,1), 
                                                 new Vector2Int(0,2), 
                                                 new Vector2Int(1,2), 
                                                 new Vector2Int(2,2), 
                                                 new Vector2Int(2,1)};

        Vector2Int[] path  = search.PerformSearch(new Vector2Int(0,0), 
                                    new Vector2Int(2,1),
                                    testMap);

        bool foundValid = SamePath(path, expected);

        Assert.IsTrue(foundValid, "Did not find expected empty Path, got " + PathToString(path));
                                                
        yield return null;
    }

    [UnityTest, Timeout(10)]
    public IEnumerator TestAsyncSearch()
    {
        int height = 1000;
        string mapStr = "oxo\n";
        for(int i = 0; i < height-2; i++)
        {
            mapStr += "0x0\n";
        }
        mapStr += "888";

        testMap.ParseString(mapStr,'x');

        Vector2Int[] expected = new Vector2Int[height*2+1];
        for(int i = 0; i < height; i++)
        {
            expected[i] = new Vector2Int(0,i);
        }
        
        expected[height] = new Vector2Int(1, height-1);

        for(int i = 0; i < height; i++)
        {
            expected[height + 1 + i] = new Vector2Int(2,height - i -1);
        }

        bool completed = false;
        int counter = 0;
        Vector2Int[] path = new Vector2Int[0];
        search.PerformSearchAsync(new Vector2Int(0,0), 
                                    new Vector2Int(2,0),
                                    testMap,
                                    (Vector2Int[] foundPath) => 
                                    {
                                        path = foundPath;
                                        completed = true;
                                    });

        if(!completed)
        {
            Assert.IsTrue(path.Length == 0, "Expected empty path, got " + path.Length);
        }
        while(!completed)
        {
            counter++;
            yield return null;
        }

        bool foundValid = SamePath(path, expected);
        Assert.IsTrue(foundValid, "Did not find expected Path + " + PathToString(expected) + ", got " + PathToString(path));
        Assert.IsTrue(counter > 0, "Finished too fast");

        yield return null;
    }

    [UnityTest, Timeout(10)]
    public IEnumerator TestLargeAsyncSearch()
    {
        int height = 2000;
        string mapStr = "ooxoo\n";
        for(int i = 0; i < height-2; i++)
        {
            mapStr += "00x00\n";
        }
        mapStr += "8o8o8";

        testMap.ParseString(mapStr,'x');

        bool completed = false;
        int counter = 0;
        Vector2Int[] path = new Vector2Int[0];
        search.PerformSearchAsync(new Vector2Int(0,0), 
                                    new Vector2Int(4,0),
                                    testMap,
                                    (Vector2Int[] foundPath) => 
                                    {
                                        path = foundPath;
                                        completed = true;
                                    });

        if(!completed)
        {
            Assert.IsTrue(path.Length == 0, "Expected empty path, got " + path.Length);
        }
        while(!completed)
        {
            counter++;
            yield return null;
        }


        int expectedLen =  height *2 +3;
        Assert.IsTrue(path.Length == expectedLen, $"Expected path with {expectedLen} nodes but got {path.Length}." + PathToString(path));
        Assert.IsTrue(counter > 0, "Finished too fast");
        yield return null;
    }
}
}