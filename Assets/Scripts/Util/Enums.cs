//Item types
// to reference the type of object, each with different properties
public enum ObjectType
{
    OBSTACLE = 1,
    NPC = 2,
    PLAYER = 3,
    EMPLOYEE = 4,
    NPC_COUNTER = 5,
    FLOOR = 6,
    NPC_SINGLE_TABLE = 7,
    SINGLE_CONTAINER = 8,
    UNDEFINED = 999
}

public enum ObjectRotation
{
    FRONT = 1,
    FRONT_INVERTED = 2,
    BACK = 3,
    BACK_INVERTED = 4
}

// To reference from fileNames to object names
public enum TileType
{
    SPAM_POINT = 1,
    WALKABLE_PATH = 2,
    FLOOR_3 = 3,
    BUS_FLOOR = 4,
    FLOOR_OBSTACLE = 5,
    FLOOR_MEDIUM_HORIZONTAL_OBSTACLE = 6,
    FLOOR_MEDIUM_VERTICAL_OBSTACLE = 7,
    FLOOR_SHORT_HORIZONTAL_OBSTACLE = 8,
    FLOOR_SHORT_VERTICAL_OBSTACLE = 9,
    ISOMETRIC_GRID_TILE = 10,
    ISOMETRIC_SINGLE_SQUARE_OBJECT = 11,
    ISOMETRIC_FOUR_SQUARE_OBJECT = 12,
    WALL = 13,
    FLOOR_EDIT = 14,
    UNDEFINED = 999
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

//Players and NPCs, to set the NPC to wander or other states
public enum NpcState
{
    IDLE = 0,
    WALKING_TO_TABLE = 1,
    AT_TABLE = 2,
    WALKING_TO_COUNTER = 3,
    AT_COUNTER = 4,
    WALKING_WANDER = 5,
    TAKING_ORDER = 6,
    WAITING_TO_BE_ATTENDED = 7,
    WALKING_UNRESPAWN = 8,
    WALKING_TO_COUNTER_AFTER_ORDER = 9,
    REGISTERING_CASH = 10,
    WAITING_TO_BE_ATTENDED_ANIMATION = 11,
    WANDER = 12,
    WAITING_FOR_ENERGY_BAR_TAKING_ORDER = 13,
    WAITING_FOR_ENERGY_BAR_REGISTERING_CASH = 14,
    WALKING = 15,
    BEING_ATTENDED = 16,
    ATTENDED = 17
}

// List of Menus
public enum Menu
{
    CENTER_TAB_MENU,
    NPC_PROFILE
}

//Menu Types
public enum MenuType
{
    TAB_MENU,
    DIALOG
}

// Object deifnition in in MenuObjectList.cs
public enum StoreItemType
{
    WOODEN_TABLE_SINGLE = 1,
    SQUARED_WOODEN_TABLE_SINGLE = 2,
    TABLE_SINGLE_3 = 3,
    TABLE_SINGLE_4 = 4,
    TABLE_SINGLE_5 = 5,
    TABLE_SINGLE_6 = 6,
    TABLE_SINGLE_7 = 7,
    TABLE_SINGLE_8 = 8,
    TABLE_SINGLE_9 = 9,
    TABLE_SINGLE_10 = 10,
    COUNTER = 11,
    WOODEN_BASE_CONTAINER = 12,
    UNDEFINED = 999
}

// Auth source
public enum AuthSource
{
    GOOGLE_PLAY = 1,
    UNDEFINED = 999
}

public enum CharacterSide
{
    LEFT = 0,
    RIGHT = 1
}