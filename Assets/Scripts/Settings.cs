public static class Settings
{
    // Player Config
    public const bool PLAYER_WALK_WITH_KEYBOARD = false;
    public const bool PLAYER_WALK_ON_CLICK = false;
    public const float PLAYER_MOVEMENT_SPEED = 1f;

    // DEBUG parameters
    public const bool DEBUG_ENABLE = true; //Only for development
    public const int DEBUG_TEXT_SIZE = 9;
    public const int DEBUG_DEBUG_LINE_DURATION = 1000; //in seconds

    // PREFABS
    public const string PREFAB_PATH = "Resources";
    public const string PREFAB_PLAYER = "Player";
    public const string PREFAB_NPC = "NPC";
    public const string PREFAB_OBSTACLE = "Obstacle";
    public const string PREFAB_GAME_GRID = "GameGrid";

    // NPC Default
    public const float NPC_DEFAULT_REACTION_TIME = 4;
    public const float NPC_DEFAULT_MOVEMENT_SPEED = 0.3f; // 0.3f
    public const float NPC_DEFAULT_MOVEMENT_INCREASE_ON_CLICK = 0.3f;
    public const float NPC_DEFAULT_RECOVERY_TIME = 10;
    public const int NPC_DEFAULT_ENERGY = 100;
    public const string NPC_ENERGY_BAR = "EnergyBar";
    public const bool NPC_ENERGY_ENABLED = false;

    // UI Constants
    public const string DEFAULT_LETTER_FONT = "Roboto-Regular";
    public const string CONST_CANVAS_PARENT_MENU = "CanvasMenu";
    public const string CONST_CENTER_TAB_MENU = "CenterTabMenu";
    public const string CONST_TOP_GAME_MENU = "TopGameMenu";
    public const int CONST_DEFAULT_CAMERA_ORTHOGRAPHICSIZE = 7;
    public const int CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL = 200;

    // UI: Camera
    public const bool CAMERA_PERSPECTIVE_HAND = true; //CAMERA_PERSPECTIVE_HAND or CAMERA_FOLLOW_PLAYER
    public const int CAMERA_PERSPECTIVE_HAND_CLAMP = 9;
    public const bool CAMERA_FOLLOW_PLAYER = false;
    public const float CAMERA_FOLLOW_INTERPOLATION = 0.034f;

    // UI : GameBackground
    public const string CONST_GAME_BACKGROUND_DEFAULT = "GameBackground";//E.g: Prefab GameBackground800x1920, GameBackground1300x1300
    public const int CONST_DEFAULT_CAMERA_WIDTH = 1500;
    public const int CONST_DEFAULT_CAMERA_HEIGHT = 1600;
    public const int CONST_CAMERA_CLAMP_X = 3; // Both sides -3, 3 in grid UNITS
    public const int CONST_CAMERA_CLAMP_Y = -1; // -1, 0
    public const int DEFAULT_GAME_OBJECTS_Z = 1;

    //UI: Buttons listeners
    public const string CONST_UI_EXIT_BUTTON = "ExitButton";
    public const string CONST_UI_INVENTORY_BUTTON = "Inventory";

    //SCENE
    public const string CONST_SCENE_MAIN = "World";

    // UI Tags
    // Grid Config
    public const float GRID_CELL_SIZE = 0.50f; // 0.25f default
    public const int GRID_WIDTH = 30; // Number of cell of the Grid CellSize
    public const int GRID_HEIGHT = 30;
    public const int GRID_START_X = -9;
    public const int GRID_START_Y = -9;
}

//Item types
public enum ObjectType
{
    OBSTACLE = 1,
    NPC = 2,
    PLAYER = 3
}

public enum TileType
{
    FLOOR = 1
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

//Players and NPCs move directions
public enum NPCState
{
    IDLE = 0,
    WANDER = 1
}

// List of Menus
public enum Menu
{
    CENTER_TAB_MENU,
    TOP_MENU
}

//Menu Types
public enum MenuType
{
    TAB_MENU,
    DIALOG,
    ON_SCREEN
}

public enum Tabs
{
    CONFIG_TAB,
    ITEMS_TAB
}