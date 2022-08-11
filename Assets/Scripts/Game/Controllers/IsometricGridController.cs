using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

// This controlls the isometric tiles on the grid
public class IsometricGridController : MonoBehaviour
{
    //Tilemap 

    private GameTile gridTile; //The gameTile used to build the grid
    private int width = 30;
    private int heigth = 35;

    // Isometric Grid with pathfinding
    private Tilemap tilemapPathFinding;
    private List<GameTile> listPathFindingMap;
    private Dictionary<Vector3, GameTile> mapWorldPositionToTile;
    private Dictionary<Vector2Int, GameTile> mapGridPositionToTile;
    private Vector3Int gridOriginPosition = new Vector3Int(-20, -20, Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL);

    // Path Finder object, contains the method to return the shortest path
    PathFind pathFind;
    private int[,] grid;

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
        mapGridPositionToTile = new Dictionary<Vector2Int, GameTile>();
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

        LoadTileMap(listFloorTileMap, tilemapFloor, mapFloor);
        BuildGrid(listPathFindingMap, mapWorldPositionToTile, mapGridPositionToTile); // We need to load the gridTile.UnityTileBase to build first. Which is on the FloorTileMap.
        LoadTileMap(listCollidersTileMap, tilemapColliders, mapColliders);
        LoadTileMap(listObjectsTileMap, tilemapObjects, mapObjects);
    }

    public void Start()
    {
        if (Settings.DEBUG_ENABLE)
        {
            DrawCellCoords();
        }
    }

    private void DrawCellCoords()
    {
        tilemapPathFinding.color = new Color(1, 1, 1, 0.2f);

        foreach (GameTile tile in listPathFindingMap)
        {
            Util.CreateTextObject(tile.GridPosition.x + "," + tile.GridPosition.y, gameObject, "(" + tile.GridPosition.x + "," + tile.GridPosition.y + ") " + tile.WorldPosition.x + "," + tile.WorldPosition.y, tile.WorldPosition, Settings.DEBUG_TEXT_SIZE, Color.black, TextAnchor.MiddleCenter, TextAlignment.Center);

        }
    }

    private void BuildGrid(List<GameTile> list, Dictionary<Vector3, GameTile> mapWorldPositionToTile, Dictionary<Vector2Int, GameTile> mapGridPositionToTile)
    {
        //  Debug.Log(tilemapPathFinding)
        for (int x = 0; x < heigth; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Vector3Int positionInGrid = new Vector3Int(x + gridOriginPosition.x, y + gridOriginPosition.y, 0);
                Vector3 positionInWorld = tilemapPathFinding.GetCellCenterWorld(positionInGrid);
                GameTile gameTile = new GameTile(positionInWorld, new Vector2Int(x, y), Util.GetTileType(gridTile.UnityTileBase.name), Util.GetTileObjectType(Util.GetTileType(gridTile.UnityTileBase.name)), gridTile.UnityTileBase);
                list.Add(gameTile);
                mapWorldPositionToTile.TryAdd(gameTile.WorldPosition, gameTile);
                mapGridPositionToTile.TryAdd(gameTile.GridPosition, gameTile);
                tilemapPathFinding.SetTile(new Vector3Int(x + gridOriginPosition.x, y + gridOriginPosition.y, 0), gridTile.UnityTileBase);
            }
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
                Vector2Int gridPosition = GetGridFromWorldPosition(placeInWorld);
                GameTile gameTile = new GameTile(placeInWorld, gridPosition, Util.GetTileType(tile.name), Util.GetTileObjectType(Util.GetTileType(tile.name)), tile);
                list.Add(gameTile);
                map.TryAdd(gameTile.WorldPosition, gameTile);

                if (Util.GetTileType(tile.name) == TileType.ISOMETRIC_GRID_TILE)
                {
                    gridTile = gameTile;
                }

                if (Util.GetTileType(tile.name) == TileType.FLOOR_OBSTACLE)
                {

                    grid[][]
                }

                if (Settings.DEBUG_ENABLE)
                {
                    Debug.Log("Tile grid: " + gameTile.GridPosition + " world: " + gameTile.WorldPosition + " " + gameTile.Type + " (" + tile.name + ")");
                }
            }
        }
    }

    public List<Node> GetPath(int[] start, int[] end)
    {
        return pathFind.Find(start, end, grid);
    }

    // Returns the Grid position given a Vector3 world position
    public Vector2Int GetGridFromWorldPosition(Vector3 position)
    {
        Vector3Int intPos = new Vector3Int(
        (int)Math.Round(position.x, MidpointRounding.AwayFromZero),
        (int)Math.Round(position.y, MidpointRounding.AwayFromZero));

        GameTile tile = mapWorldPositionToTile[intPos];

        if (tile != null)
        {
            return new Vector2Int(tile.GridPosition.x, tile.GridPosition.y);
        }
        else
        {
            Debug.LogError("GetGridFromWorldPosition / Position " + position + " not found");
        }

        return Vector2Int.zero;
    }

    public Vector3 GetWorldFromGridPosition(Vector2Int position)
    {

        GameTile tile = mapGridPositionToTile[position];

        if (tile != null)
        {
            return tile.WorldPosition;
        }
        else
        {
            Debug.LogError("GetWorldFromGridPosition / Position " + position + " not found");
        }
        return Vector3.zero;
    }
}
