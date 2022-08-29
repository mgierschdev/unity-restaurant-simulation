using System;
using System.Collections.Generic;
using UnityEngine;


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

    public static void PrintGrid(int[,] grid)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            String row = "";
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                row += "(" + i + "," + j + ")" + grid[i, j] + " ";
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
        Debug.Log("Printing Path");
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

    public static Vector2Int GetXYInGameMap(Vector3 position)
    {
        return new Vector2Int(
            (int)Math.Round((position.x - Settings.GRID_START_X) * 1 / Settings.GRID_CELL_SIZE, MidpointRounding.AwayFromZero),
            (int)Math.Round((position.y - Settings.GRID_START_Y) * 1 / Settings.GRID_CELL_SIZE, MidpointRounding.AwayFromZero));
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
        Vector3 dir = new Vector3(0, 0);

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
        return new Vector3(dir.x, dir.y);
    }

    // Gets the GRID cell position in a non-isometric grid
    public static Vector3 GetCellPosition(int x, int y)
    {
        Vector3 gridOriginPosition = new Vector3(Settings.GRID_START_X, Settings.GRID_START_Y, Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL);
        Vector3 cellPosition = new Vector3(x, y) * Settings.GRID_CELL_SIZE + new Vector3(gridOriginPosition.x, gridOriginPosition.y, 0);
        return new Vector3(cellPosition.x, cellPosition.y);
    }

    // Gets the cell position in a non-isometric grid
    public static Vector3 GetCellPosition(Vector3 position)
    {
        Vector3 gridOriginPosition = new Vector3(Settings.GRID_START_X, Settings.GRID_START_Y, Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL);
        Vector3 cellPosition = position * Settings.GRID_CELL_SIZE + new Vector3(gridOriginPosition.x, gridOriginPosition.y, 0);
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
        if (tileName == "floor1")
        {
            return TileType.SPAM_POINT;
        }
        else if (tileName == "floor2")
        {
            return TileType.WALKABLE_PATH;
        }
        else if (tileName == "floor3")
        {
            return TileType.FLOOR_3;
        }
        else if (tileName == "floor4")
        {
            return TileType.BUS_FLOOR;
        }
        else if (tileName == "floor5")
        {
            return TileType.WALL;
        }
        else if (tileName == "Complete@3x")
        {
            return TileType.FLOOR_OBSTACLE;
        }
        else if (tileName == "MediumHorizontal@3x")
        {
            return TileType.FLOOR_MEDIUM_HORIZONTAL_OBSTACLE;
        }
        else if (tileName == "MediumVertical@3x")
        {
            return TileType.FLOOR_MEDIUM_VERTICAL_OBSTACLE;
        }
        else if (tileName == "ShortHorizontal@3x")
        {
            return TileType.FLOOR_SHORT_HORIZONTAL_OBSTACLE;
        }
        else if (tileName == "ShortVertical@3x")
        {
            return TileType.FLOOR_SHORT_VERTICAL_OBSTACLE;
        }
        else if (tileName == "GridTile")
        {
            return TileType.ISOMETRIC_GRID_TILE;
        }
        else if (tileName == "Wall@3x")
        {
            return TileType.WALL;
        }
        else
        {
            return TileType.UNDEFINED;
        }
    }

    public static ObjectType GetTileObjectType(TileType type)
    {
        if (TileType.FLOOR_OBSTACLE == type ||
        TileType.FLOOR_MEDIUM_HORIZONTAL_OBSTACLE == type ||
        TileType.FLOOR_MEDIUM_VERTICAL_OBSTACLE == type ||
        TileType.FLOOR_SHORT_HORIZONTAL_OBSTACLE == type ||
        TileType.FLOOR_SHORT_VERTICAL_OBSTACLE == type ||
        TileType.ISOMETRIC_GRID_TILE == type ||
        TileType.WALL == type)
        {
            return ObjectType.OBSTACLE;
        }
        else if (type == TileType.SPAM_POINT ||
        type == TileType.FLOOR_3 ||
        type == TileType.BUS_FLOOR ||
        type == TileType.WALKABLE_PATH)
        {
            return ObjectType.FLOOR;
        }
        else
        {
            return ObjectType.UNDEFINED;
        }
    }

    public static ObjectType GetObjectType(GameObject gameObject)
    {
        if (gameObject.tag == Settings.NPC_TAG)
        {
            return ObjectType.NPC;
        }
        else if (gameObject.tag == Settings.NPC_EMPLOYEE_TAG)
        {
            return ObjectType.EMPLOYEE;

        }
        else
        {
            return ObjectType.UNDEFINED;
        }
    }

    public static void PrintAllComponents(GameObject gameObject)
    {
        UnityEngine.Component[] components = gameObject.GetComponents(typeof(UnityEngine.Component));

        foreach (UnityEngine.Component c in components)
        {
            Debug.Log(c.name + " " + c.ToString());

        }
    }

    public static bool IsNull(GameObject gameObject, string message)
    {
        if (gameObject == null)
        {
            Debug.LogWarning(message);
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool IsNull(GridController gameObject, string message)
    {
        if (gameObject == null)
        {
            Debug.LogWarning(message);
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool IsNull(EnergyBarController gameObject, string message)
    {
        if (gameObject == null)
        {
            Debug.LogWarning(message);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static Vector3Int GetVector3IntPositiveInfinity()
    {
        return new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
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
public enum NPCState
{
    IDLE = 0,
    WALKING_TO_TABLE = 1,
    AT_TABLE = 2,
    WALKING_TO_COUNTER = 3,
    AT_COUNTER = 4,
    WANDER = 5,
    TAKING_ORDER = 6,
    GOING_TO_REGISTER = 7,
    WAITING_TO_BE_ATTENDED = 8
}

// List of Menus
public enum Menu
{
    CENTER_TAB_MENU,
    TOP_MENU,
    NPC_PROFILE
}

//Menu Types
public enum MenuType
{
    TAB_MENU,
    DIALOG,
    ON_SCREEN
}

public enum Tabs
{
    CONFIG_TAB,
    ITEMS_TAB
}