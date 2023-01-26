//Item types
// to reference the type of object, each with different properties
public enum ObjectType
{
    OBSTACLE = 1,
    CLIENT = 2,
    PLAYER = 3,
    EMPLOYEE = 4,
    NPC_COUNTER = 5,
    FLOOR = 6,
    NPC_SINGLE_TABLE = 7,
    STORE_ITEM = 8,
    UPGRADE_ITEM = 9,
    COIN = 10,
    UNDEFINED = 999
}

// it should preserve the number order since it is used on the backend database
// Front = Facing Unity camera
// Back = Opposite side from the camera
public enum ObjectRotation
{
    BACK = 1,
    BACK_INVERTED = 2,
    FRONT = 3,
    FRONT_INVERTED = 4,
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
    WALK_TO_UNRESPAWN = 2,
    WAITING_AT_TABLE = 3,
    COUNTER_AVAILABLE = 4,
    ORDER_SERVED = 5,
    AT_COUNTER = 6,
    AT_TABLE = 7,
    COUNTER_MOVED = 8,
    WANDER = 9,
    REGISTERING_CASH = 10,
    ATTENDED = 11,
    BEING_ATTENDED = 12,
    CASH_REGISTERED = 13,
    MOVING_TO_UNSRESPAWN = 14,
    AT_COUNTER_FINAL = 15,
    WANDER_TO_IDLE = 16,
    EATING_FOOD = 17,
    FINISHED_EATING = 18
}

//Players and NPCs, to set the NPC to wander or other states
public enum NpcState
{
    IDLE = 0,
    WALKING_TO_TABLE = 1,
    AT_TABLE = 2,
    WALKING_TO_COUNTER = 3,
    AT_COUNTER = 4,
    AT_COUNTER_FINAL = 5,
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
    ATTENDED = 17,
    EATING_FOOD = 18
}

// List of Menus
public enum MenuTab
{
    STORE_ITEMS = 1,
    UPGRADE = 2,
    STORAGE_TAB = 3,
    SETTINGS_TAB = 4,
    //TUTORIAL_TAB = 5
}

//Menu Types
public enum MenuType
{
    TAB_MENU,
    DIALOG
}

// Object deifnition in in MenuObjectList.cs
// it should preserve the number order since it is used on the BACKEND database
// If adding more than the reserved number update the methods on the GameObjectList, IsCounter, IsTable, IsSnack..
public enum StoreItemType
{
    UPGRADE_ITEM = 0,
    TABLE_SINGLE_1 = 1,
    TABLE_SINGLE_2 = 2,
    TABLE_SINGLE_3 = 3,
    TABLE_SINGLE_4 = 4,
    TABLE_SINGLE_5 = 5,
    TABLE_SINGLE_6 = 6,
    TABLE_SINGLE_7 = 7,
    TABLE_SINGLE_8 = 8,
    TABLE_SINGLE_9 = 9,
    TABLE_SINGLE_10 = 10,
    COUNTER_1 = 11,
    COUNTER_2 = 12,
    COUNTER_3 = 13,
    COUNTER_4 = 14,
    COUNTER_5 = 15,
    COUNTER_6 = 16,
    COUNTER_7 = 17,
    COUNTER_8 = 18,
    COUNTER_9 = 19,
    COUNTER_10 = 20,
    SNACK_MACHINE_1 = 21,
    SNACK_MACHINE_2 = 22,
    SNACK_MACHINE_3 = 23,
    SNACK_MACHINE_4 = 24,
    SNACK_MACHINE_5 = 25,
    SNACK_MACHINE_6 = 26,
    SNACK_MACHINE_7 = 27,
    SNACK_MACHINE_8 = 28,
    SNACK_MACHINE_9 = 29,
    SNACK_MACHINE_10 = 30,
    KITCHEN_1 = 31,
    KITCHEN_2 = 32,
    KITCHEN_3 = 33,
    KITCHEN_4 = 34,
    KITCHEN_5 = 35,
    KITCHEN_6 = 36,
    KITCHEN_7 = 37,
    KITCHEN_8 = 38,
    KITCHEN_9 = 39,
    KITCHEN_10 = 40,
    DECORATION_1 = 41,
    DECORATION_2 = 42,
    DECORATION_3 = 43,
    DECORATION_4 = 44,
    DECORATION_5 = 45,
    DECORATION_6 = 46,
    DECORATION_7 = 47,
    DECORATION_8 = 48,
    DECORATION_9 = 49,
    DECORATION_10 = 50,
    OTHER_1 = 51,
    OTHER_2 = 52,
    OTHER_3 = 53,
    OTHER_4 = 54,
    OTHER_5 = 55,
    OTHER_6 = 56,
    OTHER_7 = 57,
    OTHER_8 = 58,
    OTHER_9 = 59,
    OTHER_10 = 60,
    UNDEFINED = 999
}

// Research items, intended to be 0 indexed, same as DB
public enum UpgradeType
{
    GRID_SIZE = 0,
    NUMBER_WAITERS = 1,
    WAITER_SPEED = 2,
    NUMBER_CLIENTS = 3,
    CLIENT_SPEED = 4,
    ORDER_COST_PERCENTAGE = 5,
    UPGRADE_AUTO_LOAD = 6,
    UPGRADE_LOAD_SPEED = 7
}

// Items given the type of store item
public enum ItemType
{
    ORANGE_JUICE = 1,
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

public enum NPCAnimatorState
{
    Walking = 0,
    Idle = 1,
    IdleTry = 2,
    WaitingAtTable = 3,
    WalkingToTable = 4,
    EatingAtTable = 5
}

public enum PlayerStats
{
    MONEY = 1,
    MONEY_SPENT = 2,
    TIME_PLAYED = 3,
    CLIENTS_ATTENDED = 4,
    ITEMS_BOUGHT = 5
}