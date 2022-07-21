using System;
using UnityEngine;
using System.Collections.Generic;

// In game position is defined by the grid coordinates (0,0), (0,1).
// World position is defined by the Unity Coords plane.
// It also controls the position of the objects in the grid and assign movements

public class GameGridController : MonoBehaviour
{
    private float cellSize = Settings.GRID_CELL_SIZE;
    private Vector3 cellOffset;
    private int[,] grid;

    // Path Finder object, contains the method to return the shortest path
    PathFind pathFind;

    private TextMesh[,] debugArray;
    private Vector3 gridOriginPosition = new Vector3(Settings.GRID_START_X, Settings.GRID_START_Y, Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL);
    private Vector3 originPosition = new Vector3(Settings.GRID_START_X, Settings.GRID_START_Y, 0);

    // Debug Parameters
    private readonly int debugLineDuration = Settings.DEBUG_DEBUG_LINE_DURATION; // in seconds
    private readonly int cellTexttSize = Settings.DEBUG_TEXT_SIZE;
    private Vector3 textOffset;

    public void Start()
    {
        int cellsX = (int)Settings.GRID_WIDTH;
        int cellsY = (int)(Settings.GRID_HEIGHT);

        grid = new int[cellsX, cellsY];
        //SetGridBoundaries();
        pathFind = new PathFind();
        cellOffset = new Vector3(cellSize, cellSize) * cellSize / 2;
        textOffset = new Vector3(cellSize, cellSize) * cellSize / 3;


        if (Settings.DEBUG_ENABLE)
        {

            debugArray = new TextMesh[cellsX, cellsY];
            for (int x = 0; x < cellsX; x++)
            {

                for (int y = 0; y < cellsY; y++)
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
                Debug.DrawLine(GetCellPosition(0, cellsX), GetCellPosition(cellsY, cellsX), Color.black, debugLineDuration);
                Debug.DrawLine(GetCellPosition(cellsY, 0), GetCellPosition(cellsY, cellsX), Color.black, debugLineDuration);
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
        }

        //Right
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            grid[grid.GetLength(0) - 1, i] = 1;
        }

        //Bottom
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            grid[i, 0] = 1;
        }

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            grid[i, grid.GetLength(1) - 1] = 1;
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
        Vector3 cellPosition = new Vector3(x, y) * cellSize + gridOriginPosition;
        return new Vector3(cellPosition.x, cellPosition.y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }

    private void SetValue(Vector3 position, int value)
    {
        Vector2Int pos = Util.GetXYInGameMap(position);
        SetValue(pos.x, pos.y, value);
    }

    private bool IsInsideGridLimit(float x, float y)
    {
        return (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1));
    }

    // For setting objects position with offset
    public Vector3 GetCellPositionWithOffset(int x, int y)
    {
        return GetCellPosition(x, y) + cellOffset;
    }

    // Updating Items on the grid
    public void UpdateObjectPosition(GameItemController obj, int width, int height)
    {
        int startX = (int)obj.GetX() - Mathf.FloorToInt(width / 2); // obj.GetX() == CenterX ,   CenterX - width / 2 
        int startY = (int)obj.GetY() - Mathf.FloorToInt(height / 2); // obj.GetY() == CenterY ,   CenterY - width / 2 

        for (int i = 0; i <= width; i++)
        {
            for (int j = 0; j <= height; j++)
            {
                SetGridObstacle(startX + i, startY + j, obj.GetType(), Color.black);
            }
        }
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

    // Gets the world cell value in Grid position
    public Vector3 GetCellPosition(Vector3 position)
    {
        Vector3 cellPosition = position * cellSize + originPosition;
        return new Vector3(cellPosition.x, cellPosition.y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }

    // In GameMap/Grid coordinates This sets the obstacle points around the obstacle
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
    public void SetTestGridObstacles(int row, int x1, int x2)
    {
        //int x, int y, ObjectType type, Color? color = null
        for (int i = x1; i <= x2; i++)
        {
            SetGridObstacle(row, i, ObjectType.OBSTACLE, Color.black);
        }
    }

    // Only for unit test use
    public void FreeTestGridObstacles(int row, int x1, int x2)
    {
        for (int i = x1; i <= x2; i++)
        {
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