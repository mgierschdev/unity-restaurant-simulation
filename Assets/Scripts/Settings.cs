public static class Settings
{

    // DEBUG parameters
    public const bool DEBUG_ENABLE = true; //Only for development
    public const int DEBUG_TEXT_SIZE = 9;

    // Player Config
    public const bool PLAYER_WALK_WITH_KEYBOARD = false;
    public const bool PLAYER_WALK_ON_CLICK = false;
    public const float PLAYER_MOVEMENT_SPEED = 1f;

    // TILEMAPS
    public const string TILEMAP_FLOOR_0 = "TilemapFloor0";
    public const string TILEMAP_COLLIDERS = "TilemapColliders";
    public const string TILEMAP_OBJECTS = "TilemapObjects";
    public const string PATH_FINDING_GRID = "PathFindingGrid";

    // PREFABS AND OBJECTS
    public const string PREFAB_GRID_TILE = "GridTile";
    public const string PREFAB_ISOMETRIC_PLAYER = "IsometricPlayer";
    public const string PREFAB_PLAYER = "Player";
    public const string PREFAB_ISOMETRIC_NPC = "IsometricNPC";
    public const string PREFAB_INVENTORY_ITEM = "InventoryItem";
    public const string PREFAB_SAND_SEA = "SandAndSea@3x";

    // NPC Default
    public const float NPC_DEFAULT_REACTION_TIME = 4;
    public const float NPC_DEFAULT_MOVEMENT_SPEED = 0.3f; // 0.3f
    public const float NPC_DEFAULT_RECOVERY_TIME = 10;
    public const int NPC_DEFAULT_ENERGY = 100;
    public const string NPC_ENERGY_BAR = "EnergyBar";
    public const bool NPC_ENERGY_ENABLED = false;

    // UI Constants
    public const string DEFAULT_LETTER_FONT = "Roboto-Regular";
    public const string CONST_NPC_PROFILE_MENU = "NPCProfile";
    public const string CONST_CANVAS_PARENT_MENU = "CanvasMenu";
    public const string CONST_CENTER_TAB_MENU = "CenterTabMenu";
    public const string CONST_CENTER_TAB_MENU_BODY = "MenuBody";
    public const string CONST_TOP_GAME_MENU = "TopGameMenu";
    public const string CONST_PARENT_GAME_OBJECT = "Game";
    public const int CONST_DEFAULT_CAMERA_ORTHOGRAPHICSIZE = 7;
    public const int CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL = 200;

    // UI: Camera
    public const bool CAMERA_PERSPECTIVE_HAND = true; //CAMERA_PERSPECTIVE_HAND or CAMERA_FOLLOW_PLAYER
    public static float[] CAMERA_PERSPECTIVE_HAND_CLAMP_X = { -0.5f, 3 };
    public static float[] CAMERA_PERSPECTIVE_HAND_CLAMP_Y = { -8, 0.5f };
    public const bool CAMERA_FOLLOW_PLAYER = false;
    public const float CAMERA_FOLLOW_INTERPOLATION = 0.034f;

    // UI : GameBackground
    public const string CONST_GAME_BACKGROUND_DEFAULT = "GameBackground";//E.g: Prefab GameBackground800x1920, GameBackground1300x1300
    public const int CONST_DEFAULT_CAMERA_WIDTH = 1500;
    public const int CONST_DEFAULT_CAMERA_HEIGHT = 1600;

    //UI: Buttons listeners
    public const string CONST_UI_EXIT_BUTTON = "ExitButton";
    public const string CONST_UI_INVENTORY_BUTTON = "Inventory";

    //SCENE
    public const string CONST_SCENE_MAIN = "World";
    public const string GAME_GRID = "GameGrid";

    // UI Tags
    // Grid Config
    public const float GRID_CELL_SIZE = 0.25f; // 0.25f default
    public const int GRID_WIDTH = 62; // Number of cell per Grid CellSize
    public const int GRID_HEIGHT = 62;
    public const int GRID_START_X = -5;
    public const int GRID_START_Y = -11;
}