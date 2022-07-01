public static class Settings
{
    // Player Display
    public const float NPC_REACTION_TIME = 4;
    public const float NPC_MOVEMENT_SPEED = 5;
    public const float PLAYER_MOVEMENT_SPEED = 10;
    // UI Constants
    public const bool PERSPECTIVE_HAND = false;
    public const string CONST_PARENT_MENU = "Canvas";
    public const string CONST_STORE_MENU = "StoreMenu";
    public const string CONST_CONFIG_MENU = "ConfigMenu";
    public const string CONST_TOP_GAME_MENU = "TopGameMenu";
    // We add other menus here like the store
    public enum Menu
    {
        CONFIG_MENU,
        STORE_MENU,
        TOP_GAME_MENU
    }
}
