public static class Settings
{
    // DEBUG parameters
    public const int DebugTextSize = 9;

    // Player Config
    public const bool PlayerWalkOnClick = true;

    public const float PlayerMovementSpeed = 3f;

    //GAME TAGS
    public const string NpcTag = "NPC";

    public const string NpcEmployeeTag = "Employee";

    // TILEMAPS
    public const string TilemapFloor0 = "TilemapFloor0";
    public const string TilemapColliders = "TilemapColliders";
    public const string TilemapObjects = "TilemapObjects";
    public const string TilemapWalkingPath = "TilemapWalkingPath";
    public const string TilemapBusinessFloor = "TilemapBusinessFloor";

    public const string PathFindingGrid = "PathFindingGrid";

    // PREFABS AND OBJECTS
    public const string PrefabGridTile = "Grid/GridTile";
    public const string PrefabSandSea = "Grid/SandAndSea@3x";
    public const string PrefabPlayer = "Players/Player";
    public const string PrefabNpcClient = "Players/Client";
    public const string PrefabNpcEmployee = "Players/Employee";
    public const string PrefabInventoryItem = "Menu/InventoryItem";
    public const string PrefabInventoryItemImage = "Image";

    public const string PrefabInventoryItemTextPrice = "Image/TextBackground/Price";

    // INVENTORY SPRITE ITEMS
    public const string SingleWoodenTable = "SpriteLibs/MenuItemSprites/MenuItemSingleTableWithChair@3x";

    // NPC Default
    public const float MinDistanceToTarget = 0.13f;
    public const float NpcDefaultMovementSpeed = 7f; // 0.7f
    public const int NpcDefaultEnergy = 100;
    public const string NpcEnergyBar = "EnergyBar";

    public const string NpcCharacter = "Character";

    // UI Constants
    public const string ConstNpcProfileMenu = "NPCProfile";
    public const string ConstCanvasParentMenu = "CanvasMenu";
    public const string ConstTopMenuDisplayMoney = "MoneyText";
    public const string ConstCenterTabMenu = "CenterTabMenu";
    public const string ConstCenterTabMenuBody = "MenuBody";
    
    public const string ConstCenterScrollContent =
        ConstCenterTabMenuBody + "/Panel/DisplayBackground/ScrollView/Viewport/ScrollContent";

    public const string ConstLeftDownPanel = "LeftDownPanel";
    public const string ConstEditStoreMenuPanel = "EditStoreMenu";
    public const string ConstEditStoreMenuCancel = "Cancel";
    public const string ConstEditStoreMenuAccept = "Accept";
    public const string ConstEditStoreMenuRotate = "Rotate";
    public const string ConstParentGameObject = "Game";
    public const int ConstDefaultCameraOrthographicsize = 7;

    public const int ConstDefaultBackgroundOrderingLevel = 200;

    // UI: Menu
    public const string ConstLeftDownMenuInventory = "Inventory";
    public const string ConstLeftDownMenuStore = "Store";

    public const string ConstLeftDownMenuEmployees = "Employees";

    // UI: Camera
    public const bool CameraPerspectiveHand = true; //CAMERA_PERSPECTIVE_HAND or CAMERA_FOLLOW_PLAYER
    public static float[] CameraPerspectiveHandClampX = { -20f, 20 };
    public static float[] CameraPerspectiveHandClampY = { -20, 20f }; // X = -8, 1 || Y = Initial default -8, 1

    public const float CameraFollowInterpolation = 0.034f;

    // UI : Camera
    public const int ConstDefaultCameraWidth = 1500;

    public const int ConstDefaultCameraHeight = 1600;

    //UI: Buttons listeners
    public const string ConstUIExitButton = "ExitButton";

    public const string ConstUIInventoryButton = "Inventory";

    //SCENE
    public const string GameGrid = "GameGrid";

    //Tiles
    private const string GRID_TILES = "Grid/Tiles/";
    public const string GridTilesSimple = GRID_TILES + "GridTile";
    public const string GridTilesHighlightedFloor = GRID_TILES + "HighlightedFloor@3x";

    // Grid Config
    public const float GridCellSize = 0.25f; // 0.25f default
    public const int GridWidth = 44; // Number of cell per Grid CellSize
    public const int GridHeight = 50;
    public const int GridStartX = -22;
    public const int GrtGridStartY = -32;
}