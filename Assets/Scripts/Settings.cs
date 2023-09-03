public static class Settings
{
    // General Config
    public const bool CellDebug = false;
    public const int DebugTextSize = 9;
    public const bool DisableNetwork = true; // for dev backend

    // Upgrades
    public const float NpcDefaultMovementSpeed = 1.3f, //1 = Looks good with current animation
        UpgradePercentageMultiplayer = 0.08f,
        DefaultItemLoadSpeed = 5f, // in seconds
        ItemLoadSliderMultiplayer = 0.05f,
        SpeedToMoveObjects = 1.3f, // in seconds
        MaxStateTime = 300, // Increases 10% per upgrade InitClientWaitTime += 10 * InitClientWaitTime / 100;
        OrderIncreaseCostPercentage = 2; // Max value 5, total increase more 10%  

    public const int NpcMultiplayer = 2; // multiplayer * upgrade = Npc number (3), more clients limit (4)
    public const double PlayerMoneyLimit = 1000000;

    // Time to retry internet connection
    public const float TimeToRetryConnection = 10;

    // Message controller 
    public const string
        CanvasMessageObject = "Canvas/Background/Message",
        MessageTextObject = "Body/Message",
        MessageImageObject = "Body/Image",
        MessageRetryButton = "Body/Button";

    //CAMERA ATTRIBUTES
    public const float CameraMovementSpeed = 25f; // the max zoom out
    public const float ZoomSpeed = 35; // the max zoom out
    public const float ZoomSpeedPinch = 8f; // the max zoom out
    public const float MinZoomSize = 1.3f; // the max zoom out
    public const float MaxZoomSize = 2f; // the max zoom out
    public const int ConstDefaultCameraOrthographicsize = 7;

    //Init start attributes: Devlopment
    public const int InitGameMoney = 1000000;

    public static readonly int InitExperience = 0;

    public static readonly int InitLevel = 0;

    public static readonly int InitGridSize = 1;

    // Init Objects
    public static readonly int[] StartStoreItemDispenser = new int[] { 15, 14 };

    public static readonly int[] StartTable = new int[] { 15, 12 };

    public static readonly int[] StartCounter = new int[] { 13, 13 };

    //Sliders 
    public const float ObjectMoveSliderMultiplayer = 0.8f,
        ScreenLoadTime = 2f,
        TimeBeforeTheSliderIsEnabled = 0.2f;

    //Waiting times
    public const float NpcMaxWaitingTime = 10f,
        NpcMaxTimeInState = 10f,
        MinEuclideanDistanceRandomWalk = 10,
        MinTimeEating = 1f,
        MaxTimeEating = 2f; // Performance relevant

    // Grid Config
    public const float GridCellSize = 0.25f; // 0.25f default

    public const int GridWidth = 40, // Number of cell per Grid CellSize
        GridHeight = 40;

    //UI: Camera
    public const bool CameraPerspectiveHand = true; //CAMERA_PERSPECTIVE_HAND or CAMERA_FOLLOW_PLAYER
    public static readonly float[] CameraPerspectiveHandClampX = { -4.5f, 10f }; // up / down

    public static readonly float[] // left / right
        CameraPerspectiveHandClampY = { 4.5f, 13f }; // up / down

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
        TryObject = "Try";

    //Unity services project environments, as displayed on the unity service console
    public const string UnityServicesProd = "production",
        UnityServicesDev = "development",
        UnityServicesPreProd = "preproduction";

    //GAME TAGS
    public const string GameName = "Idle Tycoon - Business",
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
    public const string TilemapSpamFloor = "TilemapSpamFloor",
        TilemapColliders = "TilemapColliders",
        TilemapObjects = "TilemapObjects",
        TilemapWalkingPath = "TilemapWalkingPath",
        TilemapBusinessFloor = "TilemapBusinessFloor",
        TilemapBusinessFloor2 = "TilemapBusinessFloor_2",
        TilemapBusinessFloor3 = "TilemapBusinessFloor_3",
        TilemapBusinessFloor4 = "TilemapBusinessFloor_4",
        TilemapBusinessFloor5 = "TilemapBusinessFloor_5",
        TilemapBusinessFloor6 = "TilemapBusinessFloor_6",
        TilemapBusinessFloor7 = "TilemapBusinessFloor_7",
        TilemapBusinessFloor8 = "TilemapBusinessFloor_8",
        TilemapBusinessFloor9 = "TilemapBusinessFloor_9",
        TilemapBusinessFloor10 = "TilemapBusinessFloor_10",
        TilemapBusinessFloorDecoration = "TilemapBusinessFloor_Decoration",
        TilemapBusinessWallDecoration = "TilemapBusinessWalls_Decoration",
        PathFindingGrid = "PathFindingGrid";

    //SPRITE LIBS CATEGORIES
    public const string SpriteLibCategoryTables = "Tables",
        SpriteLibCategoryStoreItems = "Store",
        SpriteLibCategoryUpgradeItems = "Upgrade",
        SpriteLibCategoryStoreObjects = "StoreObjects",
        SpriteLibCategoryKitchenObjects = "kitchens";

    //PREFABS AND OBJECTS
    public const string Button = "Button",
        PrefabSingleTable = "Objects/SingleTable",
        PrefabCounter = "Objects/Counter",
        PrefabSnackMachine = "Objects/SnackMachine",
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
        PrefabUpgradeItemTextPrice = "Image/TextBackgroundPrice/Price",
        PrefabUpgradeLevelItemTextPrice = "Image/TextBackgroundLevel/Level",
        BaseObjectUnderTile = "Object/Tiles/UnderTile",
        BaseObjectActionTile = "Object/Tiles/ActionTile",
        BaseObjectActionTile2 = "Object/Tiles/ActionTile2",
        BaseObjectSpriteRenderer = "Object",
        TopObjectInfoSprite = "Sprites";

    // Character spritelib categories parts
    public const string CharacterObjectName = "Character";

    public const string CategoryHeads = "Heads",
        CategoryBodies = "Bodies",
        CategoryLegs = "Legs",
        CategoryShoes = "Shoes",
        CategoryWaist = "Waist",
        CategoryArms = "Arms";

    //UI Constants
    public const string ConstNpcProfileMenu = "NPCProfile",
        ConstCanvasParentMenu = "CanvasMenu",
        ConstTopMenuDisplayMoney = "MoneyLabel",
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
        ClientContainerGraphDebugger = "ClientContainerGraphDebuger",
        EmployeeContainerGraphDebugger = "EmployeeContainerGraphDebuger",
        ComboBoxContainer = "ComboBoxContainer",
        EmployeePrefix = "EMPLOYEE",
        IsometricWorldDebugUI = "Assets/Editor/IsometricWorldDebug.uxml",
        IsometricWorldDebugUIStyles = "Assets/Editor/IsometricWorldDebug.uss",
        IsometricWorldDebugUIStateMachine = "Assets/Editor/StatemachineWorldDebug.uxml",
        IsometricWorldDebugUIStylesStateMachine = "Assets/Editor/StatemachineWorldDebug.uss";

    //Tiles
    public const string GridTilesSimple = "Sprites/GridTile",
        GridTilesFloorBrown = "Sprites/Floor-Brown-Tile";

    public static readonly string[] BussWalls = new string[]
    {
        "Sprites/Wall-Front-Brown-End",
        "Sprites/Wall-Front-Brown",
        "Sprites/Wall-Corner-Brown",
        "Sprites/Wall-Rotated-Brown",
        "Sprites/Wall-Rotated-Brown-End"
    };
}