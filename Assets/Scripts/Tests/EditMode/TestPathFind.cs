using System;
using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

public class TestPathFind
{
    public List<int[]> path;
    private double[,] grid;
    private int[] start;
    private int[] target;
    private PathFind pathFind;

    [SetUp]
    public void Setup()
    {
        grid = new double[20, 20];
        path = new List<int[]>();
        pathFind = new PathFind();
    }

    [Test]
    public void TestEdgePath(){
        start = new int[2]{0,0};
        target = new int[2]{0,19};
        List<int[]> expected = new List<int[]>();
        expected.Add(new int[]{0,0});
        expected.Add(new int[]{0,1});
        expected.Add(new int[]{0,2});
        expected.Add(new int[]{0,3});
        expected.Add(new int[]{0,4});
        expected.Add(new int[]{0,5});
        expected.Add(new int[]{0,6});
        expected.Add(new int[]{0,7});
        expected.Add(new int[]{0,8});
        expected.Add(new int[]{0,9});
        expected.Add(new int[]{0,10});
        expected.Add(new int[]{0,11});
        expected.Add(new int[]{0,12});
        expected.Add(new int[]{0,13});
        expected.Add(new int[]{0,14});
        expected.Add(new int[]{0,15});
        expected.Add(new int[]{0,16});
        expected.Add(new int[]{0,17});
        expected.Add(new int[]{0,18});
        expected.Add(new int[]{0,19});

        path = pathFind.Find(start, target, grid);
        for(int i = 0; i < path.Count; i++){
            Assert.AreEqual(expected[i], path[i]);
        }
        Assert.AreEqual(Math.Truncate(pathFind.GetCost()), 190);
    }


    [Test]
    public void TestPathWithObstacles(){
        start = new int[2]{0,0};
        target = new int[2]{19,19};
        FillGrid(4, 4, 10, grid);
        path = pathFind.Find(start, target, grid);
        Assert.AreEqual(Math.Truncate(pathFind.GetCost()), 366);
        PrintPath(path);
    }


    [Test]
    public void TestPathWithManyObstacles(){
        start = new int[2]{0,0};
        target = new int[2]{19,19};
        FillGrid(4, 4, 10, grid);
        FillGrid(16, 3, 19, grid);
        path = pathFind.Find(start, target, grid);
       // Assert.AreEqual(Math.Truncate(pathFind.GetCost()), 366);
        PrintBuildedPath(path);

        // Building path


    }

    private void PrintBuildedPath(List<int[]> path){
        double[,] clone = Util.CloneGrid(grid);
        for(int i = 0; i < path.Count; i++){
            clone[path[i][0], path[i][1]] = i;
        }

        Util.PrintGrid(clone);

    }

    private void PrintPath(List<int[]> arr){
        string s = "";

        foreach(int[] i in arr){
            s += " "+i[0]+","+i[1];
        }
        Debug.Log(s);
    }

    private void FillGrid(int row, int i, int j, double[,] grid){
        while(i <= j){
            grid[row, i++] = 1;
            grid[row, j--] = 1;
        }
    }
}