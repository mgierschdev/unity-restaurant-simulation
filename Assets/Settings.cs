public static class Settings
{
    // General Config
    public const bool CellDebug = false;
    public const int DebugTextSize = 9;
    public const float NpcDefaultMovementSpeed = 1.3f; //1 = Looks good with current animation

#if UNITY_EDITOR
    public const bool devEnv = true;
#else
    public const bool devEnv = false;
#endif

    public const int MaxNpcNumber = 10; // 100 > fps 11-12
    public static int[] StartStoreItemDispenser = new int[] { 5, 14 },
    StartTable = new int[] { 5, 12 },
    StartCounter = new int[] { 3, 13 };

    //CAMERA ATTRIBUTES
    public static float CameraMovementSpeed = 25f,
    ZoomSpeed = 35,
    ZoomSpeedPinch = 8f,
    MinZoomSize = 0.5f, // the max zoom in
    MaxZoomSize = 2f; // the max zoom out

    public static int ConstDefaultCameraOrthographicsize = 7;

    //Init start attributes
    public static int InitGameMoney = 0,
    InitGems = 10,
    InitExperience = 0,
    InitLevel = 0,
    InitGridSize = 1;

    //Sliders 
    public const float ObjectMoveSliderMultiplayer = 0.8f,
    ItemLoadSliderMultiplayer = 0.05f, // TODO: To be added to the object store properties
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

    // Save Data directory
    public const string DevSaveDirectory = "UserData";
    public const string SaveFileSuffix = "save.json";

    //NPC Default
    public const float MinDistanceToTarget = 0.001f;
    public const int NpcDefaultEnergy = 100;
    public const string LoadSlider = "LoadSlider",
    TopPopUpObject = "InfoPopUp",
    NpcCharacter = "Character",
    TryObject = "Character/Arm-Left-1/Try";

    //Unity services project environments, as displayed on the unity service console
    public const string UnityServicesProd = "production",
    UnityServicesDev = "development",
    UnityServicesPreProd = "preproduction";

    //GAME TAGS
    public const string gameName = "Idle Tycoon - Business",
    NpcTag = "NPC",
    NpcEmployeeTag = "Employee",
    SliderTag = "Slider";

    //SCENES
    public const string LoadScene = "LoadScene",
    GameScene = "GameScene";

    //STORE SPRITES
    public const string StoreSpritePath = "Sprites/",
    DefaultSquareSprite = "Sprite-Default";

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
    TilemapBusinessFloor_Decoration = "TilemapBusinessFloor_Decoration",
    PathFindingGrid = "PathFindingGrid";

    //SPRITE LIBS CATEGORIES
    public const string SpriteLibCategoryTables = "Tables",
    SpriteLibCategoryStoreItems = "Store",
    SpriteLibCategoryUpgradeItems = "Upgrade",
    SpriteLibCategoryStoreObjects = "StoreObjects";

    //PREFABS AND OBJECTS
    public const string Button = "Button",
    PrefabGridTile = "Grid/GridTile",
    PrefabSingleTable = "Objects/SingleTable",
    PrefabCounter = "Objects/Counter",
    PrefabBaseStoreItem = "Objects/BaseStoreItem",
    PrefabPlayer = "Players/Player",
    PrefabNpcClient = "Players/Client",
    PrefabNpcEmployee = "Players/Employee",
    PrefabInventoryItem = "Menu/InventoryItem",
    PrefabUpgradeInventoryItem = "Menu/UpgradeInventoryItem",
    PrefabSettingsItem = "Menu/SettingsItem",
    MenuBackground = "MenuBackground",
    MenuContainer = "MenuContainer",
    SideMenuButton = "Menu/CenterTabMenuBotton",
    SettingsMenuSaveButton = "SaveItem/SaveButton",
    SettingsMenuStatsText = "StatsContent/StatsText",
    PrefabInventoryItemImage = "Image",
    PrefabMenuInventoryItemImage = "Image/ItemImage",
    PrefabInventoryItemTextTitle = "Image/TextBackgroundTitle/Title",
    PrefabInventoryItemTextPrice = "Image/TextBackground/Price",
    PrefabUpgradeItemTextPrice = "Image/TextBackground/Price",
    PrefabUpgradeLevelItemTextPrice = "Image/TextBackground/Level",
    BaseObjectUnderTile = "Object/Tiles/UnderTile",
    BaseObjectActionTile = "Object/Tiles/ActionTile",
    BaseObjectActionTile2 = "Object/Tiles/ActionTile2",
    BaseObjectSpriteRenderer = "Object",
    TopObjectInfoSprite = "Sprites";

    //UI Constants
    public const string ConstNpcProfileMenu = "NPCProfile",
    ConstCanvasParentMenu = "CanvasMenu",
    ConstTopMenuDisplayMoney = "MoneyLabel",
    ConstTopMenuDisplayGems = "GemMoneyLabel",
    ConstTopMenuLevel = "LevelLabel",
    ConstTopMenuExpSlider = "ExperienceSlider",
    ConstCenterTabMenu = "CenterPanel",
    ConstCenterScrollView = "ViewPanel/MainContent/ScrollView",
    ConstCenterMenuPanelTitle = "ViewPanel/TitlePanel/ViewPanelTitleLabel",
    ConstCenterScrollContent = "ViewPanel/MainContent/ScrollView/Viewport/Content",
    ConstButtonMenuPanel = "ButtonMenuPanel",
    ConstLeftDownPanel = "LeftDownPanel",
    ConstEditItemMenuPanel = "EditPanelItem",
    ConstEditStoreMenuRotateLeft = "ButtonRotateLeft",
    ConstEditStoreMenuButtonAccept = "ButtonAccept",
    ConstEditStoreMenuButtonCancel = "ButtonCancel";
    public const string ConstEditStoreMenuSave = "ButtonSave",
    ConstParentGameObject = "Game";

    //UI: Menu
    public const string ConstLeftDownMenuStore = "Store";
    public const int StoreCellSizeX = 600,
    StoreCellSizeY = 600,
    SettingsCellSizeX = 1000,
    SettingsCellSizeY = 1200;

    //UI: Buttons listeners
    //UI: Editor tools
    public const string DebugStartButton = "GridDebugButton",
    GridDebugContent = "GridDebug",
    GridDisplay = "GridDisplay",
    MainContainer = "MainContainer",
    ClientContainerGraphDebuger = "ClientContainerGraphDebuger",
    EmployeeContainerGraphDebuger = "EmployeeContainerGraphDebuger",
    ComboBoxContainer = "ComboBoxContainer",
    EMPLOYEE_PREFIX = "EMPLOYEE",
    IsometricWorldDebugUI = "Assets/Editor/IsometricWorldDebug.uxml",
    IsometricWorldDebugUIStyles = "Assets/Editor/IsometricWorldDebug.uss",
    IsometricWorldDebugUIStateMachine = "Assets/Editor/StatemachineWorldDebug.uxml",
    IsometricWorldDebugUIStylesStateMachine = "Assets/Editor/StatemachineWorldDebug.uss";

    //Tiles
    public const string GridTilesSimple = "Sprites/Tiles/GridTile",
    GridTilesFloorBrown = "Sprites/Floor-Brown";
}