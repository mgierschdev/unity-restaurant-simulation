public static class Settings
{
    //DEBUG/DEV parameterss
    public const bool IsFirebaseEmulatorEnabled = true;
    public const bool CellDebug = true;
    public const int DebugTextSize = 9;
    public const float NpcDefaultMovementSpeed = 1f;

    //Player Config
    public const bool PlayerWalkOnClick = true;
    public const float PlayerMovementSpeed = 3f;
    public const int  MaxNpcNumber = 1; // Performance relevant
    public static int[] StartContainer = new int[] { 4, 13 };
    public static int[] StartTable = new int[] { 4, 11 };

    //Sliders 
    public const float ObjectMoveSliderMultiplayer = 0.8f;
    public const float ItemLoadSliderMultiplayer = 0.05f;
    public const float ScreenLoadTime = 2f;
    public const float TimeBeforeTheSliderIsEnabled = 0.4f;

    //Waiting times
    public const float NPCMaxWaitingTime = 10f;
    public const float NPCMaxTimeInState = 10f;
    public const double MIN_EUCLIDIAN_DISTANCE_RANDOM_WALK = 10; // Performance relevant

    //FIREBASE TEST ENV 
    public const string FIRESTORE_HOST = "localhost:8080";
    public const string CLOUD_FUNCTION_HOST = IsFirebaseEmulatorEnabled ? "localhost:5001" : "";
    public const string USER_PRED_PROD_COLLECTION = "PreProd";
    public const string TEST_USER = "TESTUSERID";

    //GAME TAGS
    public const string gameName = "CafeMadness";
    public const string NpcTag = "NPC";
    public const string NpcEmployeeTag = "Employee";
    public const string SliderTag = "Slider";

    //SCENES
    public const string LoadScene = "LoadScene";
    public const string GameScene = "GameScene";
    public const string SliderProgress = "SliderProgress";

    //STORE SPRITES
    public const string StoreSpritePath = "Objects/Sprites/";

    // TILEMAPS
    public const string TilemapFloor0 = "TilemapFloor0";
    public const string TilemapColliders = "TilemapColliders";
    public const string TilemapObjects = "TilemapObjects";
    public const string TilemapWalkingPath = "TilemapWalkingPath";
    public const string TilemapBusinessFloor = "TilemapBusinessFloor";
    public const string TilemapBusinessFloor_2 = "TilemapBusinessFloor_2";
    public const string TilemapBusinessFloor_3 = "TilemapBusinessFloor_3";
    public const string TilemapBusinessFloor_4 = "TilemapBusinessFloor_4";
    public const string TilemapBusinessFloor_5 = "TilemapBusinessFloor_5";
    public const string TilemapBusinessFloor_6 = "TilemapBusinessFloor_6";
    public const string TilemapBusinessFloor_7 = "TilemapBusinessFloor_7";
    public const string TilemapBusinessFloor_8 = "TilemapBusinessFloor_8";
    public const string TilemapBusinessFloor_9 = "TilemapBusinessFloor_9";
    public const string TilemapBusinessFloor_10 = "TilemapBusinessFloor_10";
    public const string PathFindingGrid = "PathFindingGrid";

    //SPRITE LIBS CATEGORIES
    public const string SpriteLibCategoryTables = "Tables";
    public const string SpriteLibCategoryContainers = "BaseContainers";
    public const string SpriteLibCategoryCoffeMachines = "CoffeMachines";
    public const string SpriteLibCategoryStoreObjects = "StoreObjects";

    //PREFABS AND OBJECTS
    public const string Button = "Button";
    public const string PrefabGridTile = "Grid/GridTile";
    public const string PrefabSingleTable = "Objects/SingleTable";
    public const string PrefabBaseContainerItem = "Objects/ContainerItem";
    public const string PrefabCounter = "Objects/Counter";
    public const string PrefabBaseContainer = "Objects/BaseContainer";
    public const string ObjectRotationFrontInverted = "Inverted";
    public const string ObjectRotationFront = "Front";
    public const string PrefabPlayer = "Players/Player";
    public const string PrefabNpcClient = "Players/Client";
    public const string PrefabNpcEmployee = "Players/Employee";
    public const string PrefabInventoryItem = "Menu/InventoryItem";
    public const string MenuBackground = "MenuBackground";
    public const string MenuContainer = "MenuContainer";
    public const string SideMenuButton = "Menu/CenterTabMenuBotton";
    public const string PrefabInventoryItemImage = "Image";
    public const string PrefabInventoryItemTextPrice = "Image/TextBackground/Price";
    public const string BaseObjectUnderTile = "Object/Tiles/UnderTile";
    public const string BaseObjectActionTile = "Object/Tiles/ActionTile";
    public const string BaseObjectActionTile2 = "Object/Tiles/ActionTile2";
    public const string BaseObjectSpriteRenderer = "Object";
    public const string BaseObjectTopObject = "TopObject";
    public const string undefined = "undefined";

    //NPC Default
    public const float MinDistanceToTarget = 0.001f;
    public const int NpcDefaultEnergy = 100;
    public const string NpcEnergyBar = "EnergyBar";
    public const string NpcCharacter = "Character";

    //UI Constants
    public const string ConstNpcProfileMenu = "NPCProfile";
    public const string ConstCanvasParentMenu = "CanvasMenu";
    public const string ConstTopMenuDisplayMoney = "MoneyLabel";
    public const string ConstTopMenuDisplayGems = "GemMoneyLabel";
    public const string ConstTopMenuLevel = "LevelLabel";
    public const string ConstTopMenuExpSlider = "ExperienceSlider";
    public const string ConstCenterTabMenu = "CenterPanel";
    public const string ConstCenterScrollView = "ViewPanel/MainContent/ScrollView";
    public const string ConstCenterScrollContent = "ViewPanel/MainContent/ScrollView/Viewport/Content";
    public const string ConstLeftDownPanel = "LeftDownPanel";
    public const string ConstEditItemMenuPanel = "EditPanelItem";
    public const string ConstEditTopItemMenuPanel = "EditPanelTopItem";
    public const string ConstEditStoreMenuCancel = "Cancel";
    public const string ConstEditStoreMenuAccept = "Accept";
    public const string ConstEditStoreMenuRotateLeft = "ButtonRotateLeft";
    public const string ConstEditStoreMenuButtonAccept = "ButtonAccept";
    public const string ConstEditStoreMenuButtonCancel = "ButtonCancel";

    public const string ConstEditStoreMenuSave = "ButtonSave";
    public const string ConstParentGameObject = "Game";
    public const int ConstDefaultCameraOrthographicsize = 7;
    public const int ConstDefaultBackgroundOrderingLevel = 200;

    //UI: Menu
    public const string ConstLeftDownMenuStore = "Store";

    //UI: Camera
    public const bool CameraPerspectiveHand = true; //CAMERA_PERSPECTIVE_HAND or CAMERA_FOLLOW_PLAYER
    public static float[] CameraPerspectiveHandClampX = { -20f, 20 };
    public static float[] CameraPerspectiveHandClampY = { -20, 20f }; // X = -8, 1 || Y = Initial default -8, 1
    public const float CameraFollowInterpolation = 0.034f;

    //UI : Camera
    public const int ConstDefaultCameraWidth = 1500;
    public const int ConstDefaultCameraHeight = 1600;

    //UI: Buttons listeners
    //UI: Editor tools
    public const string DebugStartButton = "GridDebugButton";
    public const string GridDebugContent = "GridDebug";
    public const string GridDisplay = "GridDisplay";
    public const string MainContainer = "MainContainer";
    public const string ClientContainerGraphDebuger = "ClientContainerGraphDebuger";
    public const string EmployeeContainerGraphDebuger = "EmployeeContainerGraphDebuger";
    public const string ComboBoxContainer = "ComboBoxContainer";
    public const string GraphLevel = "GraphLevel";
    public const string NODE = "NODE";
    public const string EMPLOYEE_PREFIX = "EMPLOYEE";
    public const string IsometricWorldDebugUI = "Assets/Editor/IsometricWorldDebug.uxml";
    public const string IsometricWorldDebugUIStyles = "Assets/Editor/IsometricWorldDebug.uss";
    public const string IsometricWorldDebugUIStateMachine = "Assets/Editor/StatemachineWorldDebug.uxml";
    public const string IsometricWorldDebugUIStylesStateMachine = "Assets/Editor/StatemachineWorldDebug.uss";

    //SCENE
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