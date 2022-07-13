public static class Settings
{
    // DEBUG parameters
    public const bool DEBUG_ENABLE = true; //Only for development
    public const int DEBUG_TEXT_SIZE = 30;
    public const int DEBUG_DEBUG_LINE_DURATION = 1000; //in seconds
    // Player Display
    public const float PLAYER_MOVEMENT_SPEED = 3;
    // PREFABS
    public const string PREFAB_PATH = "Resources";
    public const string PREFAB_PLAYER = "Player";
    public const string PREFAB_NPC = "NPC";
    public const string PREFAB_OBSTACLE = "Obstacle";
    public const string PREFAB_GAME_GRID = "GameGrid";
    // NPC Default
    public const float NPC_DEFAULT_REACTION_TIME = 4;
    public const float NPC_DEFAULT_MOVEMENT_SPEED = 10.3f; // Default 1.3
    public const float NPC_DEFAULT_MOVEMENT_INCREASE_ON_CLICK = 0.3f;
    public const float NPC_DEFAULT_RECOVERY_TIME = 10;
    public const int NPC_DEFAULT_ENERGY = 100;
    public const string NPC_ENERGY_BAR = "EnergyBar";
    public const bool NPC_ENERGY_ENABLED = false;
    // UI Constants
    public const bool PERSPECTIVE_HAND = true;
    public const string CONST_CANVAS_PARENT_MENU = "CanvasMenus";
    public const string CONST_STORE_MENU = "StoreMenu";
    public const string CONST_CsONFIG_MENU = "ConfigMenu";
    public const string CONST_TOP_GAME_MENU = "TopGameMenu";
    public const int CONST_DEFAULT_CAMERA_ORTHOGRAPHICSIZE = 7;
    public const int CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL = 200;
    // UI : GameBackground
    public const string CONST_GAME_BACKGROUND_DEFAULT = "GameBackground";//E.g: Prefab GameBackground800x1920, GameBackground1300x1300
    public const int CONST_DEFAULT_CAMERA_WIDTH = 1500;
    public const int CONST_DEFAULT_CAMERA_HEIGHT = 1600;
    public const int CONST_CAMERA_CLAMP_X = 3; // Both sides -3, 3 in grid UNITS
    public const int CONST_CAMERA_CLAMP_Y = -1; // -1, 0

    // UI Tags
    // Grid Config
    public const int GRID_WIDTH = 16;
    public const int GRID_HEIGHT = 18;
    public const int GRID_START_X = -8; // in grid UNITS
    public const int GRID_START_Y = -9; // in grid UNITS
}

//Item types
public enum ObjectType
{
    OBSTACLE = 1,
    NPC = 2,
    PLAYER = 3
}

//Players and NPCs move directions
public enum MoveDirection
{
    IDLE = 0,
    UP = 1,
    DOWN = 2,
    LEFT = 3,
    RIGHT = 4,
    UPLEFT = 5,
    UPRIGHT = 6,
    DOWNLEFT = 7,
    DOWNRIGHT = 8
}

// We add other menus here like the store
public enum Menu
{
    CONFIG_MENU,
    STORE_MENU,
    TOP_GAME_MENU
}