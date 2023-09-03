using System.Collections.Generic;
using UnityEngine;
using Util.Collections;

namespace Util.PathFinding
{
    public class PathFind
    {
        private const int CostDiagonal = 14, CostStraight = 10;
        private PathNode[,] _grid;
        private int[,] _arrayGrid;
        private bool _found;
        private PathNode _target, _start;
        private List<int[]> _path;

        //source to target in grid positions
        public List<Node> Find(int[] s, int[] t, int[,] sourceGrid)
        {
            // If it is out of bounds of if the target coord it is equal to the start coord
            if (s[0] < 0 || s[1] < 0 || t[0] < 0 || t[1] < 0 || s[0] >= sourceGrid.GetLength(0) || t[1] >= sourceGrid.GetLength(1) || (s[0] == t[0] && s[1] == t[1]))
            {
                return new List<Node>();
            }

            _arrayGrid = Util.CloneGrid(sourceGrid);

            //to explore all directions
            var directions = new[,] { { 0, 1 }, { 1, 0 }, { -1, 0 }, { 0, -1 }, { -1, -1 }, { 1, -1 }, { -1, 1 }, { 1, 1 }};

            MinBinaryHeap openList = new MinBinaryHeap(sourceGrid.GetLength(0) * sourceGrid.GetLength(1));

            // init grid, G cost == int.MaxValue
            _grid = new PathNode[_arrayGrid.GetLength(0), _arrayGrid.GetLength(1)];
            for (var i = 0; i < _grid.GetLength(0); i++)
            {
                for (var j = 0; j < _grid.GetLength(1); j++)
                {
                    _grid[i, j] = new PathNode(new[] { i, j });
                    var n = _grid[i, j];
                    n.SetGCost(int.MaxValue);
                    n.SetValue(_arrayGrid[i, j]);
                }
            }

            _start = _grid[s[0], s[1]];
            _target = _grid[t[0], t[1]];

            var root = _grid[_start.GetX(), _start.GetY()];
            root.SetGCost(0);
            root.SetHCost(CalculateDistance(_start, _target));
            root.CalculateFCost();
            root.SetParent(null);
            openList.Add(root);

            // we start BFS
            while (!openList.IsEmpty())
            {
                PathNode current = openList.Poll();
                int[] currentPosition = current.GetPosition();

                // visited
                _arrayGrid[currentPosition[0], currentPosition[1]] = 2;

                if (current.GetX() == _target.GetX() && current.GetY() == _target.GetY())
                {
                    // build path
                    return BuildPath(current);
                }

                // Go in all directions
                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    int x = currentPosition[0] + directions[i, 0];
                    int y = currentPosition[1] + directions[i, 1];
                    // We add to the queue is valid to explore
                
                    //Checking for player positions
                    //Vector3Int currentEvaluatedPosition = new Vector3Int(x, y, 0);

                    if (IsValid(x, y) && _arrayGrid[x, y] != (int)ObjectType.Obstacle)//&& arrayGrid[x, y] != (int)ObjectType.PLAYER)
                    {
                        //Additional validation to no go diagonally through 2 obstacles
                        // if (!IsValidDiagonal(x, y))
                        // {
                        //     continue;
                        // }
                        //Additional validation to no go diagonally through 2 obstacles

                        PathNode neighbor = _grid[x, y];

                        int tentativeGCost = current.GetGCost() + CalculateDistance(current, neighbor);
                        if (tentativeGCost < neighbor.GetGCost())
                        {
                            neighbor.SetParent(current);
                            neighbor.SetGCost(tentativeGCost);
                            neighbor.SetHCost(CalculateDistance(neighbor, _target));
                            neighbor.CalculateFCost();

                            if (!openList.Contains(neighbor))
                            {
                                openList.Add(neighbor);
                            }
                        }
                    }
                }
            }
            return new List<Node>();
        }

        private bool IsValidDiagonal(int x, int y)
        {
            var down = x - 1 >= 0 ? _arrayGrid[x - 1, y] : 1;
            var left = y - 1 >= 0 ? _arrayGrid[x, y - 1] : 1;
            var up = x + 1 < _arrayGrid.GetLength(0) ? _arrayGrid[x + 1, y] : 1;
            var right = y + 1 < _arrayGrid.GetLength(1) ? _arrayGrid[x, y + 1] : 1;

            //check right/down
            if (right == 1 && down == 1)
            {
                return false;
            }

            //check up / left
            if (up == 1 && left == 1)
            {
                return false;
            }

            //check down/left
            if (down == 1 && left == 1)
            {
                return false;
            }

            //check up/right
            if (up == 1 && right == 1)
            {
                return false;
            }

            return true;
        }

        private List<Node> BuildPath(PathNode endNode)
        {
            var finalPath = new List<Node>();
            var current = endNode;

            while (current.GetParent() != null)
            {
                finalPath.Add(new Node(current.GetPosition()));
                current = current.GetParent();
            }

            // Adding the first node to the path
            finalPath.Add(_start.GetNode());
            finalPath.Reverse();
            return finalPath;
        }

        private bool IsValid(int x, int y)
        {
            return !(x < 0 || y < 0 || x >= _grid.GetLength(0) || y >= _grid.GetLength(1));
        }

        // Return the min distance without obstacles between 2 points
        private static int CalculateDistance(PathNode a, PathNode b)
        {
            var xAbs = Mathf.Abs(a.GetX() - b.GetX());
            var yAbs = Mathf.Abs(a.GetY() - b.GetY());
            var diff = Mathf.Abs(xAbs - yAbs);
            return CostDiagonal * Mathf.Min(xAbs, yAbs) + CostStraight * diff;
        }
    }
}