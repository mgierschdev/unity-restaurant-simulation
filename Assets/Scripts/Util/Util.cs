using System;
using System.Collections.Generic;
using UnityEngine;


// This will contain Utility functions, to create Unity Object and other
public static class Util
{
    private const int sortingLevel = Settings.ConstDefaultBackgroundOrderingLevel; // Background 
    public static Color Unavailable = new Color(0.1f, 0.1f, 0.1f, 1);
    public static Color Available = new Color(0, 1, 0, 0.4f);
    public static Color Occupied = new Color(1, 0, 0, 0.4f);
    public static Color Free = new Color(1, 1, 1, 1);

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
            default:
                return ObjectType.UNDEFINED;
        }
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
            GameLog.Log(c.name + " " + c.ToString());
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
        return type == ObjectType.NPC_TABLE ? new Vector3(-0.50f, 0.25f, 0) : Vector3.zero;
    }

    public static Vector3Int GetActionCellOffSet(ObjectType type)
    {
        return type == ObjectType.NPC_TABLE ? new Vector3Int(0, 1, 0) : Vector3Int.zero;
    }
}

//Item types
// to reference the type of object, each with different properties
public enum ObjectType
{
    OBSTACLE = 1,
    NPC = 2,
    PLAYER = 3,
    EMPLOYEE = 4,
    NPC_TABLE = 5,
    NPC_COUNTER = 6,
    FLOOR = 7,
    UNDEFINED = 999
}

// To reference from fileNames to object names
public enum TileType
{
    SPAM_POINT = 1,
    WALKABLE_PATH = 2,
    FLOOR_3 = 3,
    BUS_FLOOR = 4,
    FLOOR_OBSTACLE = 5,
    FLOOR_MEDIUM_HORIZONTAL_OBSTACLE = 6,
    FLOOR_MEDIUM_VERTICAL_OBSTACLE = 7,
    FLOOR_SHORT_HORIZONTAL_OBSTACLE = 8,
    FLOOR_SHORT_VERTICAL_OBSTACLE = 9,
    ISOMETRIC_GRID_TILE = 10,
    ISOMETRIC_SINGLE_SQUARE_OBJECT = 11,
    ISOMETRIC_FOUR_SQUARE_OBJECT = 12,
    WALL = 13,
    FLOOR_EDIT = 14,
    UNDEFINED = 999
}

//Players and NPCs move directions
public enum MoveDirection
{
    IDLE = 0,
    UP = 1,
    DOWN = 2,
    LEFT = 3,
    RIGHT = 4,
    UPLEFT = 5,
    UPRIGHT = 6,
    DOWNLEFT = 7,
    DOWNRIGHT = 8
}

//Players and NPCs, to set the NPC to wander or other states
public enum NpcState
{
    IDLE = 0,
    WALKING_TO_TABLE = 1,
    AT_TABLE = 2,
    WALKING_TO_COUNTER = 3,
    AT_COUNTER = 4,
    WALKING_WANDER = 5,
    TAKING_ORDER = 6,
    WAITING_TO_BE_ATTENDED = 7,
    WALKING_UNRESPAWN = 8,
    WALKING_TO_COUNTER_AFTER_ORDER = 9,
    REGISTERING_CASH = 10
}

// List of Menus
public enum Menu
{
    CENTER_TAB_MENU,
    NPC_PROFILE
}

//Menu Types
public enum MenuType
{
    TAB_MENU,
    DIALOG
}