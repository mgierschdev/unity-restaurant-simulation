using System.Collections.Generic;
using NUnit.Framework;
using Util.Collections;
using Util.PathFinding;

namespace Tests.EditMode
{
    /**
     * Problem: Validate pathfinding behavior on grid inputs.
     * Goal: Ensure PathFind returns expected paths and edge cases.
     * Approach: Use NUnit tests with predefined grids and assertions.
     * Time: O(n) per path length plus pathfinding cost.
     * Space: O(n) for path storage.
     */
    public class TestPathFind
    {
        private int[] _start, _target;
        private int[,] _grid;
        private PathFind _pathFind;
        private List<Node> _path, _expected;

        [SetUp]
        public void Setup()
        {
            _pathFind = new PathFind();
            _expected = new List<Node>();
            _path = new List<Node>();
        }

        [Test]
        public void TestEdgePath()
        {
            _grid = new int[5, 5];
            _start = new[] { 0, 0 };
            _target = new[] { 0, 4 };
            _expected.Clear();
            _path.Clear();

            _expected.Add(new Node(new[] { 0, 0 }));
            _expected.Add(new Node(new[] { 0, 1 }));
            _expected.Add(new Node(new[] { 0, 2 }));
            _expected.Add(new Node(new[] { 0, 3 }));
            _expected.Add(new Node(new[] { 0, 4 }));

            for (int i = 0; i < _path.Count; i++)
            {
                Assert.True(_expected[i].Compare(_path[i]));
            }
        }

        [Test]
        public void TestPathWithObstacles()
        {
            _grid = new int[5, 5];
            _start = new[] { 0, 0 };
            _target = new[] { 4, 4 };
            FillGrid(2, 0, 3, _grid);
            _grid[3, 4] = 1;
            _grid[1, 2] = 1;
            _expected.Clear();
            _path.Clear();

            _expected.Add(new Node(new[] { 0, 0 }));
            _expected.Add(new Node(new[] { 0, 1 }));
            _expected.Add(new Node(new[] { 0, 2 }));
            _expected.Add(new Node(new[] { 1, 3 }));
            _expected.Add(new Node(new[] { 2, 4 }));
            _expected.Add(new Node(new[] { 3, 3 }));
            _expected.Add(new Node(new[] { 4, 4 }));

            _path = _pathFind.Find(_start, _target, _grid);
            for (int i = 0; i < _path.Count; i++)
            {
                Assert.True(_expected[i].Compare(_path[i]));
            }
        }


        [Test]
        public void TestHardPathWithManyObstacles()
        {
            _grid = new int[5, 5];
            _start = new[] { 0, 0 };
            _target = new[] { 4, 4 };
            FillGrid(3, 1, 4, _grid);
            _expected.Clear();
            _path.Clear();

            _expected.Add(new Node(new[] { 0, 0 }));
            _expected.Add(new Node(new[] { 1, 0 }));
            _expected.Add(new Node(new[] { 2, 0 }));
            _expected.Add(new Node(new[] { 3, 0 }));
            _expected.Add(new Node(new[] { 4, 0 }));
            _expected.Add(new Node(new[] { 4, 1 }));
            _expected.Add(new Node(new[] { 4, 2 }));
            _expected.Add(new Node(new[] { 4, 3 }));
            _expected.Add(new Node(new[] { 4, 4 }));

            Util.Util.PrintGrid(_grid);
            _path = _pathFind.Find(_start, _target, _grid);
            Util.Util.PrintPath(_path);

            for (int i = 0; i < _path.Count; i++)
            {
                Assert.True(_expected[i].Compare(_path[i]));
            }
        }

        [Test]
        public void TestInvalidPath()
        {
            _grid = new int[5, 5];
            _start = new[] { 0, -1 };
            _target = new[] { 0, 14 };

            _expected.Clear();
            _path.Clear();
            _path = _pathFind.Find(_start, _target, _grid);

            Assert.AreEqual(_path, _expected);
        }

        [Test]
        public void TestOneNodePath()
        {
            _grid = new int[4, 4];
            _start = new[] { 0, 1 };
            _target = new[] { 2, 1 };
            _expected.Clear();
            _path.Clear();

            _expected.Add(new Node(new[] { 0, 1 }));
            _expected.Add(new Node(new[] { 0, 2 }));
            _expected.Add(new Node(new[] { 1, 2 }));
            _expected.Add(new Node(new[] { 2, 2 }));
            _expected.Add(new Node(new[] { 2, 1 }));

            _grid[1, 1] = 1;

            Util.Util.PrintGrid(_grid);
            _path = _pathFind.Find(_start, _target, _grid);
            Util.Util.PrintPath(_path);

            for (int i = 0; i < _path.Count; i++)
            {
                Assert.True(_expected[i].Compare(_path[i]));
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
}
