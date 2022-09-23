using System;
using System.Collections.Generic;
using UnityEngine;


// This will contain Utility functions, to create Unity Object and other
public static class Util
{
    private const int sortingLevel = Settings.ConstDefaultBackgroundOrderingLevel; // Background 
    public static Color Unavailable = new Color(0.1f, 0.1f, 0.1f, 1);
    public static Color Available = new Color(142, 175, 50, 1);
    public static Color LightAvailable = new Color(142, 175, 50, 0.2f);
    public static Color LightOccupied = new Color(1, 0, 0, 0.2f);
    public static Color Occupied = new Color(1, 0, 0, 0.4f);
    public static Color Free = new Color(1, 1, 1, 1);
    public static Color Hidden = new Color(0, 0, 0, 0);

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

    public static void PrintGrid(int[,] grid)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            String row = "";
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                row += "(" + i + "," + j + ")" + grid[i, j] + " ";
            }
            GameLog.Log(row);
        }
        GameLog.Log(" ");
    }

    public static void PrintBussGrid(int[,] bGrid)
    {
        string output = " ";
        for (int i = 0; i < bGrid.GetLength(0); i++)
        {
            for (int j = 0; j < bGrid.GetLength(1); j++)
            {
                if (bGrid[i, j] == -1)
                {
                    output += " 0";
                }
                else
                {
                    output += " " + bGrid[i, j];
                }
            }
            output += "\n";
        }
        Debug.Log(output);
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
        GameLog.Log("Printing Path");
        string s = "";
        foreach (Node i in arr)
        {
            s += " " + i.ToString();
        }
        GameLog.Log(s);
    }

    public static void PrintGridPathNodes(int[,] arrayGrid)
    {
        GameLog.Log("Grid");
        for (int i = 0; i < arrayGrid.GetLength(0); i++)
        {
            String s = "";
            for (int j = 0; j < arrayGrid.GetLength(1); j++)
            {
                s += "  .  " + arrayGrid[i, j];
            }
            GameLog.Log(s);
        }
    }

    public static Vector2Int GetXYInGameMap(Vector3 position)
    {
        return new Vector2Int(
            (int)Math.Round((position.x - Settings.GridStartX) * 1 / Settings.GridCellSize, MidpointRounding.AwayFromZero),
            (int)Math.Round((position.y - Settings.GrtGridStartY) * 1 / Settings.GridCellSize, MidpointRounding.AwayFromZero));
    }

    public static MoveDirection GetDirectionFromVector(Vector3 vector)
    {
        if (vector == Vector3.left * Settings.GridCellSize)
        {
            return MoveDirection.LEFT;
        }
        else if (vector == Vector3.right * Settings.GridCellSize)
        {
            return MoveDirection.RIGHT;
        }
        else if (vector == Vector3.up * Settings.GridCellSize)
        {
            return MoveDirection.UP;
        }
        else if (vector == Vector3.down * Settings.GridCellSize)
        {
            return MoveDirection.DOWN;
        }
        else if (vector == new Vector3(-1, -1, 0) * Settings.GridCellSize)
        {
            return MoveDirection.DOWNLEFT;
        }
        else if (vector == new Vector3(1, -1, 0) * Settings.GridCellSize)
        {
            return MoveDirection.DOWNRIGHT;
        }
        else if (vector == new Vector3(-1, 1, 0) * Settings.GridCellSize)
        {
            return MoveDirection.UPLEFT;
        }
        else if (vector == new Vector3(1, 1, 0) * Settings.GridCellSize)
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
        Vector3 dir = new Vector3(0, 0);

        if (d == MoveDirection.LEFT)
        {
            dir = Vector3.left * Settings.GridCellSize;
        }
        else if (d == MoveDirection.RIGHT)
        {
            dir = Vector3.right * Settings.GridCellSize;
        }
        else if (d == MoveDirection.UP)
        {
            dir = Vector3.up * Settings.GridCellSize;
        }
        else if (d == MoveDirection.DOWN)
        {
            dir = Vector3.down * Settings.GridCellSize;
        }
        else if (d == MoveDirection.DOWNLEFT)
        {
            dir = new Vector3(-1, -1, 0) * Settings.GridCellSize;
        }
        else if (d == MoveDirection.DOWNRIGHT)
        {
            dir = new Vector3(1, -1, 0) * Settings.GridCellSize;
        }
        else if (d == MoveDirection.UPLEFT)
        {
            dir = new Vector3(-1, 1, 0) * Settings.GridCellSize;
        }
        else if (d == MoveDirection.UPRIGHT)
        {
            dir = new Vector3(1, 1, 0) * Settings.GridCellSize;
        }
        return new Vector3(dir.x, dir.y);
    }

    // Gets the GRID cell position in a non-isometric grid
    public static Vector3 GetCellPosition(int x, int y)
    {
        Vector3 gridOriginPosition = new Vector3(Settings.GridStartX, Settings.GrtGridStartY, Settings.ConstDefaultBackgroundOrderingLevel);
        Vector3 cellPosition = new Vector3(x, y) * Settings.GridCellSize + new Vector3(gridOriginPosition.x, gridOriginPosition.y, 0);
        return new Vector3(cellPosition.x, cellPosition.y);
    }

    // Gets the cell position in a non-isometric grid
    public static Vector3 GetCellPosition(Vector3 position)
    {
        Vector3 gridOriginPosition = new Vector3(Settings.GridStartX, Settings.GrtGridStartY, Settings.ConstDefaultBackgroundOrderingLevel);
        Vector3 cellPosition = position * Settings.GridCellSize + new Vector3(gridOriginPosition.x, gridOriginPosition.y, 0);
        return new Vector3(cellPosition.x, cellPosition.y);
    }

    // Translates a normalized angle to a direction from 0 - 360
    //       360 | 0
    //           .
    //       315 . 45 
    // 270 --------------- 90
    //       225 . 135
    //           .
    //          180

    public static MoveDirection GetDirectionFromAngles(float angle)
    {
        float offset = 20; // ofset for diagonal movements 

        if (angle >= 45 - offset && angle <= 45 + offset)
        {
            return MoveDirection.UPRIGHT;
        }
        else if (angle >= 135 - offset && angle <= 135 + offset)
        {
            return MoveDirection.DOWNRIGHT;
        }
        else if (angle >= 225 - offset && angle <= 225 + offset)
        {
            return MoveDirection.DOWNLEFT;
        }
        else if (angle >= 315 - offset && angle <= 315 + offset)
        {
            return MoveDirection.UPLEFT;
        }
        else if ((angle > 315 + offset && angle <= 360) || (angle >= 0 && angle < 45 - offset))
        {
            return MoveDirection.UP;
        }
        else if (angle > 45 + offset && angle < 135 - offset)
        {
            return MoveDirection.RIGHT;
        }
        else if (angle > 135 + offset && angle < 225 - offset)
        {
            return MoveDirection.DOWN;
        }
        else if (angle > 225 + offset && angle < 315 - offset)
        {
            return MoveDirection.LEFT;
        }
        else
        {
            return MoveDirection.IDLE;
        }
    }
    
    public static TileType GetTileType(string tileName)
    {
        return tileName switch
        {
            "floor1" => TileType.SPAM_POINT,
            "floor2" => TileType.WALKABLE_PATH,
            "floor3" => TileType.FLOOR_3,
            "floor4" => TileType.BUS_FLOOR,
            "floor5" => TileType.WALL,
            "Complete@3x" => TileType.FLOOR_OBSTACLE,
            "MediumHorizontal@3x" => TileType.FLOOR_MEDIUM_HORIZONTAL_OBSTACLE,
            "MediumVertical@3x" => TileType.FLOOR_MEDIUM_VERTICAL_OBSTACLE,
            "ShortHorizontal@3x" => TileType.FLOOR_SHORT_HORIZONTAL_OBSTACLE,
            "ShortVertical@3x" => TileType.FLOOR_SHORT_VERTICAL_OBSTACLE,
            "GridTile" => TileType.ISOMETRIC_GRID_TILE,
            "Wall@3x" => TileType.WALL,
            "HighlightedFloor@3x" => TileType.FLOOR_EDIT,
            _ => TileType.UNDEFINED
        };
    }

    public static ObjectType GetTileObjectType(TileType type)
    {
        switch (type)
        {
            case TileType.FLOOR_OBSTACLE:
            case TileType.FLOOR_MEDIUM_HORIZONTAL_OBSTACLE:
            case TileType.FLOOR_MEDIUM_VERTICAL_OBSTACLE:
            case TileType.FLOOR_SHORT_HORIZONTAL_OBSTACLE:
            case TileType.FLOOR_SHORT_VERTICAL_OBSTACLE:
            case TileType.ISOMETRIC_GRID_TILE:
            case TileType.WALL:
                return ObjectType.OBSTACLE;
            case TileType.SPAM_POINT:
            case TileType.FLOOR_3:
            case TileType.BUS_FLOOR:
            case TileType.WALKABLE_PATH:
                return ObjectType.FLOOR;
            case TileType.ISOMETRIC_SINGLE_SQUARE_OBJECT:
                break;
            case TileType.ISOMETRIC_FOUR_SQUARE_OBJECT:
                break;
            case TileType.FLOOR_EDIT:
                break;
            case TileType.UNDEFINED:
                break;
            default:
                return ObjectType.UNDEFINED;
        }

        return ObjectType.UNDEFINED;
    }

    public static ObjectType GetObjectType(GameObject gameObject)
    {
        return gameObject.tag switch
        {
            Settings.NpcTag => ObjectType.NPC,
            Settings.NpcEmployeeTag => ObjectType.EMPLOYEE,
            _ => ObjectType.UNDEFINED
        };
    }

    public static void PrintAllComponents(GameObject gameObject)
    {
        Component[] components = gameObject.GetComponents(typeof(Component));

        foreach (Component c in components)
        {
            GameLog.Log("Component: " + c.name + " " + c.ToString() + " " + c.GetType());
        }
    }

    public static bool IsNull(GameObject gameObject, string message)
    {
        if (gameObject != null)
        {
            return false;
        }

        GameLog.LogWarning(message);
        return true;

    }
    public static bool IsNull(GridController gameObject, string message)
    {
        if (gameObject == null)
        {
            GameLog.LogWarning(message);
            return true;
        }
        return false;
    }
    public static bool IsNull(EnergyBarController gameObject, string message)
    {
        if (gameObject == null)
        {
            GameLog.LogWarning(message);
            return true;
        }
        return false;
    }

    public static Vector3Int GetVector3IntPositiveInfinity()
    {
        return new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
    }

    public static Vector3 GetActionCellOffSetWorldPositon(ObjectType type)
    {
        return type == ObjectType.NPC_SINGLE_TABLE ? new Vector3(-0.50f, 0.25f, 0) : Vector3.zero;
    }

    public static Vector3Int GetActionCellOffSet(ObjectType type)
    {
        return type == ObjectType.NPC_SINGLE_TABLE ? new Vector3Int(0, 1, 0) : Vector3Int.zero;
    }

    public static bool IsAtDistanceWithObject(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(new Vector3(a.x, a.y, 0), new Vector3(b.x, b.y, 0)) < Settings.MinDistanceToTarget;
    }

    public static bool IsAtDistanceWithObjectTraslate(Vector3 a, Vector3 b, Transform transform)
    {
        if (Vector3.Distance(new Vector3(a.x, a.y, 0), new Vector3(b.x, b.y, 0)) < Settings.MinDistanceToTarget)
        {
            transform.position = b;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static long GetUnixTimeNow()
    {
        var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        return (long)timeSpan.TotalSeconds;
    }
}