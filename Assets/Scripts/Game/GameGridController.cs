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
    private double[,] grid;

    // Path Finder object, contains the method to return the shortest path
    PathFind pathFind;

    // Game objects in UI either NPCs or Static objects
    private Dictionary<NPCController, Vector3> npcs;
    private Dictionary<GameItemController, Vector3> items;

    private TextMesh[,] debugArray;
    private Vector3 gridOriginPosition = new Vector3(Settings.GRID_START_X, Settings.GRID_START_Y, Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL);
    private Vector3 originPosition = new Vector3(Settings.GRID_START_X, Settings.GRID_START_Y, 0);

    // Debug Parameters
    private readonly int debugLineDuration = Settings.DEBUG_DEBUG_LINE_DURATION; // in seconds
    private readonly int cellTexttSize = Settings.DEBUG_TEXT_SIZE;

    //Caching 

    public void Awake()
    {
        grid = new double[width, height];
        items = new Dictionary<GameItemController, Vector3>();
        npcs = new Dictionary<NPCController, Vector3>();
        pathFind = new PathFind();

        Vector3 textCellOffset = new Vector3(cellSize, cellSize) * cellSize / 2;

        if (Settings.DEBUG_ENABLE)
        {

            debugArray = new TextMesh[width, height];
            for (int x = 0; x < grid.GetLength(0); x++)
            {

                for (int y = 0; y < grid.GetLength(1); y++)
                {

                    Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x, y + 1), Color.white, debugLineDuration);
                    Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x + 1, y), Color.white, debugLineDuration);
                    debugArray[x, y] = Util.CreateTextObject(x + "," + y, gameObject, grid[x, y].ToString(), GetCellPosition(x, y) +
                        textCellOffset, cellTexttSize, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                }
                Debug.DrawLine(GetCellPosition(0, height), GetCellPosition(width, height), Color.white, debugLineDuration);
                Debug.DrawLine(GetCellPosition(width, 0), GetCellPosition(width, height), Color.white, debugLineDuration);
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
            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector2Int mousePositionVector = Util.GetXYInGameMap(mousePosition);
            SetValue(Util.GetMouseInWorldPosition(), GetCellValueInGamePosition(mousePositionVector.x, mousePositionVector.y) + 10);
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
        int value;
        int.TryParse(text, out value);
        return value;
    }

    private void SetCellColor(int x, int y, Color? color)
    {
        if (color == null)
        {
            color = Color.blue;
        }
        debugArray[x, y].text = grid[x, y].ToString();
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
            throw new Exception();
            return new Vector3();
        }
        return new Vector3(x, y, z) * cellSize + originPosition;
    }

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
        grid[x, y - 1] = (int)type;
        grid[x - 1, y] = (int)type;
        grid[x - 1, y - 1] = (int)type;

        if (Settings.DEBUG_ENABLE)
        {
            SetCellColor(x, y, color);
            SetCellColor(x, y - 1, color);
            SetCellColor(x - 1, y, color);
            SetCellColor(x - 1, y - 1, color);
        }
    }

    public void UpdateObjectPosition(NPCController obj)
    {
        if (!npcs.ContainsKey(obj))
        {
            npcs.Add(obj, new Vector3(obj.GetX(), obj.GetY()));
        }
        else if (npcs[obj] != obj.GetPosition())
        {
            FreeGridPosition((int)npcs[obj].x, (int)npcs[obj].y);
            npcs[obj] = obj.GetPosition();
        }
        SetGridObstacle(obj.GetX(), obj.GetY(), obj.GetType(), Color.black);
    }

    public void UpdateObjectPosition(GameItemController obj)
    {
        if (!items.ContainsKey(obj))
        {
            items.Add(obj, new Vector3(obj.GetX(), obj.GetY()));
        }
        else
        {
            Vector3 prevPos = items.GetValueOrDefault(obj);
            if (prevPos != obj.GetPosition())
            {
                FreeGridPosition(obj.GetX(), obj.GetY());
                items[obj] = obj.GetPosition();
            }
        }
        SetGridObstacle(obj.GetX(), obj.GetY(), obj.GetType(), Color.black);
    }

    public void SetObstacleInPosition(int x, int y, ObjectType type)
    {
        SetGridObstacle(x, y, type);
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
        grid[x, y - 1] = 0;
        grid[x - 1, y] = 0;
        grid[x - 1, y - 1] = 0;

        if (Settings.DEBUG_ENABLE)
        {
            SetCellColor(x, y, Color.white);
            SetCellColor(x, y - 1, Color.white);
            SetCellColor(x - 1, y, Color.white);
            SetCellColor(x - 1, y - 1, Color.white);
        }
    }
}