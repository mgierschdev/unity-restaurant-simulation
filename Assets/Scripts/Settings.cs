public static class Settings
{
    // Player Display
    public const float PLAYER_MOVEMENT_SPEED = 3;
    // NPC Default
    public const int NPC_DEFAULT_ENERGY = 100;
    public const float NPC_DEFAULT_REACTION_TIME = 4;
    public const float NPC_DEFAULT_MOVEMENT_SPEED = 1.3f;
    public const float NPC_DEFAULT_MOVEMENT_INCREASE_ON_CLICK = 0.3f;
    public const float NPC_DEFAULT_RECOVERY_TIME = 10;
    // UI Constants
    public const bool PERSPECTIVE_HAND = true;
    public const string CONST_CANVAS_PARENT_MENU = "CanvasMenus";
    public const string CONST_STORE_MENU = "StoreMenu";
    public const string CONST_CONFIG_MENU = "ConfigMenu";
    public const string CONST_TOP_GAME_MENU = "TopGameMenu";
    public const string CONST_GAME_BACKGROUND = "GameBackground";
    public const int CONST_DEFAULT_CAMERA_WIDTH = 1080;
    public const int CONST_DEFAULT_CAMERA_HEIGHT = 1920;
    public const int CONST_DEFAULT_CAMERA_ORTHOGRAPHICSIZE = 7;
    public const int CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL = 200;
    // UI Tags
    public const string TAG_OBSTACLE = "Obstacle";
    // Grid Config
    public const bool GRID_ENABLED = true;
    public const int GRID_WIDTH = 8;
    public const int GRID_HEIGHT = 28;
    public const int GRID_START_X = -4;
    public const int GRID_START_Y = -21;
    // We add other menus here like the store
    public enum Menu
    {
        CONFIG_MENU,
        STORE_MENU,
        TOP_GAME_MENU
    }
}