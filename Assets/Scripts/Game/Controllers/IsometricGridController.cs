using UnityEngine;
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
    private Dictionary<Vector3, GameTile> mapPathFinding;
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

    //GameGrid Controller
    private GameGridController gridController;

    private void Awake()
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

        pathFind = new PathFind();
        int cellsX = (int)Settings.GRID_WIDTH;
        int cellsY = (int)Settings.GRID_HEIGHT;
        grid = new int[cellsX, cellsY];

        LoadTileMap(listFloorTileMap, tilemapFloor, mapFloor);
        BuildGrid(); // We need to load the gridTile.UnityTileBase to build first. Which is on the FloorTileMap.
        LoadTileMap(listCollidersTileMap, tilemapColliders, mapColliders);
        LoadTileMap(listObjectsTileMap, tilemapObjects, mapObjects);
        LoadTileMap(listPathFindingMap, tilemapPathFinding, mapPathFinding);
        LoadColliders(); //Loading colliders into the grid
    }

    public void Start()
    {
        if (Settings.DEBUG_ENABLE)
        {
            DrawCellCoords();
        }
    }

    private void Update()
    {
        MouseOnClick();
    }

    private void MouseOnClick()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Util.GetMouseInWorldPosition();

            Debug.DrawLine(new Vector3(0, 0), mousePosition, Color.yellow, 20f);
            // Vector2Int mouseInGridPosition = Util.GetXYInGameMap(mousePosition);
            // List<Node> path = GetPath(new int[] { (int)X, (int)Y }, new int[] { mouseInGridPosition.x, mouseInGridPosition.y });

        }

    }


    private void DrawCellCoords()
    {
        tilemapPathFinding.color = new Color(1, 1, 1, 0.5f);

        foreach (GameTile tile in listPathFindingMap)
        {
            Util.CreateTextObject(tile.WorldPosition.x + "," + tile.WorldPosition.y, gameObject, tile.WorldPosition.x + "," + tile.WorldPosition.y, tile.WorldPosition, Settings.DEBUG_TEXT_SIZE, Color.black, TextAnchor.MiddleCenter, TextAlignment.Center);

        }
    }

    private void BuildGrid()
    {
        //  Debug.Log(tilemapPathFinding)
        for (int x = 0; x < heigth; x++)
        {
            for (int y = 0; y < width; y++)
            {
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
                GameTile gameTile = new GameTile(placeInWorld, Util.GetTileType(tile.name), Util.GetTileObjectType(Util.GetTileType(tile.name)), tile);
                list.Add(gameTile);
                map.TryAdd(gameTile.WorldPosition, gameTile);

                if (Util.GetTileType(tile.name) == TileType.ISOMETRIC_GRID_TILE)
                {
                    gridTile = gameTile;
                }

                if (Settings.DEBUG_ENABLE)
                {
                    Debug.Log("Tile grid: " + gameTile.GridPosition + " world: " + gameTile.WorldPosition + " " + gameTile.Type + " (" + tile.name + ")");
                }
            }
        }
    }

    private void LoadColliders()
    {
        // foreach (GameTile tile in listCollidersTileMap)
        // {
        //     gridController.SetIsometricGameTileCollider(tile);
        // }
    }

    public List<Node> GetPath(int[] start, int[] end)
    {
        return pathFind.Find(start, end, grid);
    }

    public Vector3 GetCellPosition(Vector3 position)
    {
        return position;
    }
}
