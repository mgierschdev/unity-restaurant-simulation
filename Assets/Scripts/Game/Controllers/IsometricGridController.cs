using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IsometricGridController : MonoBehaviour
{
    private float cellSize = Settings.GRID_CELL_SIZE;
    private Vector3 cellOffset;
    private int[,] grid;

    // Path Finder object, contains the method to return the shortest path
    PathFind pathFind;

    private TextMesh[,] debugArray;
    private Vector3 gridOriginPosition = new Vector3(Settings.GRID_START_X, Settings.GRID_START_Y, Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL);

    // Debug Parameters
    private readonly int debugLineDuration = Settings.DEBUG_DEBUG_LINE_DURATION; // in seconds
    private readonly int cellTexttSize = Settings.DEBUG_TEXT_SIZE;
    private Vector3 textOffset;

    //Tilemap
    private Tilemap tilemap;

    void Awake()
    {
        int cellsX = (int)Settings.GRID_WIDTH;
        int cellsY = (int)Settings.GRID_HEIGHT;

        grid = new int[cellsX, cellsY];
        pathFind = new PathFind();
        cellOffset = new Vector3(cellSize, cellSize) * cellSize / 2;
        textOffset = new Vector3(cellSize, cellSize) * cellSize / 3;
        debugArray = new TextMesh[cellsX, cellsY];

        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Debug.Log("x:" + x + " y:" + y + " tile:" + tile.name);
                }
                else
                {
                    // Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                }
            }
        }

        for (int x = 0; x < cellsX; x++)
        {
            for (int y = 0; y < cellsY; y++)
            {
                Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x, y + 1), Color.black, debugLineDuration);
                Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x + 1, y), Color.black, debugLineDuration);
                Color cellColor = Color.black;
                if (grid[x, y] == 2 || grid[x, y] == 1)
                {
                    cellColor = Color.yellow;
                }
                Vector3 textPosition = GetCellPosition(x, y) + cellOffset - textOffset;
                // Debug.Log(tilemap.;
                // TileBase tile = tilemap.GetTile(Vector3Int.FloorToInt(textPosition));

                debugArray[x, y] = Util.CreateTextObject(x + "," + y, gameObject, x + "," + y, textPosition, cellTexttSize, cellColor, TextAnchor.MiddleCenter, TextAlignment.Center);
            }
            Debug.DrawLine(GetCellPosition(0, cellsX), GetCellPosition(cellsY, cellsX), Color.black, debugLineDuration);
            Debug.DrawLine(GetCellPosition(cellsY, 0), GetCellPosition(cellsY, cellsX), Color.black, debugLineDuration);
        }
    }


    // Gets the world cell value in Grid position
    private Vector3 GetCellPosition(int x, int y)
    {
        Vector3 cellPosition = new Vector3(x, y) * cellSize + gridOriginPosition;
        return new Vector3(cellPosition.x, cellPosition.y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }

        // Gets the world cell value in Grid position
    public Vector3 GetCellPosition(Vector3 position)
    {
        Vector3 cellPosition = position * cellSize + new Vector3(gridOriginPosition.x, gridOriginPosition.y, 0);
        return new Vector3(cellPosition.x, cellPosition.y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }


    private bool IsCoordsValid(float x, float y)
    {
        return (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1));
    }

}
