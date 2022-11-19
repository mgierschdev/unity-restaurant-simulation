public static class Settings
{
    //DEBUG/DEV parameterss
    public const bool CellDebug = false;
    public const int DebugTextSize = 9;
    public const float NpcDefaultMovementSpeed = 3f;

    //Player Config
    public const bool PlayerWalkOnClick = true;
    public const float PlayerMovementSpeed = 3f;
    public const int MaxNpcNumber = 40; // 100 > fps 11-12
    public static int[] StartContainer = new int[] { 4, 13 },
    StartTable = new int[] { 4, 11 };

    //CAMERA ATTRIBUTES
    public static float CameraMovementSpeed = 25f,
    ZoomSpeed = 35,
    ZoomSpeedPinch = 8f,
    MinZoomSize = 0.5f, // the max zoom in
    MaxZoomSize = 2f, // the max zoom out
    MinTimeToEnablePerspectiveHand = 0.5f;
    public static int ConstDefaultCameraOrthographicsize = 7;

    //Init start attributes
    public static int InitGameMoney = 20000,
    InitGems = 40,
    InitExperience = 0,
    InitLevel = 0,
    InitGridSize = 10;

    //Sliders 
    public const float ObjectMoveSliderMultiplayer = 0.8f,
    ItemLoadSliderMultiplayer = 0.05f,
    ScreenLoadTime = 2f,
    TimeBeforeTheSliderIsEnabled = 0.2f;

    //Waiting times
    public const float NPCMaxWaitingTime = 10f,
    NPCMaxTimeInState = 10f,
    MinEuclidianDistanceRandomWalk = 10; // Performance relevant

    // Grid Config
    public const float GridCellSize = 0.25f; // 0.25f default
    public const int GridWidth = 25, // Number of cell per Grid CellSize
    GridHeight = 20,
    GridStartX = 0,
    GrtGridStartY = 0;

    //UI: Camera
    public const bool CameraPerspectiveHand = true; //CAMERA_PERSPECTIVE_HAND or CAMERA_FOLLOW_PLAYER
    public static float[] CameraPerspectiveHandClampX = { -20f, 20 },
    CameraPerspectiveHandClampY = { -20, 20f }; // X = -8, 1 || Y = Initial default -8, 1
    public const float CameraFollowInterpolation = 0.034f;

    //UI : Camera
    public const int ConstDefaultCameraWidth = 1500,
    ConstDefaultCameraHeight = 1600;

    //NPC Default
    public const float MinDistanceToTarget = 0.001f;
    public const int NpcDefaultEnergy = 100;
    public const string LoadSlider = "LoadSlider",
    TopPopUpObject = "InfoPopUp",
    NpcCharacter = "Character";

    //GAME TAGS
    public const string gameName = "Idle Tycoon - Business",
    NpcTag = "NPC",
    NpcEmployeeTag = "Employee",
    SliderTag = "Slider";

    //SCENES
    public const string LoadScene = "LoadScene",
    GameScene = "GameScene",
    SliderProgress = "SliderProgress";

    //STORE SPRITES
    public const string StoreSpritePath = "Objects/Sprites/";

    // TILEMAPS
    public const string TilemapFloor0 = "TilemapFloor0",
    TilemapColliders = "TilemapColliders",
    TilemapObjects = "TilemapObjects",
    TilemapWalkingPath = "TilemapWalkingPath",
    TilemapBusinessFloor = "TilemapBusinessFloor",
    TilemapBusinessFloor_2 = "TilemapBusinessFloor_2",
    TilemapBusinessFloor_3 = "TilemapBusinessFloor_3",
    TilemapBusinessFloor_4 = "TilemapBusinessFloor_4",
    TilemapBusinessFloor_5 = "TilemapBusinessFloor_5",
    TilemapBusinessFloor_6 = "TilemapBusinessFloor_6",
    TilemapBusinessFloor_7 = "TilemapBusinessFloor_7",
    TilemapBusinessFloor_8 = "TilemapBusinessFloor_8",
    TilemapBusinessFloor_9 = "TilemapBusinessFloor_9",
    TilemapBusinessFloor_10 = "TilemapBusinessFloor_10",
    PathFindingGrid = "PathFindingGrid";

    //OBJECTS PREFIXES
    public const string DiespenserPrefix = "DISPENSER",
    CounterPrefix = "COUNTER";

    //SPRITE LIBS CATEGORIES
    public const string SpriteLibCategoryTables = "Tables",
    SpriteLibCategoryDispensers = "Dispensers",
    SpriteLibCategoryStoreObjects = "StoreObjects";

    //PREFABS AND OBJECTS
    public const string Button = "Button",
    PrefabGridTile = "Grid/GridTile",
    PrefabSingleTable = "Objects/SingleTable",
    PrefabCounter = "Objects/Counter",
    PrefabBaseDispenser = "Objects/BaseDispenser",
    ObjectRotationFrontInverted = "Inverted",
    ObjectRotationFront = "Front",
    PrefabPlayer = "Players/Player",
    PrefabNpcClient = "Players/Client",
    PrefabNpcEmployee = "Players/Employee",
    PrefabInventoryItem = "Menu/InventoryItem",
    MenuBackground = "MenuBackground",
    MenuContainer = "MenuContainer",
    SideMenuButton = "Menu/CenterTabMenuBotton",
    PrefabInventoryItemImage = "Image",
    PrefabMenuInventoryItemImage = "Image/ItemImage",
    PrefabInventoryItemTextPrice = "Image/TextBackground/Price",
    BaseObjectUnderTile = "Object/Tiles/UnderTile",
    BaseObjectActionTile = "Object/Tiles/ActionTile",
    BaseObjectActionTile2 = "Object/Tiles/ActionTile2",
    BaseObjectSpriteRenderer = "Object",
    BaseObjectTopObject = "TopObject",
    TopObjectInfoSprite = "Sprites",
    undefined = "undefined";

    //UI Constants
    public const string ConstNpcProfileMenu = "NPCProfile",
    ConstCanvasParentMenu = "CanvasMenu",
    ConstTopMenuDisplayMoney = "MoneyLabel",
    ConstTopMenuDisplayGems = "GemMoneyLabel",
    ConstTopMenuLevel = "LevelLabel",
    ConstTopMenuExpSlider = "ExperienceSlider",
    ConstCenterTabMenu = "CenterPanel",
    ConstCenterScrollView = "ViewPanel/MainContent/ScrollView",
    ConstCenterScrollContent = "ViewPanel/MainContent/ScrollView/Viewport/Content",
    ConstLeftDownPanel = "LeftDownPanel",
    ConstEditItemMenuPanel = "EditPanelItem",
    ConstEditTopItemMenuPanel = "EditPanelTopItem",
    ConstEditStoreMenuCancel = "Cancel",
    ConstEditStoreMenuAccept = "Accept",
    ConstEditStoreMenuRotateLeft = "ButtonRotateLeft",
    ConstEditStoreMenuButtonAccept = "ButtonAccept",
    ConstEditStoreMenuButtonCancel = "ButtonCancel";

    public const string ConstEditStoreMenuSave = "ButtonSave",
    ConstParentGameObject = "Game";
    //UI: Menu
    public const string ConstLeftDownMenuStore = "Store";

    //UI: Buttons listeners
    //UI: Editor tools
    public const string DebugStartButton = "GridDebugButton",
    GridDebugContent = "GridDebug",
    GridDisplay = "GridDisplay",
    MainContainer = "MainContainer",
    ClientContainerGraphDebuger = "ClientContainerGraphDebuger",
    EmployeeContainerGraphDebuger = "EmployeeContainerGraphDebuger",
    ComboBoxContainer = "ComboBoxContainer",
    GraphLevel = "GraphLevel",
     NODE = "NODE",
    EMPLOYEE_PREFIX = "EMPLOYEE",
    IsometricWorldDebugUI = "Assets/Editor/IsometricWorldDebug.uxml",
    IsometricWorldDebugUIStyles = "Assets/Editor/IsometricWorldDebug.uss",
    IsometricWorldDebugUIStateMachine = "Assets/Editor/StatemachineWorldDebug.uxml",
    IsometricWorldDebugUIStylesStateMachine = "Assets/Editor/StatemachineWorldDebug.uss";

    //SCENE
    public const string MainCamera = "MainCamera";

    //Tiles
    public const string GridTilesSimple = "Grid/Tiles/GridTile",
    GridTilesHighlightedFloor = "Grid/Tiles/HighlightedFloor@3x";
}