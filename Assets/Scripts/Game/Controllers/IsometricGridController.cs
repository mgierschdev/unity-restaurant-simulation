using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

// This controlls the isometric tiles on the grid
public class IsometricGridController : MonoBehaviour
{
    //Tilemap 

    private TileBase gridTile; //The gameTile used to build the grid
    private int width = 30;
    private int heigth = 35;

    // Isometric Grid with pathfinding
    private Tilemap tilemapPathFinding;
    private List<GameTile> listPathFindingMap;
    private Dictionary<Vector3, GameTile> mapWorldPositionToTile; // World Position to tile
    private Dictionary<Vector3Int, GameTile> mapGridPositionToTile; // Local Grid Position to tile
    private Dictionary<Vector3Int, GameTile> mapPathFindingGrid; // PathFinding Grid to tile
    private Vector3Int gridOriginPosition = new Vector3Int(-20, -20, Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL);

    // Path Finder object, contains the method to return the shortest path
    PathFind pathFind;
    private int[,] grid;
    private TextMesh[,] debugGrid;

    //Floor
    private Tilemap tilemapFloor;
    private List<GameTile> listFloorTileMap;
    private Dictionary<Vector3, GameTile> mapFloor;

    //Colliders
    private Tilemap tilemapColliders;
    private List<GameTile> listCollidersTileMap;
    private Dictionary<Vector3, GameTile> mapColliders;

    //Objects
    private Tilemap tilemapObjects;
    private List<GameTile> listObjectsTileMap;
    private Dictionary<Vector3, GameTile> mapObjects;

    private void Awake()
    {
        tilemapPathFinding = GameObject.Find(Settings.PATH_FINDING_GRID).GetComponent<Tilemap>();
        mapWorldPositionToTile = new Dictionary<Vector3, GameTile>();
        mapGridPositionToTile = new Dictionary<Vector3Int, GameTile>();
        mapPathFindingGrid = new Dictionary<Vector3Int, GameTile>();
        listPathFindingMap = new List<GameTile>();

        tilemapFloor = GameObject.Find(Settings.TILEMAP_FLOOR_0).GetComponent<Tilemap>();
        mapFloor = new Dictionary<Vector3, GameTile>();
        listFloorTileMap = new List<GameTile>();

        tilemapColliders = GameObject.Find(Settings.TILEMAP_COLLIDERS).GetComponent<Tilemap>();
        mapColliders = new Dictionary<Vector3, GameTile>();
        listCollidersTileMap = new List<GameTile>();

        tilemapObjects = GameObject.Find(Settings.TILEMAP_OBJECTS).GetComponent<Tilemap>();
        mapObjects = new Dictionary<Vector3, GameTile>();
        listObjectsTileMap = new List<GameTile>();

        if (tilemapFloor == null || tilemapColliders == null || tilemapObjects == null)
        {
            Debug.LogWarning("IsometricGridController/tilemap null");
        }

        pathFind = new PathFind();
        int cellsX = (int)Settings.GRID_WIDTH;
        int cellsY = (int)Settings.GRID_HEIGHT;
        grid = new int[cellsX, cellsY];
        debugGrid = new TextMesh[cellsX, cellsY];

        BuildGrid(listPathFindingMap, mapWorldPositionToTile, mapGridPositionToTile, mapPathFindingGrid); // We need to load the gridTile.UnityTileBase to build first. Which is on the FloorTileMap.
        LoadTileMap(listFloorTileMap, tilemapFloor, mapFloor);
        LoadTileMap(listCollidersTileMap, tilemapColliders, mapColliders);
        LoadTileMap(listObjectsTileMap, tilemapObjects, mapObjects);
    }

    private void DrawCellCoords()
    {
        tilemapPathFinding.color = new Color(1, 1, 1, 0.2f);

        foreach (GameTile tile in listPathFindingMap)
        {
            debugGrid[tile.GridPosition.x, tile.GridPosition.y] = Util.CreateTextObject(tile.GridPosition.x + "," + tile.GridPosition.y, gameObject, "(" + tile.GridPosition.x + "," + tile.GridPosition.y + ") " + tile.WorldPosition.x + "," + tile.WorldPosition.y, tile.WorldPosition, Settings.DEBUG_TEXT_SIZE, Color.black, TextAnchor.MiddleCenter, TextAlignment.Center);
        }
    }

    private void SetCellColor(int x, int y, Color color)
    {
        debugGrid[x, y].color = (Color)color;
    }

    private void BuildGrid(List<GameTile> list, Dictionary<Vector3, GameTile> mapWorldPositionToTile, Dictionary<Vector3Int, GameTile> mapGridPositionToTile, Dictionary<Vector3Int, GameTile> mapPathFindingGrid)
    {
        TileBase gridTile = tilemapPathFinding.GetTile(new Vector3Int(-12, 28));

        //  Debug.Log(tilemapPathFinding)
        for (int x = 0; x <= heigth; x++)
        {
            for (int y = 0; y <= width; y++)
            {
                Vector3Int positionInGrid = new Vector3Int(x + gridOriginPosition.x, y + gridOriginPosition.y, 0);
                Vector3 positionInWorld = tilemapPathFinding.CellToWorld(positionInGrid);
                Vector3Int positionLocalGrid = tilemapPathFinding.WorldToCell(positionInWorld);

                GameTile gameTile = new GameTile(positionInWorld, new Vector3Int(x, y), positionLocalGrid, Util.GetTileType(gridTile.name), Util.GetTileObjectType(Util.GetTileType(gridTile.name)), gridTile);
                list.Add(gameTile);
                mapWorldPositionToTile.TryAdd(gameTile.WorldPosition, gameTile);
                mapPathFindingGrid.TryAdd(gameTile.GridPosition, gameTile);
                mapGridPositionToTile.TryAdd(gameTile.LocalGridPosition, gameTile);
                tilemapPathFinding.SetTile(new Vector3Int(x + gridOriginPosition.x, y + gridOriginPosition.y, 0), gridTile);
            }
        }

        if (Settings.DEBUG_ENABLE)
        {
            DrawCellCoords();
        }
    }

    private void LoadTileMap(List<GameTile> list, Tilemap tilemap, Dictionary<Vector3, GameTile> map)
    {
        foreach (Vector3 pos in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = Vector3Int.FloorToInt(pos);
            Vector3 placeInWorld = tilemap.CellToWorld(localPlace);

            if (tilemap.HasTile(localPlace))
            {
                TileBase tile = tilemap.GetTile(localPlace);
                Vector3Int gridPosition = GetPathFindingGridFromWorldPosition(placeInWorld);
                GameTile gameTile = new GameTile(placeInWorld, gridPosition, GetLocalGridFromWorldPosition(placeInWorld), Util.GetTileType(tile.name), Util.GetTileObjectType(Util.GetTileType(tile.name)), tile);
                list.Add(gameTile);
                map.TryAdd(gameTile.WorldPosition, gameTile);

                if (Util.GetTileType(tile.name) == TileType.FLOOR_OBSTACLE)
                {
                    SetIsometricGameTileCollider(gameTile);
                }

                if (Settings.DEBUG_ENABLE)
                {
                   // Debug.Log("Tile grid: " + gameTile.GridPosition + " world: " + gameTile.WorldPosition + " " + gameTile.Type + " (" + tile.name + ")");
                }
            }
        }
    }

    //Default for 0.25 tile cell
    public void SetIsometricGameTileCollider(GameTile tile)
    {
        SetGridObstacle((int)tile.GridPosition.x, (int)tile.GridPosition.y, tile.Type, Color.blue);
        SetGridObstacle((int)tile.GridPosition.x + 1, (int)tile.GridPosition.y, tile.Type, Color.blue);
        SetGridObstacle((int)tile.GridPosition.x + 1, (int)tile.GridPosition.y + 1, tile.Type, Color.blue);
        SetGridObstacle((int)tile.GridPosition.x, (int)tile.GridPosition.y + 1, tile.Type, Color.blue);
    }


    // In GameMap/Grid coordinates This sets the obstacle points around the obstacle
    private void SetGridObstacle(int x, int y, ObjectType type, Color color)
    {
        if (color == null)
        {
            color = Color.blue;
        }

        if (!IsCoordsValid(x, y) || x < 0 || y < 0)
        {
            if (Settings.DEBUG_ENABLE)
            {
                Debug.LogWarning("The object should be placed inside the perimeter");
            }

            return;
        }

        if (ObjectType.OBSTACLE == type)
        {
            grid[x, y] = (int)type;
        }
        else
        {
            grid[x, y] = (int)type;
        }

        if (ObjectType.OBSTACLE == type)
        {
            SetCellColor(x, y, color);
        }
    }


    private bool IsCoordsValid(float x, float y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }

    public List<Node> GetPath(int[] start, int[] end)
    {
        return pathFind.Find(start, end, grid);
    }

    // Returns the Grid position given a Vector3 world position
    public Vector3Int GetLocalGridFromWorldPosition(Vector3 position)
    {
        Debug.Log("GetGridFromWorldPosition " + tilemapPathFinding.WorldToCell(position));
        return tilemapPathFinding.WorldToCell(position);

        // if (mapWorldPositionToTile.ContainsKey(position))
        // {
        //     GameTile tile = mapWorldPositionToTile[position];
        //     return new Vector2Int(tile.GridPosition.x, tile.GridPosition.y);
        // }
        // else
        // {
        //     Debug.LogError("GetGridFromWorldPosition / Position " + position + " not found");
        // }

        // return Vector2Int.zero;
    }

    public Vector3Int GetPathFindingGridFromWorldPosition(Vector3 position)
    {
        GameTile tile = mapGridPositionToTile[tilemapPathFinding.WorldToCell(position)];
        if (tile == null)
        {
            Debug.LogError("Tile null in GetPathFindingGridFromWorldPosition");
        }
        return tile.GridPosition;
    }

    public Vector3 GetWorldFromPathFindingGridPosition(Vector3Int position)
    {
        GameTile tile = mapPathFindingGrid[position];
        return tile.WorldPosition;
    }

    public Vector3 GetWorldFromGridPosition(Vector3Int position)
    {
        return tilemapPathFinding.CellToWorld(position);

        // if (mapGridPositionToTile.ContainsKey(position))
        // {
        //     GameTile tile = mapGridPositionToTile[position];
        //     return tile.WorldPosition;
        // }
        // else
        // {
        //     Debug.LogError("GetWorldFromGridPosition / Position " + position + " not found");
        // }
        // return Vector3.zero;
    }
}
