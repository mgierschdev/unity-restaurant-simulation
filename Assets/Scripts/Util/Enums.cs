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
    BASE_CONTAINER = 8,
    CONTAINER_ITEM = 9,
    UNDEFINED = 999
}

// it should preserve the number order since it is used on the Firestore database
public enum ObjectRotation
{
    FRONT = 1,
    FRONT_INVERTED = 2,
    BACK = 3,
    BACK_INVERTED = 4,
    UNDEFINED = 999
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

// NPC transition attributes
public enum NpcStateTransitions
{
    TABLE_AVAILABLE = 0,
    TABLE_MOVED = 1,
    WANDER_TIME = 2,
    WAITING_AT_TABLE_TIME = 3,
    IDLE_TIME = 4,
    ORDER_SERVED = 5,
    ORDER_FINISHED = 6,
    ENERGY_BAR_VALUE = 7,
    COUNTER_MOVED = 8
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
public enum MenuTab
{
    TABLES_TAB = 1,//COUNTER HERE
    BASE_CONTAINER_TAB = 2,
    ITEMS_TAB = 3,
    EMPLOYEE_TAB = 4,//YOU MOST HAVE A COUNTER
    IN_GAME_STORE_TAB = 5,
    STORAGE_TAB = 6,
    SETTINGS_TAB = 999
}

//Menu Types
public enum MenuType
{
    TAB_MENU,
    DIALOG
}

// Object deifnition in in MenuObjectList.cs
// it should preserve the number order since it is used on the Firestore database
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
    ITEM_COFFE_MACHINE_1 = 13,
    UNDEFINED = 999
}

// Auth source
// it should preserve the number order since it is used on the Firestore database
public enum AuthSource
{
    GOOGLE_PLAY = 1,
    ANONYMOUS = 2,
    UNDEFINED = 999
}

public enum CharacterSide
{
    LEFT = 0,
    RIGHT = 1
}

public enum CellValue
{
    NPC_POSITION = -2,
    ACTION_POINT = -1,
    EMPTY = 0,
    BUSY = 1,
    VISITED = 2 //For DFS operations,
}