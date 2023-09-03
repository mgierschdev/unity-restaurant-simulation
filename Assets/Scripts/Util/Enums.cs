//Item types
// to reference the type of object, each with different properties

namespace Util
{
    public enum ObjectType
    {
        Obstacle = 1,
        Client = 2,
        Player = 3,
        Employee = 4,
        NpcCounter = 5,
        Floor = 6,
        NpcSingleTable = 7,
        StoreItem = 8,
        UpgradeItem = 9,
        Coin = 10,
        Undefined = 999
    }

// it should preserve the number order since it is used on the backend database
// Front = Facing Unity camera
// Back = Opposite side from the camera
    public enum ObjectRotation
    {
        Back = 1,
        BackInverted = 2,
        Front = 3,
        FrontInverted = 4,
        Undefined = 999
    }

// To reference from fileNames to object names
    public enum TileType
    {
        SpamPoint = 1,
        WalkablePath = 2,
        Floor3 = 3,
        BusFloor = 4,
        FloorObstacle = 5,
        FloorMediumHorizontalObstacle = 6,
        FloorMediumVerticalObstacle = 7,
        FloorShortHorizontalObstacle = 8,
        FloorShortVerticalObstacle = 9,
        IsometricGridTile = 10,
        IsometricSingleSquareObject = 11,
        IsometricFourSquareObject = 12,
        Wall = 13,
        FloorEdit = 14,
        Undefined = 999
    }

//Players and NPCs move directions
    public enum MoveDirection
    {
        Idle = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        UpLeft = 5,
        Upright = 6,
        DownLeft = 7,
        Downright = 8
    }

// NPC transition attributes
    public enum NpcStateTransitions
    {
        TableAvailable = 0,
        TableMoved = 1,
        WalkToUnRespawn = 2,
        WaitingAtTable = 3,
        CounterAvailable = 4,
        OrderServed = 5,
        AtCounter = 6,
        AtTable = 7,
        CounterMoved = 8,
        Wander = 9,
        RegisteringCash = 10,
        Attended = 11,
        BeingAttended = 12,
        CashRegistered = 13,
        MovingToUnsRespawn = 14,
        AtCounterFinal = 15,
        WanderToIdle = 16,
        EatingFood = 17,
        FinishedEating = 18
    }

//Players and NPCs, to set the NPC to wander or other states
    public enum NpcState
    {
        Idle = 0,
        WalkingToTable = 1,
        AtTable = 2,
        WalkingToCounter = 3,
        AtCounter = 4,
        AtCounterFinal = 5,
        TakingOrder = 6,
        WaitingToBeAttended = 7,
        WalkingUnRespawn = 8,
        WalkingToCounterAfterOrder = 9,
        RegisteringCash = 10,
        WaitingToBeAttendedAnimation = 11,
        Wander = 12,
        WaitingForEnergyBarTakingOrder = 13,
        WaitingForEnergyBarRegisteringCash = 14,
        Walking = 15,
        BeingAttended = 16,
        Attended = 17,
        EatingFood = 18
    }

// List of Menus
    public enum MenuTab
    {
        StoreItems = 1,
        Upgrade = 2,
        StorageTab = 3,
        SettingsTab = 4,
        TablesTab = 5,
        SnacksTab = 6
        //TUTORIAL_TAB = 5
    }

//Menu Types
    public enum MenuType
    {
        TabMenu,
        Dialog
    }

// Object deifnition in in MenuObjectList.cs
// it should preserve the number order since it is used on the BACKEND database
// If adding more than the reserved number update the methods on the GameObjectList, IsCounter, IsTable, IsSnack..
    public enum StoreItemType
    {
        UpgradeItem = 0,
        TableSingle1 = 1,
        TableSingle2 = 2,
        TableSingle3 = 3,
        TableSingle4 = 4,
        TableSingle5 = 5,
        TableSingle6 = 6,
        TableSingle7 = 7,
        TableSingle8 = 8,
        TableSingle9 = 9,
        TableSingle10 = 10,
        Counter1 = 11,
        Counter2 = 12,
        Counter3 = 13,
        Counter4 = 14,
        Counter5 = 15,
        Counter6 = 16,
        Counter7 = 17,
        Counter8 = 18,
        Counter9 = 19,
        Counter10 = 20,
        SnackMachine1 = 21,
        SnackMachine2 = 22,
        SnackMachine3 = 23,
        SnackMachine4 = 24,
        SnackMachine5 = 25,
        SnackMachine6 = 26,
        SnackMachine7 = 27,
        SnackMachine8 = 28,
        SnackMachine9 = 29,
        SnackMachine10 = 30,
        Kitchen1 = 31,
        Kitchen2 = 32,
        Kitchen3 = 33,
        Kitchen4 = 34,
        Kitchen5 = 35,
        Kitchen6 = 36,
        Kitchen7 = 37,
        Kitchen8 = 38,
        Kitchen9 = 39,
        Kitchen10 = 40,
        Decoration1 = 41,
        Decoration2 = 42,
        Decoration3 = 43,
        Decoration4 = 44,
        Decoration5 = 45,
        Decoration6 = 46,
        Decoration7 = 47,
        Decoration8 = 48,
        Decoration9 = 49,
        Decoration10 = 50,
        Other1 = 51,
        Other2 = 52,
        Other3 = 53,
        Other4 = 54,
        Other5 = 55,
        Other6 = 56,
        Other7 = 57,
        Other8 = 58,
        Other9 = 59,
        Other10 = 60,
        Undefined = 999
    }

// Research items, intended to be 0 indexed, same as DB
    public enum UpgradeType
    {
        GridSize = 0,
        NumberWaiters = 1,
        WaiterSpeed = 2,
        NumberClients = 3,
        ClientSpeed = 4,
        OrderCostPercentage = 5,
        UpgradeAutoLoad = 6,
        UpgradeLoadSpeed = 7
    }

// Items given the type of store item
    public enum ItemType
    {
        OrangeJuice = 1,
        Undefined = 999
    }

// Auth source
// it should preserve the number order since it is used on the Firestore database
    public enum AuthSource
    {
        GooglePlay = 1,
        Anonymous = 2,
        Undefined = 999
    }

    public enum CharacterSide
    {
        Left = 0,
        Right = 1
    }

    public enum CellValue
    {
        NpcPosition = -2,
        ActionPoint = -1,
        Empty = 0,
        Busy = 1,
        Visited = 2 //For DFS operations,
    }

    public enum NpcAnimatorState
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
        Money = 1,
        MoneySpent = 2,
        TimePlayed = 3,
        ClientsAttended = 4,
        ItemsBought = 5
    }
}