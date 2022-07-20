using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This will contain Utility functions, to create Unity Object and other
public static class Util
{
    private const int sortingLevel = Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL; // Background 

    // Creates a Text object in the scene
    public static TextMesh CreateTextObject(string name, GameObject parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment)
    {
        GameObject gameObject = new GameObject(name, typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.localPosition = localPosition;
        transform.SetParent(parent.transform, true);
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingLevel;
        return textMesh;
    }

    public static Vector3 GetMouseInWorldPosition()
    {
        Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vector.z = 0;
        return vector;
    }

    public static Vector2Int GetXYInGameMap(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt((position.x - Settings.GRID_START_X) * 1 / Settings.GRID_CELL_SIZE), Mathf.FloorToInt((position.y - Settings.GRID_START_Y) * 1 / Settings.GRID_CELL_SIZE));
    }

    public static void AddPath(List<Node> path, GameGridController gameGrid, Queue pendingMovementQueue)
    {
        pendingMovementQueue.Enqueue(gameGrid.GetCellPosition(path[0].GetVector3()));

        for (int i = 1; i < path.Count; i++)
        {
            Vector3 from = gameGrid.GetCellPosition(path[i - 1].GetVector3());
            Vector3 to = gameGrid.GetCellPosition(path[i].GetVector3());
            if (Settings.DEBUG_ENABLE)
            {
                Debug.DrawLine(from, to, Color.magenta, 10f);
            }
            pendingMovementQueue.Enqueue(gameGrid.GetCellPosition(path[i].GetVector3()));
        }

        if (path.Count == 0)
        {
            Debug.LogWarning("Path out of reach");
            return;
        }
    }

    public static void PrintGrid(int[,] grid)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            String row = "";
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                row += grid[i, j] + " ";
            }
            Debug.Log(row);
        }
        Debug.Log(" ");
    }

    public static double EuclidianDistance(int[] a, int[] b)
    {
        double distance = System.Math.Sqrt(System.Math.Pow(a[0] - b[0], 2) + System.Math.Pow(a[1] - b[1], 2));
        return distance;
    }

    public static int[,] CloneGrid(int[,] grid)
    {
        int[,] newGrid = new int[grid.GetLength(0), grid.GetLength(1)];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                newGrid[i, j] = grid[i, j];
            }
        }
        return newGrid;
    }

    public static void PrintPath(List<Node> arr)
    {
        string s = "";
        foreach (Node i in arr)
        {
            s += " " + i.ToString();
        }
        Debug.Log(s);
    }

    public static void PrintGridPathNodes(int[,] arrayGrid)
    {
        Debug.Log("Grid");
        for (int i = 0; i < arrayGrid.GetLength(0); i++)
        {
            String s = "";
            for (int j = 0; j < arrayGrid.GetLength(1); j++)
            {
                s += "  .  " + arrayGrid[i, j];
            }
            Debug.Log(s);
        }
    }

    public static MoveDirection GetDirectionFromVector(Vector3 vector)
    {
        if (vector == Vector3.left * Settings.GRID_CELL_SIZE)
        {
            return MoveDirection.LEFT;
        }
        else if (vector == Vector3.right * Settings.GRID_CELL_SIZE)
        {
            return MoveDirection.RIGHT;
        }
        else if (vector == Vector3.up * Settings.GRID_CELL_SIZE)
        {
            return MoveDirection.UP;
        }
        else if (vector == Vector3.down * Settings.GRID_CELL_SIZE)
        {
            return MoveDirection.DOWN;
        }
        else if (vector == new Vector3(-1, -1, 0) * Settings.GRID_CELL_SIZE)
        {
            return MoveDirection.DOWNLEFT;
        }
        else if (vector == new Vector3(1, -1, 0) * Settings.GRID_CELL_SIZE)
        {
            return MoveDirection.DOWNRIGHT;
        }
        else if (vector == new Vector3(-1, 1, 0) * Settings.GRID_CELL_SIZE)
        {
            return MoveDirection.UPLEFT;
        }
        else if (vector == new Vector3(1, 1, 0) * Settings.GRID_CELL_SIZE)
        {
            return MoveDirection.UPRIGHT;
        }
        else
        {
            return MoveDirection.IDLE;
        }
    }

    public static Vector3 GetVectorFromDirection(MoveDirection d)
    {
        //in case it is MoveDirection.IDLE do nothing
        Vector3 dir = new Vector3();

        if (d == MoveDirection.LEFT)
        {
            dir = Vector3.left * Settings.GRID_CELL_SIZE;
        }
        else if (d == MoveDirection.RIGHT)
        {
            dir = Vector3.right * Settings.GRID_CELL_SIZE;
        }
        else if (d == MoveDirection.UP)
        {
            dir = Vector3.up * Settings.GRID_CELL_SIZE;
        }
        else if (d == MoveDirection.DOWN)
        {
            dir = Vector3.down * Settings.GRID_CELL_SIZE;
        }
        else if (d == MoveDirection.DOWNLEFT)
        {
            dir = new Vector3(-1, -1, 0) * Settings.GRID_CELL_SIZE;
        }
        else if (d == MoveDirection.DOWNRIGHT)
        {
            dir = new Vector3(1, -1, 0) * Settings.GRID_CELL_SIZE;
        }
        else if (d == MoveDirection.UPLEFT)
        {
            dir = new Vector3(-1, 1, 0) * Settings.GRID_CELL_SIZE;
        }
        else if (d == MoveDirection.UPRIGHT)
        {
            dir = new Vector3(1, 1, 0) * Settings.GRID_CELL_SIZE;
        }
        return new Vector3(dir.x, dir.y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }

}