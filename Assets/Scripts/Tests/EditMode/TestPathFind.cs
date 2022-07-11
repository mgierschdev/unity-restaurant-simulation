using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

public class TestPathFind
{
    private int[] start;
    private int[] target;
    private PathFind pathFind;

    [SetUp]
    public void Setup()
    {
        pathFind = new PathFind();
    }

    [Test]
    public void TestEdgePath()
    {
        int[,] grid = new int[5, 5];
        start = new int[2] { 0, 0 };
        target = new int[2] { 0, 4 };
        List<Node> expected = new List<Node>();
        expected.Add(new Node(new int[] { 0, 1 }));
        expected.Add(new Node(new int[] { 0, 2 }));
        expected.Add(new Node(new int[] { 0, 3 }));
        expected.Add(new Node(new int[] { 0, 4 }));

        List<Node> path = new List<Node>();
        for (int i = 0; i < path.Count; i++)
        {
            Assert.True(expected[i].Compare(path[i]));
        }
    }

    [Test]
    public void TestPathWithObstacles()
    {
        int[,] grid = new int[5, 5];
        start = new int[2] { 0, 0 };
        target = new int[2] { 4, 4 };
        FillGrid(2, 0, 3, grid);
        grid[3, 4] = 1;
        grid[1, 2] = 1;
        List<Node> expected = new List<Node>();
        expected.Add(new Node(new int[] { 0, 1 }));
        expected.Add(new Node(new int[] { 0, 2 }));
        expected.Add(new Node(new int[] { 1, 3 }));
        expected.Add(new Node(new int[] { 2, 4 }));
        expected.Add(new Node(new int[] { 3, 3 }));
        expected.Add(new Node(new int[] { 4, 4 }));

        List<Node> path = new List<Node>();
        path = pathFind.Find(start, target, grid);
        for (int i = 0; i < path.Count; i++)
        {
            Assert.True(expected[i].Compare(path[i]));
        }
    }


    [Test]
    public void TestHardPathWithManyObstacles()
    {
        int[,] grid = new int[5, 5];
        start = new int[2] { 0, 0 };
        target = new int[2] { 4, 4 };
        FillGrid(3, 1, 4, grid);

        List<Node> expected = new List<Node>();
        expected.Add(new Node(new int[] { 1, 0 }));
        expected.Add(new Node(new int[] { 2, 0 }));
        expected.Add(new Node(new int[] { 3, 0 }));
        expected.Add(new Node(new int[] { 4, 1 }));
        expected.Add(new Node(new int[] { 4, 2 }));
        expected.Add(new Node(new int[] { 4, 3 }));
        expected.Add(new Node(new int[] { 4, 4 }));

        List<Node> path = new List<Node>();
        path = pathFind.Find(start, target, grid);
        for (int i = 0; i < path.Count; i++)
        {
            Assert.True(expected[i].Compare(path[i]));
        }
    }

    private void FillGrid(int row, int i, int j, int[,] grid)
    {
        while (i <= j)
        {
            grid[row, i++] = 1;
            grid[row, j--] = 1;
        }
    }
}