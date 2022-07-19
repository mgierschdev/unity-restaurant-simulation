using System;
using UnityEngine;
using System.Collections.Generic;

// In game position is defined by the grid coordinates (0,0), (0,1).
// World position is defined by the Unity Coords plane.
// It also controls the position of the objects in the grid and assign movements

public class GameGridController : MonoBehaviour
{
    private readonly int width = Settings.GRID_WIDTH;
    private readonly int height = Settings.GRID_HEIGHT;
    private readonly int cellSize = 1;
    private Vector3 cellOffset;
    private int[,] grid;

    // Path Finder object, contains the method to return the shortest path
    PathFind pathFind;

    // Game objects in UI either NPCs or Static objects
    private HashSet<Vector3> busyNodes;
    private Dictionary<GameItemController, Vector3> items;

    private TextMesh[,] debugArray;
    private Vector3 gridOriginPosition = new Vector3(Settings.GRID_START_X, Settings.GRID_START_Y, Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL);
    private Vector3 originPosition = new Vector3(Settings.GRID_START_X, Settings.GRID_START_Y, 0);

    // Debug Parameters
    private readonly int debugLineDuration = Settings.DEBUG_DEBUG_LINE_DURATION; // in seconds
    private readonly int cellTexttSize = Settings.DEBUG_TEXT_SIZE;
    private Vector3 textOffset;

    public void Start()
    {
        grid = new int[width, height];
        busyNodes = new HashSet<Vector3>();
        //SetGridBoundaries();

        items = new Dictionary<GameItemController, Vector3>();
        pathFind = new PathFind();
        cellOffset = new Vector3(cellSize, cellSize) * cellSize / 2;
        textOffset = new Vector3(cellSize, cellSize) * cellSize / 3;

        if (Settings.DEBUG_ENABLE)
        {

            debugArray = new TextMesh[width, height];
            for (int x = 0; x < grid.GetLength(0); x++)
            {

                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x, y + 1), Color.black, debugLineDuration);
                    Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x + 1, y), Color.black, debugLineDuration);
                    Color cellColor = Color.white;
                    if (grid[x, y] == 2 || grid[x, y] == 1)
                    {
                        cellColor = Color.black;
                    }
                    debugArray[x, y] = Util.CreateTextObject(x + "," + y, gameObject, x + "," + y, GetCellPosition(x, y) +
                        cellOffset - textOffset, cellTexttSize, cellColor, TextAnchor.MiddleCenter, TextAlignment.Center);
                }
                Debug.DrawLine(GetCellPosition(0, height), GetCellPosition(width, height), Color.black, debugLineDuration);
                Debug.DrawLine(GetCellPosition(width, 0), GetCellPosition(width, height), Color.black, debugLineDuration);
            }
        }
    }

    public void Update()
    {
        if (Settings.DEBUG_ENABLE)
        {
            MouseOnClick();
        }
    }

    //This method will draw boundaries around the grid
    private void SetGridBoundaries()
    {
        //Left
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            grid[0, i] = 1;
            busyNodes.Add(new Vector3(0, i));
        }

        //Right
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            grid[grid.GetLength(0) - 1, i] = 1;
            busyNodes.Add(new Vector3(grid.GetLength(0) - 1, i));
        }

        //Bottom
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            grid[i, 0] = 1;
            busyNodes.Add(new Vector3(i, 0));
        }

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            grid[i, grid.GetLength(1) - 1] = 1;
            busyNodes.Add(new Vector3(i, grid.GetLength(1) - 1));
        }
    }

    // Debug Methods
    private void MouseHeatMap()
    {
        Vector2Int mouseInGame = GetMousePositionInGame();
        if (IsInsideGridLimit(mouseInGame.x, mouseInGame.y))
        {
            SetValue(Util.GetMouseInWorldPosition(), GetCellValueInGamePosition(mouseInGame.x, mouseInGame.y) + (int)(100 * Time.deltaTime));
        }
    }

    private void MouseOnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Vector3 mousePosition = Util.GetMouseInWorldPosition();
            // Vector2Int mousePositionVector = Util.GetXYInGameMap(mousePosition);
            // List<Node> path = GetPath(new int[] { 1, 1 }, new int[] { mousePositionVector.x, mousePositionVector.y });
            // Debug.Log("Path found "+path.Count);

            // for (int i = 1; i < path.Count; i++)
            // {
            //     Vector3 from = GetCellPosition(path[i - 1].GetX(), path[i - 1].GetY(), 1);
            //     Vector3 to = GetCellPosition(path[i].GetX(), path[i].GetY(), 1);
            //     // Click Map
            //     // SetValue(Util.GetMouseInWorldPosition(), GetCellValueInGamePosition(mousePositionVector.x, mousePositionVector.y) + 10);
            //     Debug.DrawLine(from, to, Color.magenta, 5f);
            // }
        }
    }

    private void SetValue(int x, int y, int value)
    {
        if (!IsInsideGridLimit(x, y))
        {
            return;
        }
        grid[x, y] = value;
        debugArray[x, y].text = grid[x, y].ToString();
    }

    private int GetCellValueInGamePosition(int x, int y)
    {
        if (!IsInsideGridLimit(x, y))
        {
            Debug.LogError("The GetCellValueInGamePosition is outside boundaries");
            throw new Exception();
            return -1;
        }
        string text = debugArray[x, y].text;
        int.TryParse(text, out int value);
        return value;
    }

    private void SetCellColor(int x, int y, Color? color)
    {
        if (color == null)
        {
            color = Color.blue;
        }
        debugArray[x, y].text = x + "," + y;
        debugArray[x, y].color = (Color)color; // Busy
    }
    // Debug Methods

    private Vector3 GetCellPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + gridOriginPosition;
    }

    private void SetValue(Vector3 position, int value)
    {
        Vector2Int pos = Util.GetXYInGameMap(position);
        SetValue(pos.x, pos.y, value);
    }

    private bool IsInsideGridLimit(int x, int y)
    {
        return (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1));
    }

    // For setting objects position with offset
    public Vector3 GetCellPositionWithOffset(int x, int y)
    {
        return GetCellPosition(x, y) + cellOffset;
    }

    // Updating Items on the grid
    public void UpdateObjectPosition(GameItemController obj)
    {
        // if (!items.ContainsKey(obj))
        // {
        //     items.Add(obj, new Vector3(obj.GetX(), obj.GetY()));
        // }
        // else
        // {
        //     Vector3 prevPos = items.GetValueOrDefault(obj);
        //     if (prevPos != obj.GetPosition())
        //     {
        //         FreeGridPosition(obj.GetX(), obj.GetY());
        //         items[obj] = obj.GetPosition();
        //     }
        // }
        SetGridObstacle(obj.GetX(), obj.GetY(), obj.GetType(), Color.black);
    }

    public void SetObstacleInPosition(int x, int y, ObjectType type)
    {
        SetGridObstacle(x, y, type);
    }

    public Vector2Int GetMousePositionInGame()
    {
        Vector3 mousePosition = Util.GetMouseInWorldPosition();
        return Util.GetXYInGameMap(mousePosition);
    }

    public Vector3 GetCellPosition(int x, int y, int z)
    {
        if (!IsInsideGridLimit(x, y))
        {
            Debug.LogWarning("The GetCellPosition is outside boundaries");
            //throw new Exception();
            return new Vector3();
        }
        return new Vector3(x, y, z) * cellSize + originPosition;
    }

    // This sets the obstacle points around the obstacle
    private void SetGridObstacle(int x, int y, ObjectType type, Color? color = null)
    {
        if (color == null)
        {
            color = Color.blue;
        }

        if (!IsInsideGridLimit(x, y) && x > 1 && y > 1)
        {
            Debug.LogError("The object should be placed inside the perimeter");
            return;
        }

        grid[x, y] = (int)type;

        if (ObjectType.OBSTACLE == type)
        {
            grid[x, y] = (int)type;
        }

        if (Settings.DEBUG_ENABLE && type != ObjectType.NPC)
        {
            SetCellColor(x, y, color);
            if (ObjectType.OBSTACLE == type)
            {
                SetCellColor(x, y, color);
            }
        }
    }

    // Only for unit test use
    public void SetTestGridObstacles(int row, int x1, int x2){
        //int x, int y, ObjectType type, Color? color = null
        for(int i = x1; i <= x2; i++){
            SetGridObstacle(row, i, ObjectType.OBSTACLE, Color.black);
        }
    }

    // Only for unit test use
    public void FreeTestGridObstacles(int row, int x1, int x2){
        for(int i = x1; i <= x2; i++){
            FreeGridPosition(row, i);
        }
    }

    // Only used in game not in Playmode, since it does not find the reference of the GameGrid inside the ItemObject
    public void SetHorizontalObstaclesInGrid(int row, int x1, int x2)
    {
        if (x1 == 1 || x1 > x2)
        {
            Debug.LogWarning("Set obstacles properly");
            return;
        }

        for (int i = x1; i <= x2; i++)
        {
            Vector2Int obstacle = new Vector2Int(i, row);
            Vector3 objPos = GetCellPositionWithOffset(obstacle.x, obstacle.y);
            GameObject obstacleObject = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), new Vector3(objPos.x, objPos.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
            obstacleObject.name = "Obstacle: " + i + "," + row;
            obstacleObject.transform.SetParent(transform);
        }
    }

    // Unset position in Grid
    public void FreeGridPosition(int x, int y)
    {
        if (!IsInsideGridLimit(x, y) && x > 1 && y > 1)
        {
            Debug.LogWarning("The object should be placed inside the perimeter");
            return;
        }

        grid[x, y] = 0;
        if (Settings.DEBUG_ENABLE)
        {
            SetCellColor(x, y, Color.white);
        }
    }

    public List<Node> GetPath(int[] start, int[] end)
    {
        return pathFind.Find(start, end, grid);
    }
}