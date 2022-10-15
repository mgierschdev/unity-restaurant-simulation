public static class Settings
{
    // DEBUG / DEV parameterss
    public const bool CellDebug = true;
    public const int DebugTextSize = 9;
    public const bool IsFirebaseEmulatorEnabled = true;

    // Player Config
    public const bool PlayerWalkOnClick = true;
    public const float PlayerMovementSpeed = 3f;

    //FIREBASE TEST ENV 
    public const string FIRESTORE_HOST = IsFirebaseEmulatorEnabled ? "localhost:8080" : "";
    public const string CLOUD_FUNCTION_HOST = IsFirebaseEmulatorEnabled ? "localhost:5001" : "";
    public const string USER_COLLECTION = IsFirebaseEmulatorEnabled ? "Test" : "Users";
    public const string TEST_USER = IsFirebaseEmulatorEnabled ? "mgierschdev@gmail.com" : "";

    //GAME TAGS
    public const string NpcTag = "NPC";
    public const string NpcEmployeeTag = "Employee";
    public const string SliderTag = "Slider";

    //SCENES
    public const string LoadScene = "LoadScene";
    public const string GameScene = "GameScene";
    public const string SliderProgress = "Background/SliderProgress";

    //STORE SPRITES
    public const string StoreSpritePath = "SpriteLibs/MenuItemSprites/";

    // TILEMAPS
    public const string TilemapFloor0 = "TilemapFloor0";
    public const string TilemapColliders = "TilemapColliders";
    public const string TilemapObjects = "TilemapObjects";
    public const string TilemapWalkingPath = "TilemapWalkingPath";
    public const string TilemapBusinessFloor = "TilemapBusinessFloor";
    public const string PathFindingGrid = "PathFindingGrid";

    //SPRITE LIBS CATEGORIES
    public const string SpriteLibCategoryTables = "Tables";
    public const string SpriteLibCategoryTablesInverted = "Tables-Inverted";
    public const string SpriteLibCategoryContainers = "BaseContainers";
    public const string SpriteLibCategoryContainersInverted = "BaseContainers-Inverted";
    public const string SpriteLibCategoryStoreObjects = "StoreObjects";

    // PREFABS AND OBJECTS
    public const string Button = "Button";
    public const string PrefabGridTile = "Grid/GridTile";
    public const string PrefabSingleTable = "Objects/SingleTable";
    public const string PrefabCounter = "Objects/Counter";
    public const string PrefabBaseContainer = "Objects/BaseContainer";
    public const string PrefabSingleTableFrontInverted = "Objects/SingleTableFrontInverted";
    public const string ObjectRotationFrontInverted = "Inverted";
    public const string ObjectRotationFront = "Front";
    public const string PrefabPlayer = "Players/Player";
    public const string PrefabNpcClient = "Players/Client";
    public const string PrefabNpcEmployee = "Players/Employee";
    public const string PrefabInventoryItem = "Menu/InventoryItem";
    public const string MenuBackground = "MenuBackground";
    public const string MenuContainer = "MenuContainer";
    public const string PrefabInventoryItemImage = "Image";
    public const string PrefabInventoryItemTextPrice = "Image/TextBackground/Price";
    public const string BaseObjectUnderTile = "Object/Tiles/UnderTile";
    public const string BaseObjectActionTile = "Object/Tiles/ActionTile";
    public const string BaseObjectActionTile2 = "Object/Tiles/ActionTile2";
    public const string BaseObjectSpriteRenderer = "Object";

    // NPC Default
    public const float MinDistanceToTarget = 0.13f;
    public const float NpcDefaultMovementSpeed = 0.7f;
    public const int NpcDefaultEnergy = 100;
    public const string NpcEnergyBar = "EnergyBar";
    public const string NpcCharacter = "Character";

    // UI Constants
    public const string ConstNpcProfileMenu = "NPCProfile";
    public const string ConstCanvasParentMenu = "CanvasMenu";
    public const string ConstTopMenuDisplayMoney = "MoneyLabel";
    public const string ConstTopMenuDisplayGems = "GemMoneyLabel";
    public const string ConstTopMenuLevel = "LevelLabel";
    public const string ConstTopMenuExpSlider = "ExperienceSlider";
    public const string ConstCenterTabMenu = "CenterPanel";
    public const string ConstCenterScrollContent = "ViewPanel/MainContent/ScrollView/Viewport/Content";
    public const string ConstLeftDownPanel = "LeftDownPanel";
    public const string ConstEditStoreMenuPanel = "EditStoreMenu";
    public const string ConstEditItemMenuPanel = "EditItem";
    public const string ConstEditStoreMenuCancel = "Cancel";
    public const string ConstEditStoreMenuAccept = "Accept";
    public const string ConstEditStoreMenuRotateLeft = "ButtonRotateLeft";
    public const string ConstEditStoreMenuRotateRight = "ButtonRotateRight";
    public const string ConstEditStoreMenuSave = "ButtonSave";
    public const string ConstParentGameObject = "Game";
    public const int ConstDefaultCameraOrthographicsize = 7;
    public const int ConstDefaultBackgroundOrderingLevel = 200;

    // UI: Menu
    public const string ConstLeftDownMenuInventory = "Inventory";
    public const string ConstLeftDownMenuStore = "Store";

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

    //UI: Editor tools
    public const string DebugStartButton = "GridDebugButton";
    public const string GridDebugContent = "GridDebug";
    public const string GridDisplay = "GridDisplay";
    public const string MainContainer = "MainContainer";
    public const string IsometricWorldDebugUI = "Assets/Editor/IsometricWorldDebug.uxml";
    public const string IsometricWorldDebugUIStyles = "Assets/Editor/IsometricWorldDebug.uss";

    //SCENE
    public const string GameGrid = "GameGrid";
    public const string MainCamera = "MainCamera";

    //Tiles
    public const string GridTilesSimple = "Grid/Tiles/GridTile";
    public const string GridTilesHighlightedFloor = "Grid/Tiles/HighlightedFloor@3x";

    // Grid Config
    public const float GridCellSize = 0.25f; // 0.25f default
    public const int GridWidth = 25; // Number of cell per Grid CellSize
    public const int GridHeight = 20;
    public const int GridStartX = 0;
    public const int GrtGridStartY = 0;
}