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