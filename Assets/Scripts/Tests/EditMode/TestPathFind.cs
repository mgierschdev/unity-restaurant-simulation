using System;
using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

public class TestPathFind
{
    public List<int[]> path;
    private double[,] grid;
    int[] start;
    int[] target;
    PathFind pathFind;

    [SetUp]
    public void Setup()
    {
        grid = new double[20, 20];
        path = new List<int[]>();
        pathFind = new PathFind();
    }

    [Test]
    public void TestFindPath(){
        start = new int[2]{0,0};
        target = new int[2]{19,19};

        FillGrid(5, 0, 16, grid);
        FillGrid(15, 10, 19, grid);
        path = pathFind.Find(start, target, grid);

        Assert.NotNull(path);
    }

    private void FillGrid(int row, int i, int j, double[,] grid){
        while(i < j){
            grid[row, i++] = 1;
            grid[row, j--] = 1;
        }
    }
}