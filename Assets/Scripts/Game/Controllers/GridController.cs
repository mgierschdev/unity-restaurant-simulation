using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

// This controlls the isometric tiles on the grid
public class GridController : MonoBehaviour
{
    //Tilemap 
    private int width = Settings.GRID_WIDTH; // Down -> Up
    private int heigth = Settings.GRID_HEIGHT; // along side from left to right
    //x = -20, y= -22 ||  x along side left to right
    private Vector3Int gridOriginPosition = new Vector3Int(Settings.GRID_START_X, Settings.GRID_START_Y, Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL);

    // Isometric Grid with pathfinding
    private Tilemap tilemapPathFinding;
    [SerializeField]
    private List<GameTile> listPathFindingMap;
    [SerializeField]
    private Dictionary<Vector3, GameTile> mapWorldPositionToTile; // World Position to tile
    [SerializeField]
    private Dictionary<Vector3Int, GameTile> mapGridPositionToTile; // Local Grid Position to tile
    [SerializeField]
    private Dictionary<Vector3Int, GameTile> mapPathFindingGrid; // PathFinding Grid to tile

    // Spam Points list
    List<GameTile> spamPoints;

    // Path Finder object, contains the method to return the shortest path
    PathFind pathFind;
    [SerializeField]
    private int[,] grid;
    [SerializeField]
    private TextMesh[,] debugGrid;

    //Floor
    private Tilemap tilemapFloor;
    private List<GameTile> listFloorTileMap;
    private Dictionary<Vector3, GameTile> mapFloor;

    //WalkingPath 
    private Tilemap tilemapWalkingPath;
    private List<GameTile> listWalkingPathileMap;
    private Dictionary<Vector3, GameTile> mapWalkingPath;

    //Colliders
    [SerializeField]
    private Tilemap tilemapColliders;
    [SerializeField]
    private List<GameTile> listCollidersTileMap;
    [SerializeField]
    private Dictionary<Vector3, GameTile> mapColliders;

    //Objects
    [SerializeField]
    private Tilemap tilemapObjects;
    [SerializeField]
    private List<GameTile> listObjectsTileMap;
    [SerializeField]
    private Dictionary<Vector3, GameTile> mapObjects;

    //Prefabs in the TilemapObjects
    private List<GameGridObject> listGamePrefabs;
    private Queue<GameGridObject> FreeBusinessSpots { get; set; } // Tables to attend or chairs
    private Queue<GameGridObject> TablesWithClient { get; set; } // Tables to attend or chairs
    public GameGridObject Counter { get; set; }
    [SerializeField]
    private Dictionary<string, GameGridObject> mapGamePrefabs; //In PathfindingGrid pos

    //Business floor
    private Tilemap tilemapBusinessFloor;
    private List<GameTile> listBusinessFloor;
    private Dictionary<Vector3, GameTile> mapBusinessFloor;
    private bool isMouseHoverActive;

    private void Awake()
    {
        tilemapPathFinding = GameObject.Find(Settings.PATH_FINDING_GRID).GetComponent<Tilemap>();
        mapWorldPositionToTile = new Dictionary<Vector3, GameTile>();
        mapGridPositionToTile = new Dictionary<Vector3Int, GameTile>();
        mapPathFindingGrid = new Dictionary<Vector3Int, GameTile>();
        listPathFindingMap = new List<GameTile>();
        spamPoints = new List<GameTile>();

        tilemapFloor = GameObject.Find(Settings.TILEMAP_FLOOR_0).GetComponent<Tilemap>();
        mapFloor = new Dictionary<Vector3, GameTile>();
        listFloorTileMap = new List<GameTile>();

        tilemapColliders = GameObject.Find(Settings.TILEMAP_COLLIDERS).GetComponent<Tilemap>();
        mapColliders = new Dictionary<Vector3, GameTile>();
        listCollidersTileMap = new List<GameTile>();

        tilemapObjects = GameObject.Find(Settings.TILEMAP_OBJECTS).GetComponent<Tilemap>();
        mapObjects = new Dictionary<Vector3, GameTile>();
        listObjectsTileMap = new List<GameTile>();
        listGamePrefabs = new List<GameGridObject>();
        mapGamePrefabs = new Dictionary<string, GameGridObject>();
        FreeBusinessSpots = new Queue<GameGridObject>();
        TablesWithClient = new Queue<GameGridObject>();

        tilemapWalkingPath = GameObject.Find(Settings.TILEMAP_WALKING_PATH).GetComponent<Tilemap>();
        mapWalkingPath = new Dictionary<Vector3, GameTile>();
        listWalkingPathileMap = new List<GameTile>();

        tilemapBusinessFloor = GameObject.Find(Settings.TILEMAP_BUSINESS_FLOOR).GetComponent<Tilemap>();
        listBusinessFloor = new List<GameTile>();
        mapBusinessFloor = new Dictionary<Vector3, GameTile>(); ;

        if (tilemapFloor == null || tilemapColliders == null || tilemapObjects == null || tilemapPathFinding == null || tilemapWalkingPath == null || tilemapBusinessFloor == null)
        {
            Debug.LogWarning("GridController/tilemap");
            Debug.LogWarning("tilemapFloor " + tilemapFloor);
            Debug.LogWarning("tilemapColliders " + tilemapColliders);
            Debug.LogWarning("tilemapObjects " + tilemapObjects);
            Debug.LogWarning("tilemapPathFinding " + tilemapPathFinding);
            Debug.LogWarning("tilemapWalkingPath " + tilemapWalkingPath);
            Debug.LogWarning("tilemapBusinessFloor " + tilemapBusinessFloor);
        }

        if (Settings.DEBUG_ENABLE)
        {
            tilemapPathFinding.color = new Color(1, 1, 1, 0.0f);
            tilemapColliders.color = new Color(1, 1, 1, 0.0f);
            tilemapWalkingPath.color = new Color(1, 1, 1, 0.0f);
            tilemapBusinessFloor.color = new Color(1, 1, 1, 0.0f);
        }

        pathFind = new PathFind();
        grid = new int[Settings.GRID_HEIGHT, Settings.GRID_WIDTH];
        debugGrid = new TextMesh[Settings.GRID_HEIGHT, Settings.GRID_WIDTH];
        isMouseHoverActive = false;

        InitGrid(grid);
        BuildGrid(listPathFindingMap, mapWorldPositionToTile, mapGridPositionToTile, mapPathFindingGrid); // We need to load the gridTile.UnityTileBase to build first. Which is on the FloorTileMap.
        LoadTileMap(listFloorTileMap, tilemapFloor, mapFloor);
        LoadTileMap(listCollidersTileMap, tilemapColliders, mapColliders);
        LoadTileMap(listObjectsTileMap, tilemapObjects, mapObjects);
        LoadTileMap(listWalkingPathileMap, tilemapWalkingPath, mapWalkingPath);
        LoadTileMap(listBusinessFloor, tilemapBusinessFloor, mapBusinessFloor);
    }

    private void Update()
    {
        if (isMouseHoverActive)
        {
            MouseHover();
        }
    }

    public void HighlightGridBussFloor()
    {
        // If we Highlight we are in edit mode
        tilemapBusinessFloor.color = new Color(1, 1, 1, 0.5f);
        isMouseHoverActive = true;
    }

    private void MouseHover()
    {
        Vector3 mousePosition = Util.GetMouseInWorldPosition();
        Vector3Int mouseInGridPosition = GetPathFindingGridFromWorldPosition(mousePosition);
        GameTile tile = mapBusinessFloor[mousePosition];
        

       // SetCellColor(mouseInGridPosition.x, mouseInGridPosition.y, transParentRed);
    }

    public void HideGridBussFloor()
    {
        tilemapBusinessFloor.color = new Color(1, 1, 1, 0.0f);
        isMouseHoverActive = false;
    }

    private void DrawCellCoords()
    {
        foreach (GameTile tile in listPathFindingMap)
        {
            if (tile.GridPosition.x >= grid.GetLength(0) || tile.GridPosition.y >= grid.GetLength(1))
            {
                continue;
            }
            debugGrid[tile.GridPosition.x, tile.GridPosition.y] = Util.CreateTextObject(tile.GridPosition.x + "," + tile.GridPosition.y, gameObject, "(" + tile.GridPosition.x + "," + tile.GridPosition.y + ") " + tile.WorldPosition.x + "," + tile.WorldPosition.y, tile.WorldPosition, Settings.DEBUG_TEXT_SIZE, Color.black, TextAnchor.MiddleCenter, TextAlignment.Center);
        }
    }

    private void InitGrid(int[,] grid)
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = 1;
            }
        }
    }

    private void SetIsometricCellSolor(int x, int y, Color color)
    {
        SetCellColor(x, y, color);
        SetCellColor(x + 1, y, color);
        SetCellColor(x + 1, y + 1, color);
        SetCellColor(x, y + 1, color);
    }

    private void SetCellColor(int x, int y, Color color)
    {
        debugGrid[x, y].color = (Color)color;
    }

    private void BuildGrid(List<GameTile> list, Dictionary<Vector3, GameTile> mapWorldPositionToTile, Dictionary<Vector3Int, GameTile> mapGridPositionToTile, Dictionary<Vector3Int, GameTile> mapPathFindingGrid)
    {
        TileBase gridTile = Resources.Load<Tile>(Settings.GRID_TILES_SIMPLE);

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

                if (Settings.DEBUG_ENABLE)
                {
                    //Debug.Log("DEBUG: GridCell map "+gameTile.WorldPosition + " " + gameTile.GridPosition + " " + gameTile.LocalGridPosition + " " + gameTile.GetWorldPositionWithOffset());
                }
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
                TileType tileType = Util.GetTileType(tile.name);
                GameTile gameTile = new GameTile(placeInWorld, gridPosition, GetLocalGridFromWorldPosition(placeInWorld), tileType, Util.GetTileObjectType(Util.GetTileType(tile.name)), tile);
                list.Add(gameTile);
                map.TryAdd(gameTile.WorldPosition, gameTile);

                if (tileType == TileType.FLOOR_OBSTACLE)
                {
                    SetIsometricGameTileCollider(gameTile);
                }

                if (tileType == TileType.ISOMETRIC_GRID_TILE)
                {
                    SetIsometricGameTileCollider(gameTile);
                }

                if (tileType == TileType.WALKABLE_PATH)
                {
                    grid[gridPosition.x, gridPosition.y] = 0;

                    if (Settings.DEBUG_ENABLE)
                    {
                        SetIsometricCellSolor(gridPosition.x, gridPosition.y, Color.white);
                    }
                }

                if (tileType == TileType.BUS_FLOOR)
                {
                    grid[gridPosition.x, gridPosition.y] = 0;

                    if (Settings.DEBUG_ENABLE)
                    {
                        SetIsometricCellSolor(gridPosition.x, gridPosition.y, Color.white);
                    }
                }

                if (tileType == TileType.SPAM_POINT)
                {
                    spamPoints.Add(gameTile);
                }
            }
        }
    }

    //Gets a GameTIle in Camera.main.ScreenToWorldPoint(Input.mousePosition))      
    public GameTile GetGameTileFromClickInPathFindingGrid(Vector3Int position)
    {
        if (mapPathFindingGrid.ContainsKey(position))
        {
            return mapPathFindingGrid[position];
        }
        else
        {
            Debug.LogWarning("IsometricGrid/GetGameTileFromClickInWorldPosition null");
            return null;
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

        if (ObjectType.OBSTACLE == type && Settings.DEBUG_ENABLE)
        {
            SetCellColor(x, y, color);
        }
    }

    //Sets 1 isometric cell
    private void SetGridObstacle(Vector3Int pos)
    {
        grid[pos.x, pos.y] = 1;
        grid[pos.x + 1, pos.y] = 1;
        grid[pos.x + 1, pos.y + 1] = 1;
        grid[pos.x, pos.y + 1] = 1;
        SetCellColor(pos.x, pos.y, Color.blue);
        SetCellColor(pos.x + 1, pos.y, Color.blue);
        SetCellColor(pos.x + 1, pos.y + 1, Color.blue);
        SetCellColor(pos.x, pos.y + 1, Color.blue);
    }

    private bool IsCoordsValid(float x, float y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }

    public List<Node> GetPath(int[] start, int[] end)
    {
        if (grid[start[0], start[1]] == 1 || grid[end[0], end[1]] == 1)
        {
            return new List<Node>();
        }
        return pathFind.Find(start, end, grid);
    }

    // Returns the Grid position given a Vector3 world position
    public Vector3Int GetLocalGridFromWorldPosition(Vector3 position)
    {
        return tilemapPathFinding.WorldToCell(position);
    }

    public Vector3Int GetPathFindingGridFromWorldPosition(Vector3 position)
    {
        if (!mapGridPositionToTile.ContainsKey(tilemapPathFinding.WorldToCell(position)))
        {
            Debug.LogError("GetPathFindingGridFromWorldPosition/ mapGridPositionToTile does not contain the key " + position + "/" + tilemapPathFinding.WorldToCell(position));
        }
        else
        {
            GameTile tile = mapGridPositionToTile[tilemapPathFinding.WorldToCell(position)];
            return tile.GridPosition;
        }

        return Vector3Int.zero;
    }

    public Vector3 GetWorldFromPathFindingGridPosition(Vector3Int position)
    {
        GameTile tile = mapPathFindingGrid[position];
        return tile.GetWorldPositionWithOffset();
    }

    public Vector3 GetWorldFromGridPosition(Vector3Int position)
    {
        return tilemapPathFinding.CellToWorld(position);
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

    public Vector3Int GetRandomWalkableGridPosition()
    {
        if (listWalkingPathileMap.Count == 0)
        {
            Debug.LogWarning("There is not listWalkingPathileMap points");
            return Vector3Int.zero;
        }
        GameTile tile = listWalkingPathileMap[Random.Range(0, listWalkingPathileMap.Count)];
        return tile.GridPosition;

    }
    public GameTile GetRandomSpamPointWorldPosition()
    {
        if (spamPoints.Count == 0)
        {
            Debug.LogWarning("There is not spam points");
        }

        GameTile tile = spamPoints[Random.Range(0, spamPoints.Count)];
        return tile;
    }

    // Unset position in Grid
    public void FreeGridPosition(int x, int y)
    {
        if (!IsCoordsValid(x, y) && x > 1 && y > 1)
        {
            if (Settings.DEBUG_ENABLE)
            {
                Debug.LogError("The object should be placed inside the perimeter");
            }
            return;
        }

        grid[x, y] = 0;
        if (Settings.DEBUG_ENABLE)
        {
            SetCellColor(x, y, Color.white);
        }
    }

    public void SetGridObject(GameGridObject obj)
    {

        if (obj.Type == ObjectType.NPC_COUNTER)
        {
            Counter = obj;
        }

        listGamePrefabs.Add(obj);
        mapGamePrefabs.Add(obj.Name, obj);
        SetObjectObstacle(obj);
    }

    private void SetObjectObstacle(GameGridObject obj)
    {
        if (obj.Type == ObjectType.NPC_TABLE)
        {
            FreeBusinessSpots.Enqueue(obj);
            grid[obj.GridPosition.x, obj.GridPosition.y] = 1;
            if (Settings.DEBUG_ENABLE)
            {
                SetCellColor(obj.GridPosition.x, obj.GridPosition.y, Color.blue);
            }
        }
        else if (obj.TileType == TileType.ISOMETRIC_SINGLE_SQUARE_OBJECT)
        {
            grid[obj.GridPosition.x, obj.GridPosition.y] = 1;
            if (Settings.DEBUG_ENABLE)
            {
                SetCellColor(obj.GridPosition.x, obj.GridPosition.y, Color.blue);
            }
        }
    }

    public GameGridObject GetFreeTable()
    {
        if (FreeBusinessSpots.Count > 0)
        {
            return FreeBusinessSpots.Dequeue();
        }
        return null;
    }

    public bool IsThereFreeTables()
    {
        return FreeBusinessSpots.Count > 0;
    }

    public bool IsThereCustomer()
    {
        return TablesWithClient.Count > 0;
    }

    public void FreeTable(string name)
    {
        FreeBusinessSpots.Enqueue(mapGamePrefabs[name]);
    }

    public void AddFreeBusinessSpots(GameGridObject obj)
    {
        FreeBusinessSpots.Enqueue(obj);
    }

    public GameGridObject GetTableWithClient()
    {
        if (TablesWithClient.Count > 0)
        {
            return TablesWithClient.Dequeue();
        }
        else
        {
            return null;
        }
    }

    public void AddClientToTable(GameGridObject obj)
    {
        TablesWithClient.Enqueue(obj);
    }
}