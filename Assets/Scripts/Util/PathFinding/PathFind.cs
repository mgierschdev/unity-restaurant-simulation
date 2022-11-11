using System.Collections.Generic;
using UnityEngine;

public class PathFind
{
    private const int COST_DIAGONAL = 14, COST_STRAIGHT = 10;
    private PathNode[,] grid;
    private int[,] arrayGrid;
    private bool found;
    private PathNode target, start;
    private List<int[]> path;

    //source to target in grid positions
    public List<Node> Find(int[] s, int[] t, int[,] sourceGrid)
    {
        // If it is out of bounds of if the target coord it is equal to the start coord
        if (s[0] < 0 || s[1] < 0 || t[0] < 0 || t[1] < 0 || s[0] >= sourceGrid.GetLength(0) || t[1] >= sourceGrid.GetLength(1) || (s[0] == t[0] && s[1] == t[1]))
        {
            return new List<Node>();
        }

        arrayGrid = Util.CloneGrid(sourceGrid);

        //to explore all directions
        int[,] directions = new int[,] { { 0, 1 }, { 1, 0 }, { -1, 0 }, { 0, -1 }/*, { -1, -1 }, { 1, -1 }, { -1, 1 }, { 1, 1 } */};

        // This should be a priority queue to achieve Log(n). 
        MinBinaryHeap openList = new MinBinaryHeap(sourceGrid.GetLength(0) * sourceGrid.GetLength(1));

        // init grid, G cost == int.MaxValue
        grid = new PathNode[arrayGrid.GetLength(0), arrayGrid.GetLength(1)];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = new PathNode(new int[] { i, j });
                PathNode n = grid[i, j];
                n.SetGCost(int.MaxValue);
                n.SetValue(arrayGrid[i, j]);
            }
        }

        start = grid[s[0], s[1]];
        target = grid[t[0], t[1]];

        PathNode root = grid[start.GetX(), start.GetY()];
        root.SetGCost(0);
        root.SetHCost(CalculateDistance(start, target));
        root.CalculateFCost();
        root.SetParent(null);
        openList.Add(root);

        // we start BFS
        while (!openList.IsEmpty())
        {
            PathNode current = openList.Poll();
            int[] currentPosition = current.GetPosition();

            // visited
            arrayGrid[currentPosition[0], currentPosition[1]] = 2;

            if (current.GetX() == target.GetX() && current.GetY() == target.GetY())
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
                Vector3Int currentEvaluatedPosition = new Vector3Int(x, y, 0);

                if (IsValid(x, y) && arrayGrid[x, y] != (int)ObjectType.OBSTACLE)//&& arrayGrid[x, y] != (int)ObjectType.PLAYER)
                {
                    //Additional validation to no go diagonally through 2 obstacles
                    // if (!IsValidDiagonal(x, y))
                    // {
                    //     continue;
                    // }
                    //Additional validation to no go diagonally through 2 obstacles

                    PathNode neighbor = grid[x, y];

                    int tentativeGCost = current.GetGCost() + CalculateDistance(current, neighbor);
                    if (tentativeGCost < neighbor.GetGCost())
                    {
                        neighbor.SetParent(current);
                        neighbor.SetGCost(tentativeGCost);
                        neighbor.SetHCost(CalculateDistance(neighbor, target));
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
        int down = x - 1 >= 0 ? arrayGrid[x - 1, y] : 1;
        int left = y - 1 >= 0 ? arrayGrid[x, y - 1] : 1;
        int up = x + 1 < arrayGrid.GetLength(0) ? arrayGrid[x + 1, y] : 1;
        int right = y + 1 < arrayGrid.GetLength(1) ? arrayGrid[x, y + 1] : 1;

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
        List<Node> path = new List<Node>();
        PathNode current = endNode;

        while (current.GetParent() != null)
        {
            path.Add(new Node(current.GetPosition()));
            current = current.GetParent();
        }

        // Adding the first node to the path
        path.Add(start.GetNode());
        path.Reverse();
        return path;
    }

    private bool IsValid(int x, int y)
    {
        return !(x < 0 || y < 0 || x >= grid.GetLength(0) || y >= grid.GetLength(1));
    }

    // Return the min distance without obstacles between 2 points
    public int CalculateDistance(PathNode a, PathNode b)
    {
        int xAbs = Mathf.Abs(a.GetX() - b.GetX());
        int yAbs = Mathf.Abs(a.GetY() - b.GetY());
        int diff = Mathf.Abs(xAbs - yAbs);
        return COST_DIAGONAL * Mathf.Min(xAbs, yAbs) + COST_STRAIGHT * diff;
    }
}