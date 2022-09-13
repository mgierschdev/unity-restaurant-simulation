using System.Collections.Generic;
using Game.Players;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GridController : MonoBehaviour
{
    //Player Data
    public PlayerData PlayerData { get; set; }
    //Tilemap 
    private const int WIDTH = Settings.GridWidth; // Down -> Up
    private const int HEIGHT = Settings.GridHeight; // along side from left to right x = -20, y= -22 ||  x along side left to right
    private Vector3Int gridOriginPosition = new Vector3Int(Settings.GridStartX, Settings.GrtGridStartY, Settings.ConstDefaultBackgroundOrderingLevel);
    // Isometric Grid with pathfinding
    private Tilemap tilemapPathFinding;
    private Dictionary<Vector3, GameTile> mapWorldPositionToTile; // World Position to tile
    private Dictionary<Vector3Int, GameTile> mapGridPositionToTile; // Local Grid Position to tile
    private Dictionary<Vector3Int, GameTile> mapPathFindingGrid; // PathFinding Grid to tile
    // Spam Points list
    private List<GameTile> spamPoints;
    // Path Finder object, contains the method to return the shortest path
    private PathFind pathFind;
    private int[,] grid;
    // private TextMesh[,] debugGrid;
    //Floor
    private Tilemap tilemapFloor;
    private List<GameTile> listFloorTileMap;
    private Dictionary<Vector3Int, GameTile> mapFloor;
    //WalkingPath 
    private Tilemap tilemapWalkingPath;
    private List<GameTile> listWalkingPathTileMap;
    private Dictionary<Vector3Int, GameTile> mapWalkingPath;
    //Colliders
    private Tilemap tilemapColliders;
    private List<GameTile> listCollidersTileMap;
    private Dictionary<Vector3Int, GameTile> mapColliders;
    //Objects
    private Tilemap tilemapObjects;
    private List<GameTile> listObjectsTileMap;
    private Dictionary<Vector3Int, GameTile> mapObjects;
    //Prefabs in the TilemapObjects
    private Dictionary<string, GameGridObject> businessObjects;
    private Dictionary<string, GameGridObject> BusyBusinessSpotsMap { get; set; }
    private Dictionary<string, GameGridObject> FreeBusinessSpotsMap { get; set; }
    private Queue<GameGridObject> FreeBusinessSpots { get; set; } // Tables to attend or chairs
    private Queue<GameGridObject> TablesWithClient { get; set; } // Tables to attend or chairs
    public GameGridObject Counter { get; set; }
    //Business floor
    private Tilemap tilemapBusinessFloor;
    private List<GameTile> listBusinessFloor;
    private Dictionary<Vector3Int, GameTile> mapBusinessFloor;
    private string currentClickedActiveGameObject;
    private int[,] arroundVectorPoints;
    public bool DraggingObject;

    private void Awake()
    {
        tilemapPathFinding = GameObject.Find(Settings.PathFindingGrid).GetComponent<Tilemap>();
        mapWorldPositionToTile = new Dictionary<Vector3, GameTile>();
        mapGridPositionToTile = new Dictionary<Vector3Int, GameTile>();
        mapPathFindingGrid = new Dictionary<Vector3Int, GameTile>();
        spamPoints = new List<GameTile>();

        tilemapFloor = GameObject.Find(Settings.TilemapFloor0).GetComponent<Tilemap>();
        mapFloor = new Dictionary<Vector3Int, GameTile>();
        listFloorTileMap = new List<GameTile>();

        tilemapColliders = GameObject.Find(Settings.TilemapColliders).GetComponent<Tilemap>();
        mapColliders = new Dictionary<Vector3Int, GameTile>();
        listCollidersTileMap = new List<GameTile>();

        tilemapObjects = GameObject.Find(Settings.TilemapObjects).GetComponent<Tilemap>();
        mapObjects = new Dictionary<Vector3Int, GameTile>();
        listObjectsTileMap = new List<GameTile>();
        FreeBusinessSpots = new Queue<GameGridObject>();
        TablesWithClient = new Queue<GameGridObject>();
        FreeBusinessSpotsMap = new Dictionary<string, GameGridObject>();
        BusyBusinessSpotsMap = new Dictionary<string, GameGridObject>();
        businessObjects = new Dictionary<string, GameGridObject>();

        tilemapWalkingPath = GameObject.Find(Settings.TilemapWalkingPath).GetComponent<Tilemap>();
        mapWalkingPath = new Dictionary<Vector3Int, GameTile>();
        listWalkingPathTileMap = new List<GameTile>();

        tilemapBusinessFloor = GameObject.Find(Settings.TilemapBusinessFloor).GetComponent<Tilemap>();
        listBusinessFloor = new List<GameTile>();
        mapBusinessFloor = new Dictionary<Vector3Int, GameTile>();

        if (tilemapFloor == null || tilemapColliders == null || tilemapObjects == null || tilemapPathFinding == null ||
            tilemapWalkingPath == null || tilemapBusinessFloor == null)
        {
            GameLog.LogWarning("GridController/tilemap");
            GameLog.LogWarning("tilemapFloor " + tilemapFloor);
            GameLog.LogWarning("tilemapColliders " + tilemapColliders);
            GameLog.LogWarning("tilemapObjects " + tilemapObjects);
            GameLog.LogWarning("tilemapPathFinding " + tilemapPathFinding);
            GameLog.LogWarning("tilemapWalkingPath " + tilemapWalkingPath);
            GameLog.LogWarning("tilemapBusinessFloor " + tilemapBusinessFloor);
        }

        tilemapPathFinding.color = new Color(1, 1, 1, 0.0f);
        tilemapColliders.color = new Color(1, 1, 1, 0.0f);
        tilemapWalkingPath.color = new Color(1, 1, 1, 0.0f);
        tilemapBusinessFloor.color = new Color(1, 1, 1, 0.0f);

        pathFind = new PathFind();
        grid = new int[Settings.GridHeight, Settings.GridWidth];
        // debugGrid = new TextMesh[Settings.GridHeight, Settings.GridWidth];
        currentClickedActiveGameObject = "";
        arroundVectorPoints = new int[,] { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };

        InitGrid();
        BuildGrid(); // We need to load the gridTile.UnityTileBase to build first. Which is on the FloorTileMap.
        LoadTileMap(listFloorTileMap, tilemapFloor, mapFloor);
        LoadTileMap(listCollidersTileMap, tilemapColliders, mapColliders);
        LoadTileMap(listObjectsTileMap, tilemapObjects, mapObjects);
        LoadTileMap(listWalkingPathTileMap, tilemapWalkingPath, mapWalkingPath);
        LoadTileMap(listBusinessFloor, tilemapBusinessFloor, mapBusinessFloor);
    }

    // private void MouseHover()
    // {
    //     Vector3 mousePosition = Util.GetMouseInWorldPosition();
    //     Vector3Int mouseInGridPosition = GetPathFindingGridFromWorldPosition(mousePosition);
    //
    //     if (!mapPathFindingGrid.ContainsKey(mouseInGridPosition))
    //     {
    //         GameLog.Log("Does not contain the position " + mouseInGridPosition);
    //         return;
    //     }
    //
    //     GameTile tile = mapPathFindingGrid[mouseInGridPosition];
    //     TileBase highLightedTile = Resources.Load<Tile>(Settings.GridTilesHighlightedFloor);
    //     tilemapBusinessFloor.SetTile(tile.LocalGridPosition, highLightedTile);
    //     // SetCellColor(mouseInGridPosition.x, mouseInGridPosition.y, transParentRed);
    // }

    // private void DrawCellCoords()
    // {
    //     foreach (GameTile tile in listPathFindingMap)
    //     {
    //         if (tile.GridPosition.x >= grid.GetLength(0) || tile.GridPosition.y >= grid.GetLength(1))
    //         {
    //             continue;
    //         }
    //         debugGrid[tile.GridPosition.x, tile.GridPosition.y] = Util.CreateTextObject(tile.GridPosition.x + "," + tile.GridPosition.y, gameObject,
    //             "(" + tile.GridPosition.x + "," + tile.GridPosition.y + ") " + tile.WorldPosition.x + "," +
    //             tile.WorldPosition.y, tile.WorldPosition, Settings.DebugTextSize, Color.black,
    //             TextAnchor.MiddleCenter, TextAlignment.Center);
    //     }
    // }

    private void InitGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = 1;
            }
        }
    }

    // private void SetIsometricCellColor(int x, int y, Color color)
    // {
    //     SetCellColor(x, y, color);
    //     SetCellColor(x + 1, y, color);
    //     SetCellColor(x + 1, y + 1, color);
    //     SetCellColor(x, y + 1, color);
    // }

    // private void SetCellColor(int x, int y, Color color)
    // {
    //     debugGrid[x, y].color = (Color)color;
    // }

    private void BuildGrid()
    {
        TileBase gridTile = Resources.Load<Tile>(Settings.GridTilesSimple);

        for (int x = 0; x <= HEIGHT; x++)
        {
            for (int y = 0; y <= WIDTH; y++)
            {
                Vector3Int positionInGrid = new Vector3Int(x + gridOriginPosition.x, y + gridOriginPosition.y, 0);
                Vector3 positionInWorld = tilemapPathFinding.CellToWorld(positionInGrid);
                Vector3Int positionLocalGrid = tilemapPathFinding.WorldToCell(positionInWorld);
                GameTile gameTile = new GameTile(positionInWorld, new Vector3Int(x, y), positionLocalGrid,
                    Util.GetTileType(gridTile.name), Util.GetTileObjectType(Util.GetTileType(gridTile.name)), gridTile);
                mapWorldPositionToTile.TryAdd(gameTile.WorldPosition, gameTile);
                mapPathFindingGrid.TryAdd(gameTile.GridPosition, gameTile);
                mapGridPositionToTile.TryAdd(gameTile.LocalGridPosition, gameTile);
                tilemapPathFinding.SetTile(new Vector3Int(x + gridOriginPosition.x, y + gridOriginPosition.y, 0),
                    gridTile);

                // if (Settings.DEBUG_ENABLE)
                // {
                //     //GameLog.Log("DEBUG: GridCell map "+gameTile.WorldPosition + " " + gameTile.GridPosition + " " + gameTile.LocalGridPosition + " " + gameTile.GetWorldPositionWithOffset());
                // }
            }
        }

        // if (Settings.DEBUG_ENABLE)
        // {
        //     DrawCellCoords();
        // }
    }

    private void LoadTileMap(List<GameTile> list, Tilemap tilemap, Dictionary<Vector3Int, GameTile> map)
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
                map.TryAdd(gridPosition, gameTile);

                if (tileType == TileType.WALKABLE_PATH)
                {
                    grid[gridPosition.x, gridPosition.y] = 0;
                }

                if (tileType == TileType.BUS_FLOOR)
                {
                    grid[gridPosition.x, gridPosition.y] = 0;
                }

                if (tileType == TileType.SPAM_POINT)
                {
                    spamPoints.Add(gameTile);
                }
            }
        }
    }

    // In GameMap/Grid coordinates This sets the obstacle points around the obstacle
    private void SetGridObstacle(int x, int y, ObjectType type)
    {
        if (!IsCoordsValid(x, y) || x < 0 || y < 0)
        {
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
    }

    private bool IsCoordsValid(int x, int y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }

    private void SetObjectObstacle(GameGridObject obj)
    {
        if (obj.Type == ObjectType.NPC_SINGLE_TABLE)
        {
            businessObjects.TryAdd(obj.Name, obj);
            FreeBusinessSpots.Enqueue(obj);
            FreeBusinessSpotsMap.Add(obj.Name, obj);
            grid[obj.GridPosition.x, obj.GridPosition.y] = 1;
            Vector3Int ActionGridPosition = GetPathFindingGridFromWorldPosition(obj.GetActionTile());
            grid[ActionGridPosition.x, ActionGridPosition.y] = -1;
        }
        else
        {
            grid[obj.GridPosition.x, obj.GridPosition.y] = 1;

            if (obj.Type == ObjectType.NPC_COUNTER)
            {
                Vector3Int ActionGridPosition = GetPathFindingGridFromWorldPosition(obj.GetActionTile());
                grid[ActionGridPosition.x, ActionGridPosition.y] = -1;
            }
        }
    }

    // worldPos = Current position of the object while dragging, actionTileOne: the initial actiontile in grid coord
    public bool IsValidBussPosition(GameGridObject initial, Vector3 worldPos, Vector3Int actionTileOne)
    {
        Vector3Int currentGridPos = GetPathFindingGridFromWorldPosition(worldPos);
        Vector3 currentActionPointWorldPos = initial.GetActionTile();
        Vector3Int currentActionPointInGrid = GetPathFindingGridFromWorldPosition(currentActionPointWorldPos);

        // If we are at the initial grid position we return true
        if (worldPos == initial.WorldPosition ||
            currentActionPointInGrid == actionTileOne ||
            currentActionPointInGrid == initial.GridPosition ||
            currentGridPos == actionTileOne
            )
        {
            return true;
        }

        // If the coords are ousite the perimter we return false, or if the position is different than 0
        if (!IsCoordsValid(currentGridPos.x, currentGridPos.y) ||
        !IsCoordsValid(currentActionPointInGrid.x, currentActionPointInGrid.y) ||
        grid[currentActionPointInGrid.x, currentActionPointInGrid.y] != 0 ||
        grid[currentGridPos.x, currentGridPos.y] != 0
        )
        {
            return false;
        }

        // if the current grid position is in the buss map we return trues
        if (mapBusinessFloor.ContainsKey(currentGridPos) &&
        mapBusinessFloor.ContainsKey(currentActionPointInGrid))
        {
            return true;
        }

        return false;
    }

    public void HideGridBussFloor()
    {
        if (currentClickedActiveGameObject != "")
        {
            GameGridObject gameGridObject = businessObjects[currentClickedActiveGameObject];
            gameGridObject.Hide();
            currentClickedActiveGameObject = "";
        }
        tilemapBusinessFloor.color = new Color(1, 1, 1, 0.0f);
    }

    //Gets a GameTIle in Camera.main.ScreenToWorldPoint(Input.mousePosition))      
    public GameTile GetGameTileFromClickInPathFindingGrid(Vector3Int position)
    {
        if (mapPathFindingGrid.ContainsKey(position))
        {
            return mapPathFindingGrid[position];
        }

        GameLog.LogWarning("IsometricGrid/GetGameTileFromClickInWorldPosition null");
        return null;
    }

    public void HighlightGridBussFloor()
    {
        // If we Highlight we are in edit mode
        tilemapBusinessFloor.color = new Color(1, 1, 1, 0.5f);
    }

    // Returns the nearest grid World position given any world map position
    public Vector3 GetNearestGridPositionFromWorldMap(Vector3 pos)
    {
        return GetWorldFromGridPosition(GetLocalGridFromWorldPosition(pos));
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
            GameLog.LogError("GetPathFindingGridFromWorldPosition/ mapGridPositionToTile does not contain the key " + position + "/" + tilemapPathFinding.WorldToCell(position));
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

    private Vector3 GetWorldFromGridPosition(Vector3Int position)
    {
        return tilemapPathFinding.CellToWorld(position);
    }

    // Only for unit test use
    public void SetTestGridObstacles(int row, int x1, int x2)
    {
        //int x, int y, ObjectType type, Color? color = null
        for (int i = x1; i <= x2; i++)
        {
            SetGridObstacle(row, i, ObjectType.OBSTACLE);
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
        if (listWalkingPathTileMap.Count == 0)
        {
            GameLog.LogWarning("There is not listWalkingPathTileMap points");
            return Vector3Int.zero;
        }

        GameTile tile = listWalkingPathTileMap[Random.Range(0, listWalkingPathTileMap.Count)];
        return tile.GridPosition;
    }

    public GameTile GetRandomSpamPointWorldPosition()
    {
        if (spamPoints.Count == 0)
        {
            GameLog.LogWarning("There is not spam points");
        }

        GameTile tile = spamPoints[Random.Range(0, spamPoints.Count)];
        return tile;
    }

    // Unset position in Grid
    private void FreeGridPosition(int x, int y)
    {
        if (!IsCoordsValid(x, y) && x > 1 && y > 1)
        {
            return;
        }

        grid[x, y] = 0;
    }

    public void SwapCoords(int x1, int y1, int x2, int y2)
    {
        GameLog.Log(" ");
        GameLog.Log("Swapping: " + grid[x1, y1] + " " + grid[x2, y2]);
        (grid[x1, y1], grid[x2, y2]) = (grid[x2, y2], grid[x1, y1]);
        GameLog.Log("Swapping: " + grid[x1, y1] + " " + grid[x2, y2]);
    }

    // called when the object is destroyed
    public void FreeObject(GameGridObject gameGridObject)
    {
        grid[gameGridObject.GridPosition.x, gameGridObject.GridPosition.y] = 0;
        Vector3Int gridActionTile = GetPathFindingGridFromWorldPosition(gameGridObject.GetActionTile());
        grid[gridActionTile.x, gridActionTile.y] = 0;
    }

    public void SetGridObject(GameGridObject obj)
    {
        if (obj.Type == ObjectType.NPC_COUNTER)
        {
            Counter = obj;
        }
        SetObjectObstacle(obj);
    }

    public GameGridObject GetFreeTable()
    {
        if (FreeBusinessSpots.Count <= 0)
        {
            return null;
        }

        BusyBusinessSpotsMap.Add(FreeBusinessSpots.Peek().Name, FreeBusinessSpots.Peek());
        FreeBusinessSpotsMap.Remove(FreeBusinessSpots.Peek().Name);
        return FreeBusinessSpots.Dequeue();
    }

    public bool IsThereFreeTables()
    {
        return FreeBusinessSpots.Count > 0;
    }

    public bool IsThereCustomer()
    {
        return TablesWithClient.Count > 0;
    }

    public bool IsTableInFreeBussSpot(GameGridObject obj)
    {
        return FreeBusinessSpotsMap.ContainsKey(obj.Name);
    }

    public bool IsTableBusy(GameGridObject obj)
    {
        return BusyBusinessSpotsMap.ContainsKey(obj.Name);
    }

    public void AddFreeBusinessSpots(GameGridObject obj)
    {
        BusyBusinessSpotsMap.Remove(obj.Name);
        FreeBusinessSpotsMap.Add(obj.Name, obj);
        FreeBusinessSpots.Enqueue(obj);
    }

    public GameGridObject GetTableWithClient()
    {
        return TablesWithClient.Count <= 0 ? null : TablesWithClient.Dequeue();
    }

    public Vector3Int GetClosestPathGridPoint(Vector3Int init, Vector3Int target)
    {
        float min = float.MaxValue;
        Vector3Int result = target;

        for (int i = 0; i < arroundVectorPoints.GetLength(0); i++)
        {
            int x = arroundVectorPoints[i, 0] + target.x;
            int y = arroundVectorPoints[i, 1] + target.y;

            Vector3Int tmp = new Vector3Int(x, y, 0);

            if (IsCoordsValid(x, y) && grid[x, y] == 0 && min > Vector3Int.Distance(init, tmp))
            {

                min = Vector3Int.Distance(init, tmp);
                result = tmp;
            }
        }
        return result;
    }

    public void AddClientToTable(GameGridObject obj)
    {
        TablesWithClient.Enqueue(obj);
    }
    // Used to highlight the current object being edited
    public void SetActiveGameGridObject(GameGridObject obj)
    {
        if (currentClickedActiveGameObject != "")
        {
            GameGridObject gameGridObject = businessObjects[currentClickedActiveGameObject];
            gameGridObject.Hide();
        }
        currentClickedActiveGameObject = obj.Name;
        obj.Show();
    }

    public bool IsThisSelectedObject(string objName)
    {
        return currentClickedActiveGameObject == objName;
    }

    public string BussGridToText()
    {
        string output = " ";
        int[,] busGrid = new int[grid.GetLength(0), grid.GetLength(1)];
        int minX = int.MaxValue;
        int minY = int.MaxValue; 
        int maxX = int.MinValue;
        int maxY = int.MinValue; 

        foreach(GameTile tile in listBusinessFloor){
            minX = Mathf.Min(minX, tile.GridPosition.x);
            minY = Mathf.Min(minY, tile.GridPosition.y);
            maxX = Mathf.Max(maxX, tile.GridPosition.x);
            maxY = Mathf.Max(maxY, tile.GridPosition.y);

            busGrid[tile.GridPosition.x, tile.GridPosition.y] = grid[tile.GridPosition.x, tile.GridPosition.y];
        }

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                output += " " + busGrid[i, j];
            }
            output += "\n";
        }
        return output;
    }
}