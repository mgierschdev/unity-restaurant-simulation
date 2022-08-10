using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using CodiceApp;

// This controlls the isometric tiles on the grid
public class IsometricGridController : MonoBehaviour
{
    //Tilemap 
    // Isometric Grid with pathfinding
    private GameTile gridTile; //The gameTile used to build the grid
    private Tilemap tilemapPathFinding;
    private List<GameTile> listPathFindingMap;
    private Dictionary<Vector3, GameTile> mapPathFinding;
    private Vector3Int gridOriginPosition = new Vector3Int(Settings.GRID_START_X, Settings.GRID_START_Y, Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL);

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

    //GameGrid Controller
    private GameGridController gridController;

    void Awake()
    {
        tilemapPathFinding = GameObject.Find(Settings.PATH_FINDING_GRID).GetComponent<Tilemap>();
        mapPathFinding = new Dictionary<Vector3, GameTile>();
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

        // Loading game grid controller
        gridController = transform.GetComponent<GameGridController>();

        if (tilemapFloor == null || tilemapColliders == null || tilemapObjects == null)
        {
            Debug.LogWarning("IsometricGridController/tilemap null");
        }

        LoadTileMap(listFloorTileMap, tilemapFloor, mapFloor);
        LoadTileMap(listCollidersTileMap, tilemapColliders, mapColliders);
        LoadTileMap(listObjectsTileMap, tilemapObjects, mapObjects);
        LoadTileMap(listPathFindingMap, tilemapPathFinding, mapPathFinding);

        //Loading colliders into the grid
        LoadColliders();
    }

    void Start()
    {
        BuildGrid();
    }

    public void BuildGrid()
    {
        if (Settings.DEBUG_ENABLE)
        {
            tilemapPathFinding.color = new Color(1, 1, 1, 0.0f);
        }

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                //   Debug.Log("Setting tileMap floor ");
                tilemapPathFinding.SetTile(new Vector3Int(i + gridOriginPosition.x, j + gridOriginPosition.y, 0), gridTile.UnityTileBase);
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
                GameTile gameTile = new GameTile(placeInWorld, Util.GetTileType(tile.name), Util.GetTileObjectType(Util.GetTileType(tile.name)), tile);
                list.Add(gameTile);
                map.TryAdd(gameTile.GridPosition, gameTile);

                if (Util.GetTileType(tile.name) == TileType.ISOMETRIC_GRID_TILE)
                {
                    gridTile = gameTile;
                }

                if (Settings.DEBUG_ENABLE)
                {
                    Debug.Log("Tile " + gameTile.GridPosition + " " + gameTile.WorldPosition + " " + gameTile.Type + " (" + tile.name + ")");
                }
            }
        }
    }

    private void LoadColliders()
    {
        foreach (GameTile tile in listCollidersTileMap)
        {
            gridController.SetIsometricGameTileCollider(tile);
        }
    }
}
