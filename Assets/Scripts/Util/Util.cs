using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// This will contain Utility functions, to create Unity Object and other
public static class Util
{
    // Z ordering objects , less closer to the camera
    public static int NPCZPosition = 0,
    ObjectZPosition = -1,
    SelectedObjectZPosition = -2;
    //UI sorting
    public const int highlightObjectSortingPosition = 800,
    ConstDefaultBackgroundOrderingLevel = 200;
    // Colors
    public static Color Unavailable = new Color(0.1f, 0.1f, 0.1f, 1);
    public static Color Available = new Color(0, 1, 50, 1);
    public static Color LightAvailable = new Color(0, 1, 50, 0.5f);
    public static Color LightOccupied = new Color(1, 0, 0, 0.2f);
    public static Color Occupied = new Color(1, 0, 0, 0.4f);
    public static Color Free = new Color(1, 1, 1, 1);
    public static Color Hidden = new Color(0, 0, 0, 0);
    public static int[,] ArroundVectorPoints = new int[,] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 } };
    public static int[,] ArroundPartialVectorPoints = new int[,] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
    public static int[,] AroundVectorPointsPlusTwo = new int[,] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 }, { 1, 1 }, { -1, -1 }, { 2, 2 }, { 0, -2 }, { -2, 0 }, { 0, 2 }, { 2, 0 } };
    public static int[,] ObejectSide = new int[,] { { 0, 1 }, { 1, 0 } };

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
        textMesh.GetComponent<MeshRenderer>().sortingOrder = ConstDefaultBackgroundOrderingLevel;
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
        GameLog.Log(output);
    }
    public static double EuclidianDistance(int[] coordA, int[] coordB)
    {
        return System.Math.Sqrt(System.Math.Pow(coordA[0] - coordB[0], 2) + System.Math.Pow(coordA[1] - coordB[1], 2));
    }

    public static double EuclidianDistance(Vector3Int a, Vector3Int b)
    {
        return System.Math.Sqrt(System.Math.Pow(a.x - b.x, 2) + System.Math.Pow(a.y - b.y, 2));
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

    public static Vector3Int GetGridPositionFromMoveDirection(MoveDirection d, Vector3Int pos)
    {
        if (d == MoveDirection.DOWN)
        {
            return pos + new Vector3Int(0, -1, 0);
        }
        else if (d == MoveDirection.UP)
        {
            return pos + new Vector3Int(0, 1, 0);
        }
        else if (d == MoveDirection.LEFT)
        {
            return pos + new Vector3Int(-1, 0, 0);
        }
        else
        {
            return pos + new Vector3Int(-1, 0, 0);
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
        Vector3 gridOriginPosition = new Vector3(Settings.GridStartX, Settings.GrtGridStartY, ConstDefaultBackgroundOrderingLevel);
        Vector3 cellPosition = new Vector3(x, y) * Settings.GridCellSize + new Vector3(gridOriginPosition.x, gridOriginPosition.y, 0);
        return new Vector3(cellPosition.x, cellPosition.y);
    }

    // Gets the cell position in a non-isometric grid
    public static Vector3 GetCellPosition(Vector3 position)
    {
        Vector3 gridOriginPosition = new Vector3(Settings.GridStartX, Settings.GrtGridStartY, ConstDefaultBackgroundOrderingLevel);
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
        // ofset for diagonal movements 
        float offset = 20;

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
            "WalkTile@3x" => TileType.WALKABLE_PATH,
            "floor3" => TileType.FLOOR_3,
            "BussTile@3x" => TileType.BUS_FLOOR,
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

    public static bool IsNull(LoadSliderController gameObject, string message)
    {
        if (gameObject == null)
        {
            GameLog.LogWarning(message);
            return true;
        }
        return false;
    }

    public static Vector3Int GetVector3IntNegativeInfinity()
    {
        return new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    }

    public static bool IsAtDistanceWithObject(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(new Vector3(a.x, a.y, 0), new Vector3(b.x, b.y, 0)) <= 0;
    }

    // Meassures the distance between a and b, and translate the transform position of the object to fix small precision problem, 0.01f
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

    // Returns the sorting given the coords of the objects
    public static int GetSorting(Vector3Int pos)
    {
        int x = pos.x;
        int y = pos.y;
        return (x + y) * -1;
    }

    public static void EnqueueToList(List<GameGridObject> list, GameGridObject obj)
    {
        list.Insert(0, obj);
    }

    public static GameGridObject DequeueFromList(List<GameGridObject> list)
    {
        if (list.Count == 0)
        {
            GameLog.Log("There is no spots to dequeue");
            return null;
        }

        GameGridObject obj = list.Last();
        list.RemoveAt(list.Count - 1);
        return obj;
    }

    public static GameGridObject PeekFromList(List<GameGridObject> list)
    {
        return list.Last();
    }

    public static Color GetRandomColor()
    {
        return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }

    public static Color GetRandomColor(int val)
    {
        switch (val)
        {
            case 0: return Color.black;
            case 1: return Color.blue;
            case 2: return Color.cyan;
            case 3: return Color.gray;
            case 4: return Color.green;
            case 5: return Color.grey;
            case 6: return Color.magenta;
            case 7: return Color.red;
            case 8: return Color.white;
            case 9: return Color.yellow;
            case 10: return Color.yellow;
        }
        return Color.black;
    }

    public static int[,] TransposeGridForDebugging(int[,] grid)
    {
        int w = grid.GetLength(0);
        int h = grid.GetLength(1);

        int[,] result = new int[h, w];

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                result[h - j - 1, i] = grid[i, j];
            }
        }
        return result;
    }

    public static bool IsInternetReachable()
    {
        int count = 0;
        //Check if the device cannot reach the internet
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //Change the Text
            // m_ReachabilityText = "Not Reachable.";
            count++;
        }
        //Check if the device can reach the internet via a carrier data network
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            // m_ReachabilityText = "Reachable via carrier data network.";
            count++;
        }
        //Check if the device can reach the internet via a LAN
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            // m_ReachabilityText = "Reachable via Local Area Network.";
            count++;
        }

        return count > 0;
    }

    public static Vector3 GetCameraPoisiton()
    {
        return Camera.main.transform.position;
    }

    public static bool CompareNegativeInifinity(Vector3 coord)
    {
        return coord.Equals(Vector3.negativeInfinity) || coord.Equals(new Vector3(Vector3.negativeInfinity.x, Vector3.negativeInfinity.y, 0));
    }

    public static bool CompareNegativeInifinity(Vector3Int coord)
    {
        return coord.Equals(GetVector3IntNegativeInfinity()) || coord.Equals(new Vector3(GetVector3IntNegativeInfinity().x, GetVector3IntNegativeInfinity().y, 0));
    }

    public static void DrawMainCameraCorners()
    {
        Camera camera = Camera.main;
        Rect rect = camera.rect;

        Vector3Int cameraGridPosition = BussGrid.GetLocalGridFromWorldPosition(camera.transform.position);
        Debug.Log("0,0 left bot corner, top right corner:" + camera.pixelWidth + "," + camera.pixelHeight);

        int size = 8;

        Vector3Int rightBotCorner = cameraGridPosition + new Vector3Int(0, -size);
        Vector3Int leftBotCorner = cameraGridPosition + new Vector3Int(-size, 0);
        Vector3Int leftTopCorner = cameraGridPosition + new Vector3Int(0, size);
        Vector3Int rightTopCorner = cameraGridPosition + new Vector3Int(size, 0);

        Debug.Log("Camera position " + cameraGridPosition);
        //BussGrid.DrawCell(cameraGridPosition);
        BussGrid.DrawCell(rightBotCorner);
        BussGrid.DrawCell(leftBotCorner);
        BussGrid.DrawCell(leftTopCorner);
        BussGrid.DrawCell(rightTopCorner);
    }
}