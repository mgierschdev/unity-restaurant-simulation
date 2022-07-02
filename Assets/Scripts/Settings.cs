public static class Settings
{
    // Player Display
    public const float NPC_REACTION_TIME = 4;
    public const float NPC_MOVEMENT_SPEED = 1.3f;
    public const float NPC_MOVEMENT_INCREASE_ON_CLICK = 0.3f;
    public const float PLAYER_MOVEMENT_SPEED = 3;
    // UI Constants
    public const bool PERSPECTIVE_HAND = true;
    public const string CONST_CANVAS_PARENT_MENU = "CanvasMenus";
    public const string CONST_STORE_MENU = "StoreMenu";
    public const string CONST_CONFIG_MENU = "ConfigMenu";
    public const string CONST_TOP_GAME_MENU = "TopGameMenu";
    // UI Tags
    public const string TAG_OBSTACLE = "Obstacle";
    public const string TAG_CAMARA_OBSTACLE = "MainCamera";
    // We add other menus here like the store
    public enum Menu
    {
        CONFIG_MENU,
        STORE_MENU,
        TOP_GAME_MENU
    }
}