using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public static class BussGrid
{
    //Tilemap 
    private static int WIDTH = Settings.GridWidth; // Down -> Up
    private static int HEIGHT = Settings.GridHeight; // along side from left to right x = -20, y= -22 ||  x along side left to right
    private static Vector3Int gridOriginPosition = new Vector3Int(Settings.GridStartX, Settings.GrtGridStartY, Util.ConstDefaultBackgroundOrderingLevel);
    // Isometric Grid with pathfinding
    public static Tilemap TilemapPathFinding { get; set; }
    private static ConcurrentDictionary<Vector3, GameTile> mapWorldPositionToTile; // World Position to tile
    private static ConcurrentDictionary<Vector3Int, GameTile> mapGridPositionToTile; // Local Grid Position to tile
    private static ConcurrentDictionary<Vector3Int, GameTile> mapPathFindingGrid; // PathFinding Grid to tile
    // Spam Points list
    private static List<GameTile> spamPoints;
    // Path Finder object, contains the method to return the shortest path
    private static PathFind pathFind;
    private static int[,] gridArray;
    private static TextMesh[,] debugGrid;
    //Floor
    public static Tilemap TilemapFloor { get; set; }
    private static List<GameTile> listFloorTileMap;
    private static ConcurrentDictionary<Vector3Int, GameTile> mapFloor;
    //WalkingPath 
    public static Tilemap TilemapWalkingPath { get; set; }
    private static List<GameTile> listWalkingPathTileMap;
    private static ConcurrentDictionary<Vector3Int, GameTile> mapWalkingPath;
    //Colliders
    public static Tilemap TilemapColliders { get; set; }
    private static List<GameTile> listCollidersTileMap;
    private static ConcurrentDictionary<Vector3Int, GameTile> mapColliders;
    //Objects
    public static Tilemap TilemapObjects { get; set; }
    private static List<GameTile> listObjectsTileMap;
    private static ConcurrentDictionary<Vector3Int, GameTile> mapObjects;
    //Game floor, which allows drag/drop
    public static Tilemap TilemapGameFloor { get; set; }
    private static List<GameTile> listGameFloor;
    private static ConcurrentDictionary<Vector3Int, GameTile> mapGameFloor;
    public static GameController GameController { get; set; }
    public static GameObject ControllerGameObject { get; set; }
    // Grid object map
    private static ConcurrentDictionary<string, GameGridObject> gameGridObjectsDictionary;

    public static void Init()
    {
        // TILEMAP DATA 
        mapWorldPositionToTile = new ConcurrentDictionary<Vector3, GameTile>();
        mapGridPositionToTile = new ConcurrentDictionary<Vector3Int, GameTile>();
        mapPathFindingGrid = new ConcurrentDictionary<Vector3Int, GameTile>();
        spamPoints = new List<GameTile>();

        mapFloor = new ConcurrentDictionary<Vector3Int, GameTile>();
        listFloorTileMap = new List<GameTile>();

        mapColliders = new ConcurrentDictionary<Vector3Int, GameTile>();
        listCollidersTileMap = new List<GameTile>();

        mapObjects = new ConcurrentDictionary<Vector3Int, GameTile>();
        listObjectsTileMap = new List<GameTile>();

        gameGridObjectsDictionary = new ConcurrentDictionary<string, GameGridObject>();

        mapWalkingPath = new ConcurrentDictionary<Vector3Int, GameTile>();
        listWalkingPathTileMap = new List<GameTile>();

        listGameFloor = new List<GameTile>();
        mapGameFloor = new ConcurrentDictionary<Vector3Int, GameTile>();

        //ObjectListConfiguration
        MenuObjectList.Init();

        // Object Dragging Handler
        ObjectDraggingHandler.Init();
        // Table Handler 
        TableHandler.Init();

        if (TilemapFloor == null || TilemapColliders == null || TilemapObjects == null || TilemapPathFinding == null ||
            TilemapWalkingPath == null || TilemapGameFloor == null)
        {
            GameLog.LogWarning("GridController/tilemap");
            GameLog.LogWarning("tilemapFloor " + TilemapFloor);
            GameLog.LogWarning("tilemapColliders " + TilemapColliders);
            GameLog.LogWarning("tilemapObjects " + TilemapObjects);
            GameLog.LogWarning("tilemapPathFinding " + TilemapPathFinding);
            GameLog.LogWarning("tilemapWalkingPath " + TilemapWalkingPath);
            GameLog.LogWarning("tilemapBusinessFloor " + TilemapGameFloor);
        }

        TilemapPathFinding.color = new Color(1, 1, 1, 0.0f);
        TilemapColliders.color = new Color(1, 1, 1, 0.0f);
        TilemapWalkingPath.color = new Color(1, 1, 1, 0.0f);
        TilemapGameFloor.color = new Color(1, 1, 1, 0.0f);

        if (Settings.CellDebug)
        {
            TilemapPathFinding.color = new Color(1, 1, 1, 0.3f);
            TilemapColliders.color = new Color(1, 1, 1, 0.3f);
            TilemapWalkingPath.color = new Color(1, 1, 1, 0.3f);
            TilemapGameFloor.color = new Color(1, 1, 1, 0.3f);
        }

        pathFind = new PathFind();
        gridArray = new int[Settings.GridHeight, Settings.GridWidth];
        debugGrid = new TextMesh[Settings.GridHeight, Settings.GridWidth];

        InitGrid();
        BuildGrid(); // We need to load the gridTile.UnityTileBase to build first. Which is on the FloorTileMap.
        LoadTileMap(listCollidersTileMap, TilemapColliders, mapColliders);
        LoadTileMap(listObjectsTileMap, TilemapObjects, mapObjects);
        LoadTileMap(listWalkingPathTileMap, TilemapWalkingPath, mapWalkingPath);
        LoadTileMap(listGameFloor, TilemapGameFloor, mapGameFloor);
        LoadTileMap(listFloorTileMap, TilemapFloor, mapFloor);
        DrawTilemapBusinessDecoration();
    }

    public static void DrawTilemapBusinessDecoration()
    {
        Tilemap floorToDraw = GameObject.Find(Settings.TilemapBusinessFloor_Decoration).GetComponent<Tilemap>();
        floorToDraw.ClearAllTiles();
        TileBase gridTile = Resources.Load<Tile>(Settings.GridTilesFloorBrown);

        foreach (Vector3Int pos in TilemapGameFloor.cellBounds.allPositionsWithin)
        {
            if (!TilemapGameFloor.HasTile(pos))
            {
                continue;
            }

            TileBase tile = TilemapGameFloor.GetTile(pos);
            TileType tileType = Util.GetTileType(tile.name);
            if (tileType == TileType.BUS_FLOOR)
            {
                floorToDraw.SetTile(pos, gridTile);
            }
        }
    }

    private static void DrawCellCoords()
    {
        foreach (GameTile tile in mapPathFindingGrid.Values)
        {
            if (tile.GridPosition.x >= gridArray.GetLength(0) || tile.GridPosition.y >= gridArray.GetLength(1))
            {
                continue;
            }
            debugGrid[tile.GridPosition.x, tile.GridPosition.y] = Util.CreateTextObject(tile.GridPosition.x + "," + tile.GridPosition.y, ControllerGameObject,
                "(" + tile.GridPosition.x + "," + tile.GridPosition.y + ") " + tile.WorldPosition.x + "," +
                tile.WorldPosition.y, tile.WorldPosition, Settings.DebugTextSize, Color.black,
                TextAnchor.MiddleCenter, TextAlignment.Center);
        }
    }

    // Debug method to draw a cell
    public static void DrawCell(Vector3Int cellPosition)
    {
        if (!IsCoordValid(cellPosition) || !Settings.CellDebug)
        {
            return;
        }
        TextMesh text = debugGrid[cellPosition.x, cellPosition.y];
        text.color = Util.GetRandomColor();
    }

    private static void InitGrid()
    {
        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                gridArray[i, j] = (int)CellValue.BUSY;
            }
        }
    }

    private static void SetCellColor(int x, int y, Color color)
    {
        debugGrid[x, y].color = (Color)color;
    }

    private static void BuildGrid()
    {
        TileBase gridTile = Resources.Load<Tile>(Settings.GridTilesSimple);

        for (int x = 0; x <= HEIGHT; x++)
        {
            for (int y = 0; y <= WIDTH; y++)
            {
                Vector3Int positionInGrid = new Vector3Int(x + gridOriginPosition.x, y + gridOriginPosition.y, 0);
                Vector3 positionInWorld = TilemapPathFinding.CellToWorld(positionInGrid);
                Vector3Int positionLocalGrid = TilemapPathFinding.WorldToCell(positionInWorld);
                GameTile gameTile = new GameTile(positionInWorld, new Vector3Int(x, y), positionLocalGrid,
                Util.GetTileType(gridTile.name), Util.GetTileObjectType(Util.GetTileType(gridTile.name)), gridTile);
                mapWorldPositionToTile.TryAdd(gameTile.WorldPosition, gameTile);
                mapPathFindingGrid.TryAdd(gameTile.GridPosition, gameTile);
                mapGridPositionToTile.TryAdd(gameTile.LocalGridPosition, gameTile);
                TilemapPathFinding.SetTile(new Vector3Int(x + gridOriginPosition.x, y + gridOriginPosition.y, 0), gridTile);

                // if (Settings.CellDebug)
                // {
                //     GameLog.Log("DEBUG: Setting GridCell map " + gameTile.WorldPosition + " " + gameTile.GridPosition + " " + gameTile.LocalGridPosition + " " + gameTile.GetWorldPositionWithOffset());
                // }
            }
        }

        if (Settings.CellDebug)
        {
            DrawCellCoords();
        }
    }

    private static void LoadTileMap(List<GameTile> list, Tilemap tilemap, ConcurrentDictionary<Vector3Int, GameTile> map)
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
                    gridArray[gridPosition.x, gridPosition.y] = (int)CellValue.EMPTY;
                }

                if (tileType == TileType.BUS_FLOOR)
                {
                    gridArray[gridPosition.x, gridPosition.y] = (int)CellValue.EMPTY;
                }

                if (tileType == TileType.SPAM_POINT)
                {
                    spamPoints.Add(gameTile);
                }
            }
        }
    }

    // In GameMap/Grid coordinates This sets the obstacle points around the obstacle
    private static void SetGridObstacle(int x, int y, ObjectType type)
    {
        if (!IsCoordValid(x, y) || x < 0 || y < 0)
        {
            return;
        }

        if (ObjectType.OBSTACLE == type)
        {
            gridArray[x, y] = (int)type;
        }
        else
        {
            gridArray[x, y] = (int)type;
        }
    }

    private static bool IsCoordValid(Vector3Int position)
    {
        return IsCoordValid(position.x, position.y);
    }

    private static bool IsCoordValid(int x, int y)
    {
        return x >= 0 && x < gridArray.GetLength(0) && y >= 0 && y < gridArray.GetLength(1);
    }

    public static void SetObjectObstacle(GameGridObject obj)
    {
        Vector3Int actionGridPosition = obj.GetActionTileInGridPosition();
        gameGridObjectsDictionary.TryAdd(obj.Name, obj);
        if (obj.Type == ObjectType.NPC_SINGLE_TABLE)
        {
            TableHandler.GetBussQueueMap().TryAdd(obj, 0);
            gridArray[obj.GridPosition.x, obj.GridPosition.y] = (int)CellValue.BUSY;
            gridArray[actionGridPosition.x, actionGridPosition.y] = (int)CellValue.ACTION_POINT;
        }
        else
        {
            gridArray[obj.GridPosition.x, obj.GridPosition.y] = (int)CellValue.BUSY;

            if (obj.Type == ObjectType.NPC_COUNTER)
            {
                gridArray[actionGridPosition.x, actionGridPosition.y] = (int)CellValue.ACTION_POINT;
            }
        }

        if (Settings.CellDebug)
        {
            SetCellColor(obj.GridPosition.x, obj.GridPosition.y, Color.blue);
            SetCellColor(actionGridPosition.x, actionGridPosition.y, Color.cyan);
        }
    }

    public static bool IsValidWalkablePosition(Vector3Int position)
    {
        return IsCoordValid(position.x, position.y) &&
            //  !IsThereNPCInPosition(position) &&
            gridArray[position.x, position.y] == (int)CellValue.EMPTY &&
            IsValidBussCoord(position);
    }

    // Used while dragging
    // worldPos = Current position that you are moving the object
    // actionTileOne: the initial actiontile in grid coord
    // gameGridObject: the game gridObject
    public static bool IsValidBussPosition(GameGridObject gameGridObject)
    {
        Vector3Int gridPosition = gameGridObject.GridPosition;
        bool isClosingGrid = IsClosingIsland(gridPosition);

        // Single square objects
        if (!gameGridObject.GetStoreGameObject().HasActionPoint)
        {
            return
            IsCoordValid(gridPosition.x, gridPosition.y) &&
            !isClosingGrid &&
            !GameController.GetPlayerPositionSet().Contains(gridPosition) &&
            gridArray[gridPosition.x, gridPosition.y] == (int)CellValue.EMPTY &&
            IsValidBussCoord(gridPosition) &&
            !IsGridPositionBlockingEntrance(gridPosition) &&
            !GameController.IsPathPlannedByEmployee(gridPosition);
        }

        // Objects with two squares or action tiles,
        Vector3Int gridActionPoint = gameGridObject.GetActionTileInGridPosition();

        // GameLog.Log(gameGridObject.Name + " IsValidBussPosition " + (IsCoordValid(gridPosition.x, gridPosition.y) && IsCoordValid(gridActionPoint.x, gridActionPoint.y))
        // + " " + (!isClosingGrid)
        // + " " + (!IsThereNPCInPosition(gridPosition) && !IsThereNPCInPosition(gridActionPoint))
        // + " " + (gridArray[gridPosition.x, gridPosition.y] == (int)CellValue.EMPTY && gridArray[gridActionPoint.x, gridActionPoint.y] == (int)CellValue.EMPTY)
        // + " " + (IsValidBussCoord(gridPosition) && IsValidBussCoord(gridActionPoint))
        // + " " + (!IsGridPositionBlockingEntrance(gridPosition)));

        return IsCoordValid(gridPosition.x, gridPosition.y) && IsCoordValid(gridActionPoint.x, gridActionPoint.y) &&
               !isClosingGrid &&
               !GameController.GetPlayerPositionSet().Contains(gridPosition) && !GameController.GetPlayerPositionSet().Contains(gridActionPoint) &&
               gridArray[gridPosition.x, gridPosition.y] == (int)CellValue.EMPTY && gridArray[gridActionPoint.x, gridActionPoint.y] == (int)CellValue.EMPTY &&
               IsValidBussCoord(gridPosition) && IsValidBussCoord(gridActionPoint) &&
               !IsGridPositionBlockingEntrance(gridPosition) &&
               !GameController.IsPathPlannedByEmployee(gridPosition);
    }

    public static bool IsValidBussCoord(Vector3Int pos)
    {
        return mapGameFloor.ContainsKey(pos);
    }

    // Gets a GameTIle in Camera.main.ScreenToWorldPoint(Input.mousePosition))      
    public static GameTile GetGameTileFromClickInPathFindingGrid(Vector3Int position)
    {
        if (mapPathFindingGrid.ContainsKey(position))
        {
            return mapPathFindingGrid[position];
        }

        GameLog.LogWarning("IsometricGrid/GetGameTileFromClickInWorldPosition null");
        return null;
    }

    public static void HighlightGridBussFloor()
    {
        // If we Highlight we are in edit mode
        TilemapGameFloor.color = new Color(1, 1, 1, 0.5f);
    }
    // This snaps the object to the pathfinding grid position 
    public static Vector3 GetGridWorldPositionMapMouseDrag(Vector3 worldPos)
    {
        Vector3 currentPos = GetWorldFromPathFindingGridPositionWithOffSet(GetPathFindingGridFromWorldPosition(worldPos));
        //test
        Vector3 offset = new Vector3(0, 0.25f, 0);
        currentPos -= offset;
        return currentPos;
    }

    public static List<Node> GetPath(int[] start, int[] end)
    {
        if (gridArray[start[0], start[1]] == (int)CellValue.BUSY || gridArray[end[0], end[1]] == (int)CellValue.BUSY)
        {
            return new List<Node>();
        }

        return pathFind.Find(start, end, gridArray);
    }

    // Returns the Grid position given a Vector3 world position
    public static Vector3Int GetLocalGridFromWorldPosition(Vector3 position)
    {
        return TilemapPathFinding.WorldToCell(position);
    }

    public static Vector3Int GetPathFindingGridFromWorldPosition(Vector3 position)
    {
        Vector3 newPosition = new Vector3(position.x, position.y, 0);
        position = newPosition;

        if (Util.CompareNegativeInifinity(position))
        {
            return Util.GetVector3IntNegativeInfinity();
        }

        if (mapGridPositionToTile.ContainsKey(TilemapPathFinding.WorldToCell(position)))
        {
            GameTile tile = mapGridPositionToTile[TilemapPathFinding.WorldToCell(position)];
            return tile.GridPosition;
        }

        GameLog.LogError("GetPathFindingGridFromWorldPosition/ mapGridPositionToTile does not contain the key " + position + "/" + TilemapPathFinding.WorldToCell(position));
        return Util.GetVector3IntNegativeInfinity();
    }

    public static Vector3 GetWorldFromPathFindingGridPositionWithOffSet(Vector3Int position)
    {

        if (Util.CompareNegativeInifinity(position))
        {
            return Util.GetVector3IntNegativeInfinity();
        }

        GameTile tile = mapPathFindingGrid[position];
        return tile.GetWorldPositionWithOffset();
    }

    public static Vector3 GetWorldFromPathFindingGridPosition(Vector3Int position)
    {

        if (position.Equals(Util.GetVector3IntNegativeInfinity()))
        {
            return Util.GetVector3IntNegativeInfinity();
        }


        GameTile tile = mapPathFindingGrid[position];
        return tile.WorldPosition;
    }

    // Only for unit test case use
    // public static void SetTestGridObstacles(int row, int x1, int x2)
    // {
    //     //int x, int y, ObjectType type, Color? color = null
    //     for (int i = x1; i <= x2; i++)
    //     {
    //         SetGridObstacle(row, i, ObjectType.OBSTACLE);
    //     }
    // }

    // Only for unit test case use
    // public static void FreeTestGridObstacles(int row, int x1, int x2)
    // {
    //     for (int i = x1; i <= x2; i++)
    //     {
    //         FreeGridPosition(row, i);
    //     }
    // }

    public static Vector3Int GetRandomWalkablePosition(Vector3Int currentPosition)
    {
        Vector3Int wanderPos = currentPosition;
        double distance = 0;
        // There is a small chance that the NPC will go to the same place in which he is standing
        while (wanderPos == currentPosition && distance <= Settings.MinEuclidianDistanceRandomWalk)
        {
            wanderPos = GetRandomWalkableGridPosition();
            distance = Util.EuclidianDistance(new int[] { currentPosition.x, currentPosition.y }, new int[] { wanderPos.x, wanderPos.y });
        }
        return wanderPos;
    }

    private static Vector3Int GetRandomWalkableGridPosition()
    {
        if (listWalkingPathTileMap.Count == 0)
        {
            GameLog.LogWarning("There is not listWalkingPathTileMap points");
            return Vector3Int.zero;
        }

        GameTile tile = listWalkingPathTileMap[Random.Range(0, listWalkingPathTileMap.Count)];
        return tile.GridPosition;
    }

    public static GameTile GetRandomSpamPointWorldPosition()
    {
        if (spamPoints.Count == 0)
        {
            GameLog.LogWarning("There is not spam points");
        }

        GameTile tile = spamPoints[Random.Range(0, spamPoints.Count)];
        return tile;
    }

    // Unset position in Grid
    private static void FreeGridPosition(int x, int y)
    {
        if (!IsCoordValid(x, y) && x > 1 && y > 1)
        {
            return;
        }

        gridArray[x, y] = (int)CellValue.EMPTY;
    }

    public static void UpdateObjectPosition(GameGridObject gameGridObject)
    {
        gridArray[gameGridObject.GridPosition.x, gameGridObject.GridPosition.y] = (int)CellValue.BUSY;
        if (gameGridObject.GetStoreGameObject().HasActionPoint)
        {
            Vector3Int gridActionTile = gameGridObject.GetActionTileInGridPosition();// GetPathFindingGridFromWorldPosition(gameGridObject.GetActionTile());
            gridArray[gridActionTile.x, gridActionTile.y] = (int)CellValue.ACTION_POINT;
        }
    }

    // It gets the closest free coord next to the target
    // TODO: Improve so it will choose the closest path and the npc will stand towards the client
    // This should be calculating with Vector3 instead of vector3int
    public static Vector3Int GetClosestPathGridPoint(Vector3Int currentPosition, Vector3Int target)
    {
        Vector3Int result = target;
        int distance = int.MaxValue;

        for (int i = 0; i < Util.ArroundVectorPoints.GetLength(0); i++)
        {
            int x = Util.ArroundVectorPoints[i, 0] + target.x;
            int y = Util.ArroundVectorPoints[i, 1] + target.y;
            Vector3Int tmp = new Vector3Int(x, y, 0);

            if (IsCoordValid(x, y) && gridArray[x, y] == (int)CellValue.EMPTY)
            {
                List<Node> path = GetPath(new[] { currentPosition.x, currentPosition.y }, new[] { target.x, target.y });
                if (distance > path.Count && path.Count != 0)
                {
                    result = tmp;
                }
            }
        }
        return result;
    }

    public static int[,] GetBussGrid(Vector3Int position)
    {
        int[,] busGrid = new int[gridArray.GetLength(0), gridArray.GetLength(1)];
        int[,] gridClone = (int[,])gridArray.Clone();
        gridClone[position.x, position.y] = (int)CellValue.BUSY;

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (GameTile tile in listGameFloor)
        {
            minX = Mathf.Min(minX, tile.GridPosition.x);
            minY = Mathf.Min(minY, tile.GridPosition.y);
            maxX = Mathf.Max(maxX, tile.GridPosition.x);
            maxY = Mathf.Max(maxY, tile.GridPosition.y);
            busGrid[tile.GridPosition.x, tile.GridPosition.y] = gridClone[tile.GridPosition.x, tile.GridPosition.y];
        }

        int[,] reducedBusGrid = new int[maxX - minX + 1, maxY - minY + 1];

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                reducedBusGrid[i - minX, j - minY] = busGrid[i, j];
            }
        }
        return reducedBusGrid;
    }

    public static bool IsFreeBussCoord(Vector3Int pos)
    {
        if (!IsCoordValid(pos.x, pos.y))
        {
            return false;
        }
        return gridArray[pos.x, pos.y] == 0 && mapGameFloor.ContainsKey(pos);
    }

    //Gets the closest next tile to the object
    public static Vector3Int[] GetNextFreeTileWithActionPoint(GameGridObject gameGridObject)
    {
        for (int i = 0; i < Util.AroundVectorPointsPlusTwo.GetLength(0); i++)
        {
            Vector3Int offset = new Vector3Int(Util.AroundVectorPointsPlusTwo[i, 0], Util.AroundVectorPointsPlusTwo[i, 1], 0);
            Vector3Int position = gameGridObject.GridPosition + offset;

            Vector3Int actionPoint = position + new Vector3Int(Util.ObejectSide[0, 0], Util.ObejectSide[0, 1], 0);
            Vector3Int actionPoint2 = position + new Vector3Int(Util.ObejectSide[1, 0], Util.ObejectSide[1, 1], 0);

            bool isClosingGrid = IsClosingIsland(position);

            if (IsFreeBussCoord(position) && IsFreeBussCoord(actionPoint) && !isClosingGrid && !GameController.PositionOverlapsNPC(position))
            {
                return new Vector3Int[] { position, Vector3Int.up }; //front
            }

            if (IsFreeBussCoord(position) && IsFreeBussCoord(actionPoint2) && !isClosingGrid && !GameController.PositionOverlapsNPC(position))
            {
                return new Vector3Int[] { position, Vector3Int.right }; // front inverted
            }
        }
        return new Vector3Int[] { };
    }

    public static Vector3Int GetNextFreeTile()
    {
        Vector3Int spamPoint;

        foreach (GameTile tile in GetListBusinessFloor())
        {
            spamPoint = new Vector3Int(tile.GridPosition.x, tile.GridPosition.y);
            bool isClosingGrid = IsClosingIsland(spamPoint);

            if (IsFreeBussCoord(spamPoint) && !isClosingGrid)
            {
                return spamPoint;
            }
        }
        return Util.GetVector3IntNegativeInfinity();

    }

    public static Vector3Int GetNextTileFromEmptyMap(StoreGameObject obj)
    {
        Vector3Int spamPoint, actionPoint;

        foreach (GameTile tile in GetListBusinessFloor())
        {
            spamPoint = new Vector3Int(tile.GridPosition.x, tile.GridPosition.y);
            actionPoint = new Vector3Int(tile.GridPosition.x, tile.GridPosition.y + 1);

            if (IsFreeBussCoord(spamPoint) && (IsFreeBussCoord(actionPoint) || !obj.HasActionPoint))
            {
                return spamPoint;
            }
        }
        return Util.GetVector3IntNegativeInfinity();
    }

    private static bool IsClosingIsland(Vector3Int position)
    {
        int[,] bGrid = GetBussGrid(position);
        int count = 0;

        for (int i = 0; i < bGrid.GetLength(0); i++)
        {
            for (int j = 0; j < bGrid.GetLength(1); j++)
            {
                if (bGrid[i, j] == 0 || bGrid[i, j] == -1)
                {
                    if (count > 0)
                    {
                        return true;
                    }

                    DFS(bGrid, i, j);
                    count++;
                }
            }
        }

        if (count > 1)
        {
            return true;
        }
        return false;
    }

    private static void DFS(int[,] bGrid, int x, int y)
    {
        // grid =  2, means visited
        if (x < 0 || y < 0 || x >= bGrid.GetLength(0) || y >= bGrid.GetLength(1) || bGrid[x, y] == (int)CellValue.VISITED || bGrid[x, y] == (int)CellValue.BUSY)
        {
            return;
        }

        bGrid[x, y] = (int)CellValue.VISITED;
        DFS(bGrid, x, y - 1);
        DFS(bGrid, x - 1, y);
        DFS(bGrid, x, y + 1);
        DFS(bGrid, x + 1, y);
    }

    // This evaluates that the Grid is representing properly every object position
    public static void RecalculateBussGrid()
    {
        HashSet<Vector3Int> positions = new HashSet<Vector3Int>();
        HashSet<Vector3Int> actionPositions = new HashSet<Vector3Int>();

        foreach (GameGridObject gObj in gameGridObjectsDictionary.Values)
        {
            if (PlayerData.IsItemStored(gObj.Name) || gObj.GetIsObjectSelected())//Replacing
            {
                continue;
            }

            Vector3Int currentGrid = gObj.GridPosition;
            positions.Add(currentGrid);
            actionPositions.Add(gObj.GetActionTileInGridPosition());

            // The grid is empty and there is an object
            if (gridArray[currentGrid.x, currentGrid.y] == (int)CellValue.EMPTY)
            {
                gridArray[currentGrid.x, currentGrid.y] = (int)CellValue.BUSY;
            }

            if (gridArray[currentGrid.x, currentGrid.y] == (int)CellValue.EMPTY && gObj.GetStoreGameObject().HasActionPoint)
            {
                gridArray[currentGrid.x, currentGrid.y] = (int)CellValue.ACTION_POINT;
            }
        }

        foreach (GameTile tile in listGameFloor)
        {
            Vector3Int current = new Vector3Int(tile.GridPosition.x, tile.GridPosition.y);

            // The grid is busy and there is no object
            if ((gridArray[tile.GridPosition.x, tile.GridPosition.y] == (int)CellValue.BUSY && !positions.Contains(current)) ||
            (gridArray[tile.GridPosition.x, tile.GridPosition.y] == (int)CellValue.ACTION_POINT && !actionPositions.Contains(current)))
            {
                // we clean the invalid position   
                gridArray[tile.GridPosition.x, tile.GridPosition.y] = (int)CellValue.EMPTY;
            }
        }
    }

    public static int GetObjectCount()
    {
        return gameGridObjectsDictionary.Count;
    }

    public static int[,] GetGridArray()
    {
        return gridArray;
    }

    public static List<GameTile> GetListBusinessFloor()
    {
        return listGameFloor;
    }


    public static ConcurrentDictionary<string, GameGridObject> GetGameGridObjectsDictionary()
    {
        return gameGridObjectsDictionary;
    }

    // Gets the world position from the grid position
    public static Vector3 GetMouseOnGameGridWorldPosition()
    {
        // As accurate as the Camera.main.ScreenToWorldPoint(Input.mousePosition) position
        Vector3Int pos = GetLocalGridFromWorldPosition(Util.GetMouseInWorldPosition());
        GameTile tile = mapGridPositionToTile[pos];
        return tile.WorldPosition;
    }

    private static List<Vector3Int> GetBussEntrance()
    {
        List<Vector3Int> entranceTile = new List<Vector3Int>();

        foreach (GameTile tile in listGameFloor)
        {

            for (int i = 0; i < Util.ArroundPartialVectorPoints.GetLength(0); i++)
            {
                Vector3Int tmp = new Vector3Int(tile.GridPosition.x + Util.ArroundVectorPoints[i, 0], tile.GridPosition.x + Util.ArroundVectorPoints[i, 1]);
                if (mapWalkingPath.ContainsKey(tmp))
                {
                    entranceTile.Add(tile.GridPosition);
                    break;
                }
            }
        }
        return entranceTile;
    }

    public static bool IsGridPositionBlockingEntrance(Vector3Int pos)
    {
        List<Vector3Int> list = GetBussEntrance();

        if (!list.Contains(pos))
        {
            return false;
        }
        // There most be at least 2 empty spots 
        int count = 0;
        foreach (Vector3Int vector3 in list)
        {
            if (gridArray[vector3.x, vector3.y] == 0)
            {
                count++;
            }
        }

        return count < 2;
    }

    public static void FreeObject(GameGridObject obj)
    {
        gridArray[obj.GridPosition.x, obj.GridPosition.y] = 0;

        if (obj.GetStoreGameObject().HasActionPoint)
        {
            gridArray[obj.GetActionTileInGridPosition().x, obj.GetActionTileInGridPosition().y] = 0;
        }
    }
}