using System.Collections.Generic;
using NUnit.Framework;

public class TestPathFind
{
    private int[] start;
    private int[] target;
    private int[,] grid;
    private PathFind pathFind;
    private List<Node> path;
    private List<Node> expected;

    [SetUp]
    public void Setup()
    {
        pathFind = new PathFind();
        expected = new List<Node>();
        path = new List<Node>();
    }

    [Test]
    public void TestEdgePath()
    {
        grid = new int[5, 5];
        start = new int[2] { 0, 0 };
        target = new int[2] { 0, 4 };
        expected.Clear();
        path.Clear();

        expected.Add(new Node(new int[] { 0, 0 }));
        expected.Add(new Node(new int[] { 0, 1 }));
        expected.Add(new Node(new int[] { 0, 2 }));
        expected.Add(new Node(new int[] { 0, 3 }));
        expected.Add(new Node(new int[] { 0, 4 }));

        for (int i = 0; i < path.Count; i++)
        {
            Assert.True(expected[i].Compare(path[i]));
        }
    }

    [Test]
    public void TestPathWithObstacles()
    {
        grid = new int[5, 5];
        start = new int[2] { 0, 0 };
        target = new int[2] { 4, 4 };
        FillGrid(2, 0, 3, grid);
        grid[3, 4] = 1;
        grid[1, 2] = 1;
        expected.Clear();
        path.Clear();

        expected.Add(new Node(new int[] { 0, 0 }));
        expected.Add(new Node(new int[] { 0, 1 }));
        expected.Add(new Node(new int[] { 0, 2 }));
        expected.Add(new Node(new int[] { 1, 3 }));
        expected.Add(new Node(new int[] { 2, 4 }));
        expected.Add(new Node(new int[] { 3, 3 }));
        expected.Add(new Node(new int[] { 4, 4 }));

        path = pathFind.Find(start, target, grid);
        for (int i = 0; i < path.Count; i++)
        {
            Assert.True(expected[i].Compare(path[i]));
        }
    }


    [Test]
    public void TestHardPathWithManyObstacles()
    {
        grid = new int[5, 5];
        start = new int[2] { 0, 0 };
        target = new int[2] { 4, 4 };
        FillGrid(3, 1, 4, grid);
        expected.Clear();
        path.Clear();

        expected.Add(new Node(new int[] { 0, 0 }));
        expected.Add(new Node(new int[] { 1, 0 }));
        expected.Add(new Node(new int[] { 2, 0 }));
        expected.Add(new Node(new int[] { 3, 0 }));
        expected.Add(new Node(new int[] { 4, 1 }));
        expected.Add(new Node(new int[] { 4, 2 }));
        expected.Add(new Node(new int[] { 4, 3 }));
        expected.Add(new Node(new int[] { 4, 4 }));

        path = pathFind.Find(start, target, grid);
        for (int i = 0; i < path.Count; i++)
        {
            Assert.True(expected[i].Compare(path[i]));
        }
    }

    [Test]
    public void TestInvalidPath()
    {
        grid = new int[5, 5];
        start = new int[2] { 0, -1 };
        target = new int[2] { 0, 14 };

        expected.Clear();
        path.Clear();
        path = pathFind.Find(start, target, grid);

        Assert.AreEqual(path, expected);
    }

    [Test]
    public void TestOneNodePath()
    {
        grid = new int[4, 4];
        start = new int[2] { 0, 1 };
        target = new int[2] { 2, 1 };
        expected.Clear();
        path.Clear();

        expected.Add(new Node(new int[] { 0, 1 }));
        expected.Add(new Node(new int[] { 1, 0 }));
        expected.Add(new Node(new int[] { 2, 1 }));

        grid[1, 1] = 1;

        Util.PrintGrid(grid);
        path = pathFind.Find(start, target, grid);
        Util.PrintPath(path);

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