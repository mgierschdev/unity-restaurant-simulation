using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Game.Controllers.Other_Controllers;
using Game.Players;
using UnityEngine;
using UnityEngine.Tilemaps;
using Util;
using Util.Collections;
using Util.PathFinding;
using Random = UnityEngine.Random;

namespace Game.Grid
{
    /**
     * Problem: Manage tilemaps and grid data for the game world.
     * Goal: Provide grid lookups, pathfinding data, and tilemap helpers.
     * Approach: Maintain concurrent maps and initialize tilemaps/resources.
     * Time: O(n) for grid builds (n = tiles).
     * Space: O(n) for grid maps and arrays.
     */
    public static class BussGrid
    {
        //Tilemap 
        private const int Width = Settings.GridWidth; // Down -> Up

        private const int
            Height = Settings
                .GridHeight; // along side from left to right x = -20, y= -22 ||  x along side left to right

        // Isometric Grid with pathfinding
        public static Tilemap TilemapPathFinding { get; set; }
        private static ConcurrentDictionary<Vector3, GameTile> _mapWorldPositionToTile; // World Position to tile
        private static ConcurrentDictionary<Vector3Int, GameTile> _mapGridPositionToTile; // Local Grid Position to tile

        private static ConcurrentDictionary<Vector3Int, GameTile> _mapPathFindingGrid; // PathFinding Grid to tile

        // Spam Points list
        private static List<GameTile> _spamPoints;

        // Path Finder object, contains the method to return the shortest path
        private static PathFind _pathFind;
        private static int[,] _gridArray;

        private static TextMesh[,] _debugGrid;

        //Floor
        public static Tilemap TilemapFloor { get; set; }
        private static List<GameTile> _listFloorTileMap;

        private static ConcurrentDictionary<Vector3Int, GameTile> _mapFloor;

        //WalkingPath 
        public static Tilemap TilemapWalkingPath { get; set; }
        private static List<GameTile> _listWalkingPathTileMap;

        private static ConcurrentDictionary<Vector3Int, GameTile> _mapWalkingPath;

        //Colliders
        public static Tilemap TilemapColliders { get; set; }
        private static List<GameTile> _listCollidersTileMap;

        private static ConcurrentDictionary<Vector3Int, GameTile> _mapColliders;

        //Game floor, which allows drag/drop
        public static Tilemap TilemapGameFloor { get; set; }
        private static List<GameTile> _listGameFloor;
        private static ConcurrentDictionary<Vector3Int, GameTile> _mapGameFloor;
        public static GameController GameController { get; set; }

        public static GameObject ControllerGameObject { get; set; }

        // Grid object map
        private static ConcurrentDictionary<string, GameGridObject> _gameGridObjectsDictionary;

        public static void Init()
        {
            // TILEMAP DATA 
            _mapWorldPositionToTile = new ConcurrentDictionary<Vector3, GameTile>();
            _mapGridPositionToTile = new ConcurrentDictionary<Vector3Int, GameTile>();
            _mapPathFindingGrid = new ConcurrentDictionary<Vector3Int, GameTile>();

            _spamPoints = new List<GameTile>();

            _mapFloor = new ConcurrentDictionary<Vector3Int, GameTile>();
            _listFloorTileMap = new List<GameTile>();

            _mapColliders = new ConcurrentDictionary<Vector3Int, GameTile>();
            _listCollidersTileMap = new List<GameTile>();

            _gameGridObjectsDictionary = new ConcurrentDictionary<string, GameGridObject>();

            _mapWalkingPath = new ConcurrentDictionary<Vector3Int, GameTile>();
            _listWalkingPathTileMap = new List<GameTile>();

            _listGameFloor = new List<GameTile>();
            _mapGameFloor = new ConcurrentDictionary<Vector3Int, GameTile>();

            //ObjectListConfiguration
            GameObjectList.Init();
            // Object Dragging Handler
            ObjectDraggingHandler.Init();
            // Table Handler 
            TableHandler.Init();

            if (TilemapFloor == null || TilemapColliders == null || TilemapPathFinding == null ||
                TilemapWalkingPath == null || TilemapGameFloor == null)
            {
                GameLog.LogWarning("GridController/tilemap");
                GameLog.LogWarning("tilemapFloor " + TilemapFloor);
                GameLog.LogWarning("tilemapColliders " + TilemapColliders);
                // GameLog.LogWarning("tilemapObjects " + TilemapObjects);
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

            _pathFind = new PathFind();
            _gridArray = new int[Settings.GridHeight, Settings.GridWidth];
            _debugGrid = new TextMesh[Settings.GridHeight, Settings.GridWidth];

            InitGrid();
            BuildGrid(); // We need to load the gridTile.UnityTileBase to build first. Which is on the FloorTileMap.
            LoadTileMap(_listCollidersTileMap, TilemapColliders, _mapColliders);
            LoadTileMap(_listWalkingPathTileMap, TilemapWalkingPath, _mapWalkingPath);
            LoadTileMap(_listGameFloor, TilemapGameFloor, _mapGameFloor);
            LoadTileMap(_listFloorTileMap, TilemapFloor, _mapFloor);
            DrawBuss();
        }

        // Increases the tilemap buss floor/Walls for the upgrade
        public static void ReloadTilemapGameFloor(Tilemap floor)
        {
            TilemapGameFloor = floor;
            LoadTileMap(_listGameFloor, TilemapGameFloor, _mapGameFloor);
            // Re-draws floor
            DrawBuss();
        }

        // Draws buss floor/walls
        private static void DrawBuss()
        {
            DrawFloor();
            DrawBussWalls();
        }

        private static void DrawFloor()
        {
            Tilemap floorToDraw = GameObject.Find(Settings.TilemapBusinessFloorDecoration).GetComponent<Tilemap>();
            floorToDraw.ClearAllTiles();
            TileBase gridTile = Resources.Load<Tile>(Settings.GridTilesFloorBrown);

            foreach (Vector3Int pos in TilemapGameFloor.cellBounds.allPositionsWithin)
            {
                if (!TilemapGameFloor.HasTile(pos))
                {
                    continue;
                }

                TileBase tile = TilemapGameFloor.GetTile(pos);
                TileType tileType = Util.Util.GetTileType(tile.name);

                if (tileType == TileType.BusFloor)
                {
                    floorToDraw.SetTile(pos, gridTile);
                }
            }
        }

        private static void DrawBussWalls()
        {
            //Buss walls
            TileBase startWall = Resources.Load<Tile>(Settings.BussWalls[0]),
                frontWall = Resources.Load<Tile>(Settings.BussWalls[1]),
                cornerWall = Resources.Load<Tile>(Settings.BussWalls[2]),
                frontRotated = Resources.Load<Tile>(Settings.BussWalls[3]),
                endFrontRotated = Resources.Load<Tile>(Settings.BussWalls[4]);
            var wallToDraw = GameObject.Find(Settings.TilemapBusinessWallDecoration).GetComponent<Tilemap>();
            wallToDraw.ClearAllTiles();

            var grid = GetBussGridGrid();

            // Drawing beginning
            wallToDraw.SetTile(grid[0][grid[0].Count - 1], startWall);

            // Drawing front
            for (int i = 1; i < grid.Count; i++)
            {
                wallToDraw.SetTile(grid[i][grid[0].Count - 1], frontWall);
            }

            //Corner
            wallToDraw.SetTile(grid[grid.Count - 1][grid[0].Count - 1], cornerWall);

            // Drawing rotated front
            for (int i = grid[0].Count - 2; i >= 1; i--)
            {
                wallToDraw.SetTile(grid[grid.Count - 1][i], frontRotated);
            }

            //Drawing end
            wallToDraw.SetTile(grid[grid.Count - 1][0], endFrontRotated);
        }

        private static void DrawCellCoords()
        {
            foreach (GameTile tile in _mapPathFindingGrid.Values)
            {
                if (tile.GridPosition.x >= _gridArray.GetLength(0) || tile.GridPosition.y >= _gridArray.GetLength(1))
                {
                    continue;
                }

                _debugGrid[tile.GridPosition.x, tile.GridPosition.y] = Util.Util.CreateTextObject(
                    tile.GridPosition.x + "," + tile.GridPosition.y, ControllerGameObject,
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

            var text = _debugGrid[cellPosition.x, cellPosition.y];
            text.color = Util.Util.GetRandomColor();
        }

        private static void InitGrid()
        {
            for (int i = 0; i < _gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < _gridArray.GetLength(1); j++)
                {
                    _gridArray[i, j] = (int)CellValue.Busy;
                }
            }
        }

        private static void SetCellColor(int x, int y, Color color)
        {
            _debugGrid[x, y].color = color;
        }

        private static void BuildGrid()
        {
            TileBase gridTile = Resources.Load<Tile>(Settings.GridTilesSimple);

            for (int x = 0; x <= Height; x++)
            {
                for (int y = 0; y <= Width; y++)
                {
                    Vector3Int positionInGrid = new Vector3Int(x, y, 0);
                    Vector3 positionInWorld = TilemapPathFinding.CellToWorld(positionInGrid);
                    Vector3Int positionLocalGrid = TilemapPathFinding.WorldToCell(positionInWorld);
                    GameTile gameTile = new GameTile(positionInWorld, new Vector3Int(x, y), positionLocalGrid,
                        Util.Util.GetTileType(gridTile.name), Util.Util.GetTileObjectType(Util.Util.GetTileType(gridTile.name)),
                        gridTile);
                    _mapWorldPositionToTile.TryAdd(gameTile.WorldPosition, gameTile);
                    _mapPathFindingGrid.TryAdd(gameTile.GridPosition, gameTile);
                    _mapGridPositionToTile.TryAdd(gameTile.LocalGridPosition, gameTile);
                    TilemapPathFinding.SetTile(new Vector3Int(x, y, 0), gridTile);

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

        private static void LoadTileMap(List<GameTile> list, Tilemap tilemap,
            ConcurrentDictionary<Vector3Int, GameTile> map)
        {
            foreach (Vector3 pos in tilemap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = Vector3Int.FloorToInt(pos);
                Vector3 placeInWorld = tilemap.CellToWorld(localPlace);

                if (tilemap.HasTile(localPlace))
                {
                    TileBase tile = tilemap.GetTile(localPlace);
                    Vector3Int gridPosition = GetPathFindingGridFromWorldPosition(placeInWorld);
                    TileType tileType = Util.Util.GetTileType(tile.name);
                    GameTile gameTile = new GameTile(placeInWorld, gridPosition,
                        GetLocalGridFromWorldPosition(placeInWorld), tileType,
                        Util.Util.GetTileObjectType(Util.Util.GetTileType(tile.name)), tile);
                    list.Add(gameTile);
                    map.TryAdd(gridPosition, gameTile);

                    if (tileType == TileType.WalkablePath)
                    {
                        _gridArray[gridPosition.x, gridPosition.y] = (int)CellValue.Empty;
                    }

                    if (tileType == TileType.BusFloor)
                    {
                        _gridArray[gridPosition.x, gridPosition.y] = (int)CellValue.Empty;
                    }

                    if (tileType == TileType.SpamPoint)
                    {
                        _spamPoints.Add(gameTile);
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

            if (ObjectType.Obstacle == type)
            {
                _gridArray[x, y] = (int)type;
            }
            else
            {
                _gridArray[x, y] = (int)type;
            }
        }

        private static bool IsCoordValid(Vector3Int position)
        {
            return IsCoordValid(position.x, position.y);
        }

        private static bool IsCoordValid(int x, int y)
        {
            return x >= 0 && x < _gridArray.GetLength(0) && y >= 0 && y < _gridArray.GetLength(1);
        }

        public static void SetObjectObstacle(GameGridObject obj)
        {
            Vector3Int actionGridPosition = obj.GetActionTileInGridPosition();
            _gameGridObjectsDictionary.TryAdd(obj.Name, obj);
            if (obj.Type == ObjectType.NpcSingleTable)
            {
                TableHandler.GetBussQueueMap().TryAdd(obj, 0);
                _gridArray[obj.GridPosition.x, obj.GridPosition.y] = (int)CellValue.Busy;
                _gridArray[actionGridPosition.x, actionGridPosition.y] = (int)CellValue.ActionPoint;
            }
            else
            {
                _gridArray[obj.GridPosition.x, obj.GridPosition.y] = (int)CellValue.Busy;

                if (obj.Type == ObjectType.NpcCounter)
                {
                    _gridArray[actionGridPosition.x, actionGridPosition.y] = (int)CellValue.ActionPoint;
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
                   _gridArray[position.x, position.y] == (int)CellValue.Empty &&
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
                    _gridArray[gridPosition.x, gridPosition.y] == (int)CellValue.Empty &&
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
                   !GameController.GetPlayerPositionSet().Contains(gridPosition) &&
                   !GameController.GetPlayerPositionSet().Contains(gridActionPoint) &&
                   _gridArray[gridPosition.x, gridPosition.y] == (int)CellValue.Empty &&
                   _gridArray[gridActionPoint.x, gridActionPoint.y] == (int)CellValue.Empty &&
                   IsValidBussCoord(gridPosition) && IsValidBussCoord(gridActionPoint) &&
                   !IsGridPositionBlockingEntrance(gridPosition) &&
                   !GameController.IsPathPlannedByEmployee(gridPosition);
        }

        private static bool IsValidBussCoord(Vector3Int pos)
        {
            return _mapGameFloor.ContainsKey(pos);
        }

        // Gets a GameTIle in Camera.main.ScreenToWorldPoint(Input.mousePosition))      
        public static GameTile GetGameTileFromClickInPathFindingGrid(Vector3Int position)
        {
            if (_mapPathFindingGrid.TryGetValue(position, out var grid))
            {
                return grid;
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
            Vector3 currentPos =
                GetWorldFromPathFindingGridPositionWithOffSet(GetPathFindingGridFromWorldPosition(worldPos));
            //test
            Vector3 offset = new Vector3(0, 0.25f, 0);
            currentPos -= offset;
            return currentPos;
        }

        public static List<Node> GetPath(int[] start, int[] end)
        {
            if (_gridArray[start[0], start[1]] == (int)CellValue.Busy ||
                _gridArray[end[0], end[1]] == (int)CellValue.Busy)
            {
                return new List<Node>();
            }

            return _pathFind.Find(start, end, _gridArray);
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

            if (Util.Util.CompareNegativeInfinity(position))
            {
                return Util.Util.GetVector3IntNegativeInfinity();
            }

            if (_mapGridPositionToTile.ContainsKey(TilemapPathFinding.WorldToCell(position)))
            {
                GameTile tile = _mapGridPositionToTile[TilemapPathFinding.WorldToCell(position)];
                return tile.GridPosition;
            }

            GameLog.LogWarning("GetPathFindingGridFromWorldPosition/ mapGridPositionToTile does not contain the key " +
                               position + "/" + TilemapPathFinding.WorldToCell(position));
            return Util.Util.GetVector3IntNegativeInfinity();
        }

        public static Vector3 GetWorldFromPathFindingGridPositionWithOffSet(Vector3Int position)
        {
            if (Util.Util.CompareNegativeInfinity(position))
            {
                return Util.Util.GetVector3IntNegativeInfinity();
            }

            GameTile tile = _mapPathFindingGrid[position];
            return tile.GetWorldPositionWithOffset();
        }

        public static Vector3 GetWorldFromPathFindingGridPosition(Vector3Int position)
        {
            if (position.Equals(Util.Util.GetVector3IntNegativeInfinity()))
            {
                return Util.Util.GetVector3IntNegativeInfinity();
            }


            GameTile tile = _mapPathFindingGrid[position];
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
            while (wanderPos == currentPosition && distance <= Settings.MinEuclideanDistanceRandomWalk)
            {
                wanderPos = GetRandomWalkableGridPosition();
                distance = Util.Util.EuclideanDistance(new[] { currentPosition.x, currentPosition.y },
                    new[] { wanderPos.x, wanderPos.y });
            }

            return wanderPos;
        }

        private static Vector3Int GetRandomWalkableGridPosition()
        {
            if (_listWalkingPathTileMap.Count == 0)
            {
                GameLog.LogWarning("There is not listWalkingPathTileMap points");
                return Vector3Int.zero;
            }

            GameTile tile = _listWalkingPathTileMap[Random.Range(0, _listWalkingPathTileMap.Count)];
            return tile.GridPosition;
        }

        public static GameTile GetRandomSpamPointWorldPosition()
        {
            if (_spamPoints.Count == 0)
            {
                GameLog.LogWarning("There is not spam points");
            }

            GameTile tile = _spamPoints[Random.Range(0, _spamPoints.Count)];
            return tile;
        }

        // Unset position in Grid
        private static void FreeGridPosition(int x, int y)
        {
            if (!IsCoordValid(x, y) && x > 1 && y > 1)
            {
                return;
            }

            _gridArray[x, y] = (int)CellValue.Empty;
        }

        public static void UpdateObjectPosition(GameGridObject gameGridObject)
        {
            _gridArray[gameGridObject.GridPosition.x, gameGridObject.GridPosition.y] = (int)CellValue.Busy;
            if (gameGridObject.GetStoreGameObject().HasActionPoint)
            {
                Vector3Int
                    gridActionTile =
                        gameGridObject
                            .GetActionTileInGridPosition(); // GetPathFindingGridFromWorldPosition(gameGridObject.GetActionTile());
                _gridArray[gridActionTile.x, gridActionTile.y] = (int)CellValue.ActionPoint;
            }
        }

        // It gets the closest free coord next to the target
        // This should be calculating with Vector3 instead of vector3int
        public static Vector3Int GetClosestPathGridPoint(Vector3Int currentPosition, Vector3Int target)
        {
            Vector3Int result = target;
            int distance = int.MaxValue;

            for (int i = 0; i < Util.Util.AroundVectorPoints.GetLength(0); i++)
            {
                int x = Util.Util.AroundVectorPoints[i, 0] + target.x;
                int y = Util.Util.AroundVectorPoints[i, 1] + target.y;
                Vector3Int tmp = new Vector3Int(x, y, 0);

                if (IsCoordValid(x, y) && _gridArray[x, y] == (int)CellValue.Empty)
                {
                    List<Node> path = GetPath(new[] { currentPosition.x, currentPosition.y },
                        new[] { target.x, target.y });
                    if (distance > path.Count && path.Count != 0)
                    {
                        result = tmp;
                    }
                }
            }

            return result;
        }

        private static int[,] GetBussGrid(Vector3Int position)
        {
            int[,] busGrid = new int[_gridArray.GetLength(0), _gridArray.GetLength(1)];
            int[,] gridClone = (int[,])_gridArray.Clone();
            gridClone[position.x, position.y] = (int)CellValue.Busy;

            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            foreach (GameTile tile in _listGameFloor)
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

            return _gridArray[pos.x, pos.y] == 0 && _mapGameFloor.ContainsKey(pos);
        }

        //Gets the closest next tile to the object
        public static Vector3Int[] GetNextFreeTileWithActionPoint(GameGridObject gameGridObject)
        {
            for (int i = 0; i < Util.Util.AroundVectorPointsPlusTwo.GetLength(0); i++)
            {
                Vector3Int offset = new Vector3Int(Util.Util.AroundVectorPointsPlusTwo[i, 0],
                    Util.Util.AroundVectorPointsPlusTwo[i, 1], 0);
                Vector3Int position = gameGridObject.GridPosition + offset;

                Vector3Int actionPoint = position + new Vector3Int(Util.Util.ObjectSide[0, 0], Util.Util.ObjectSide[0, 1], 0);
                Vector3Int actionPoint2 = position + new Vector3Int(Util.Util.ObjectSide[1, 0], Util.Util.ObjectSide[1, 1], 0);

                bool isClosingGrid = IsClosingIsland(position);

                if (IsFreeBussCoord(position) && IsFreeBussCoord(actionPoint) && !isClosingGrid &&
                    !GameController.PositionOverlapsNpc(position))
                {
                    return new[] { position, Vector3Int.up }; //front
                }

                if (IsFreeBussCoord(position) && IsFreeBussCoord(actionPoint2) && !isClosingGrid &&
                    !GameController.PositionOverlapsNpc(position))
                {
                    return new[] { position, Vector3Int.right }; // front inverted
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

            return Util.Util.GetVector3IntNegativeInfinity();
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

            return Util.Util.GetVector3IntNegativeInfinity();
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

                        Dfs(bGrid, i, j);
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

        private static void Dfs(int[,] bGrid, int x, int y)
        {
            // grid =  2, means visited
            if (x < 0 || y < 0 || x >= bGrid.GetLength(0) || y >= bGrid.GetLength(1) ||
                bGrid[x, y] == (int)CellValue.Visited || bGrid[x, y] == (int)CellValue.Busy)
            {
                return;
            }

            bGrid[x, y] = (int)CellValue.Visited;
            Dfs(bGrid, x, y - 1);
            Dfs(bGrid, x - 1, y);
            Dfs(bGrid, x, y + 1);
            Dfs(bGrid, x + 1, y);
        }

        // This evaluates that the Grid is representing properly every object position
        public static void RecalculateBussGrid()
        {
            HashSet<Vector3Int> positions = new HashSet<Vector3Int>();
            HashSet<Vector3Int> actionPositions = new HashSet<Vector3Int>();

            foreach (GameGridObject gObj in _gameGridObjectsDictionary.Values)
            {
                if (PlayerData.IsItemStored(gObj.Name) || gObj.GetIsObjectSelected()) //Replacing
                {
                    continue;
                }

                Vector3Int currentGrid = gObj.GridPosition;
                positions.Add(currentGrid);
                actionPositions.Add(gObj.GetActionTileInGridPosition());

                // The grid is empty and there is an object
                if (_gridArray[currentGrid.x, currentGrid.y] == (int)CellValue.Empty)
                {
                    _gridArray[currentGrid.x, currentGrid.y] = (int)CellValue.Busy;
                }

                if (_gridArray[currentGrid.x, currentGrid.y] == (int)CellValue.Empty &&
                    gObj.GetStoreGameObject().HasActionPoint)
                {
                    _gridArray[currentGrid.x, currentGrid.y] = (int)CellValue.ActionPoint;
                }
            }

            foreach (GameTile tile in _listGameFloor)
            {
                Vector3Int current = new Vector3Int(tile.GridPosition.x, tile.GridPosition.y);

                // The grid is busy and there is no object
                if ((_gridArray[tile.GridPosition.x, tile.GridPosition.y] == (int)CellValue.Busy &&
                     !positions.Contains(current)) ||
                    (_gridArray[tile.GridPosition.x, tile.GridPosition.y] == (int)CellValue.ActionPoint &&
                     !actionPositions.Contains(current)))
                {
                    // we clean the invalid position   
                    _gridArray[tile.GridPosition.x, tile.GridPosition.y] = (int)CellValue.Empty;
                }
            }
        }

        private static int GetObjectCount()
        {
            return _gameGridObjectsDictionary.Count;
        }

        public static int[,] GetGridArray()
        {
            return _gridArray;
        }

        public static List<GameTile> GetListBusinessFloor()
        {
            return _listGameFloor;
        }


        public static ConcurrentDictionary<string, GameGridObject> GetGameGridObjectsDictionary()
        {
            return _gameGridObjectsDictionary;
        }

        // Gets the world position from the grid position
        public static Vector3 GetMouseOnGameGridWorldPosition()
        {
            // As accurate as the Camera.main.ScreenToWorldPoint(Input.mousePosition) position
            Vector3Int pos = GetLocalGridFromWorldPosition(Util.Util.GetMouseInWorldPosition());
            GameTile tile = _mapGridPositionToTile[pos];
            return tile.WorldPosition;
        }

        private static List<Vector3Int> GetBussEntrance()
        {
            List<Vector3Int> entranceTile = new List<Vector3Int>();

            foreach (GameTile tile in _listGameFloor)
            {
                for (int i = 0; i < Util.Util.AroundPartialVectorPoints.GetLength(0); i++)
                {
                    Vector3Int tmp = new Vector3Int(tile.GridPosition.x + Util.Util.AroundVectorPoints[i, 0],
                        tile.GridPosition.x + Util.Util.AroundVectorPoints[i, 1]);
                    if (_mapWalkingPath.ContainsKey(tmp))
                    {
                        entranceTile.Add(tile.GridPosition);
                        break;
                    }
                }
            }

            return entranceTile;
        }

        private static bool IsGridPositionBlockingEntrance(Vector3Int pos)
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
                if (_gridArray[vector3.x, vector3.y] == 0)
                {
                    count++;
                }
            }

            return count < 2;
        }

        public static void FreeObject(GameGridObject obj)
        {
            _gridArray[obj.GridPosition.x, obj.GridPosition.y] = 0;

            if (obj.GetStoreGameObject().HasActionPoint)
            {
                _gridArray[obj.GetActionTileInGridPosition().x, obj.GetActionTileInGridPosition().y] = 0;
            }
        }

        public static int[,] GetBussGridWithinGrid()
        {
            int[,] bussGrid = new int[_gridArray.GetLength(0), _gridArray.GetLength(1)];

            foreach (GameTile tile in _listGameFloor)
            {
                bussGrid[tile.GridPosition.x, tile.GridPosition.y] =
                    _gridArray[tile.GridPosition.x, tile.GridPosition.y];
            }

            return bussGrid;
        }

        public static List<List<Vector3Int>> GetBussGridGrid()
        {
            int[,] bussGrid = new int[_gridArray.GetLength(0), _gridArray.GetLength(1)];
            List<List<Vector3Int>> bussGridList = new List<List<Vector3Int>>();

            foreach (GameTile tile in _listGameFloor)
            {
                bussGrid[tile.GridPosition.x, tile.GridPosition.y] = 1;
            }

            for (int i = 0; i < bussGrid.GetLength(0); i++)
            {
                List<Vector3Int> level = new List<Vector3Int>();

                for (int j = 0; j < bussGrid.GetLength(1); j++)
                {
                    if (bussGrid[i, j] == 1)
                    {
                        level.Add(new Vector3Int(i, j));
                    }
                }

                if (level.Count > 0)
                {
                    bussGridList.Add(level);
                }
            }

            return bussGridList;
        }

        public static string GetObjectID(ObjectType type)
        {
            string id = GetObjectCount() + 1 + "-" + Time.frameCount + "-" + Guid.NewGuid().ToString().Substring(0, 5);
            return type + "." + id;
        }
    }
}
