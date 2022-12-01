using System.Collections.Generic;

public static class MenuObjectList
{
    public static List<StoreGameObject> ActionPointItems;
    public static List<StoreGameObject> CounterItems;
    public static List<StoreGameObject> ContainerItems;
    public static List<StoreGameObject> BaseDispenserList;
    public static List<StoreGameObject> AllStoreItems;
    public static List<StoreGameObject> InGameStoreItems;
    public static List<StoreGameObject> EmployeeItems;
    public static List<StoreGameObject> SettingsItems;
    public static List<StoreGameObject> Storage;
    //Object Sprite Library Identifier / StoreObject
    public static Dictionary<string, StoreGameObject> StoreItemDictionary;
    //Object Sprite Library Identifier / StoreObject
    public static Dictionary<StoreItemType, StoreGameObject> StoreItemTypeDic;

    public static void Init()
    {
        SetAllItems();
    }
    public static void SetAllItems()
    {
        ActionPointItems = new List<StoreGameObject>();
        CounterItems = new List<StoreGameObject>();
        BaseDispenserList = new List<StoreGameObject>();
        ContainerItems = new List<StoreGameObject>();
        InGameStoreItems = new List<StoreGameObject>();
        SettingsItems = new List<StoreGameObject>();
        EmployeeItems = new List<StoreGameObject>();
        Storage = new List<StoreGameObject>();

        StoreItemDictionary = new Dictionary<string, StoreGameObject>();
        StoreItemTypeDic = new Dictionary<StoreItemType, StoreGameObject>();

        AllStoreItems = new List<StoreGameObject>{
            new StoreGameObject("Wooden table", "SingleTable-1", ObjectType.NPC_SINGLE_TABLE, StoreItemType.WOODEN_TABLE_SINGLE, UpgradeType.UNDEFINED, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 20, true),
            new StoreGameObject("Wooden squared table", "SingleTable-2", ObjectType.NPC_SINGLE_TABLE, StoreItemType.SQUARED_WOODEN_TABLE_SINGLE, UpgradeType.UNDEFINED,  Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 40, true),
            // new StoreGameObject("Wooden table", "SingleTable-3", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_3, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 50, true),
            // new StoreGameObject("Dark wood table", "SingleTable-4", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_4, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 60, true),
            // new StoreGameObject("Wooden table", "SingleTable-5", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_5, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 70, true),
            // new StoreGameObject("Dark wood table", "SingleTable-6", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_6, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 80, true),
            // new StoreGameObject("Wooden table", "SingleTable-7", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_7, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 90, true),
            // new StoreGameObject("Dark wood table", "SingleTable-8", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_8, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 100, true),
            // new StoreGameObject("Iron table", "SingleTable-9", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_9, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 200, true),
            // new StoreGameObject("Iron table", "SingleTable-10", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_10, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 200, true),
            new StoreGameObject("Counter", "Counter-1", ObjectType.NPC_COUNTER, StoreItemType.COUNTER, UpgradeType.UNDEFINED,  Settings.SpriteLibCategoryStoreObjects, Settings.PrefabCounter, 50, true),

            new StoreGameObject("Wooden container", "Dispenser-1", ObjectType.DISPENSER, StoreItemType.LEMONADE_DISPENSER, UpgradeType.UNDEFINED, Settings.SpriteLibCategoryDispensers, Settings.PrefabBaseDispenser, 40, false),
            new StoreGameObject("Wooden container 2", "Dispenser-2", ObjectType.DISPENSER, StoreItemType.LEMONADE_DISPENSER_TEST_2, UpgradeType.UNDEFINED, Settings.SpriteLibCategoryDispensers, Settings.PrefabBaseDispenser, 50, false),
            new StoreGameObject("Wooden container 3", "Dispenser-3", ObjectType.DISPENSER, StoreItemType.LEMONADE_DISPENSER_TEST_3, UpgradeType.UNDEFINED, Settings.SpriteLibCategoryDispensers, Settings.PrefabBaseDispenser, 60, false),
            new StoreGameObject("Wooden container 4", "Dispenser-4", ObjectType.DISPENSER, StoreItemType.LEMONADE_DISPENSER_TEST_4, UpgradeType.UNDEFINED, Settings.SpriteLibCategoryDispensers, Settings.PrefabBaseDispenser, 70, false),
            new StoreGameObject("Wooden container 5", "Dispenser-5", ObjectType.DISPENSER, StoreItemType.LEMONADE_DISPENSER_TEST_5, UpgradeType.UNDEFINED, Settings.SpriteLibCategoryDispensers, Settings.PrefabBaseDispenser, 70, false),

            new StoreGameObject("GRID SIZE", "Upgrade-1", ObjectType.UPGRADE_ITEM, StoreItemType.UNDEFINED, UpgradeType.BUSS_SIZE, Settings.SpriteLibCategoryUpgradeItems, "", 70, false),

            new StoreGameObject("UNDEFINED", "UNDEFINED", ObjectType.UNDEFINED, StoreItemType.UNDEFINED, UpgradeType.UNDEFINED, "UNDEFINED", "UNDEFINED", 999, false)
        };

        foreach (StoreGameObject storeItem in AllStoreItems)
        {
            StoreItemDictionary.Add(storeItem.Identifier, storeItem);
            StoreItemTypeDic.Add(storeItem.StoreItemType, storeItem);

            if (storeItem.HasActionPoint)
            {
                ActionPointItems.Add(storeItem);
            }

            if (storeItem.Type == ObjectType.DISPENSER)
            {
                BaseDispenserList.Add(storeItem);
            }
        }

        ActionPointItems.Sort();
        BaseDispenserList.Sort();
    }
    // The id = StoreGameObject.Identifier
    public static StoreGameObject GetStoreObject(string id)
    {
        if (!StoreItemDictionary.ContainsKey(id))
        {
            GameLog.LogWarning("Storeitem with id " + id + " does not exist");
            return null;
        }
        return StoreItemDictionary[id];
    }

    public static StoreGameObject GetStoreObject(StoreItemType storeItem)
    {
        if (!StoreItemTypeDic.ContainsKey(storeItem))
        {
            GameLog.LogWarning("Storeitem with id " + storeItem + " does not exist");
            return null;
        }
        return StoreItemTypeDic[storeItem];
    }

    public static ItemType GetItemGivenDispenser(StoreItemType type)
    {
        switch (type)
        {
            case StoreItemType.LEMONADE_DISPENSER: return ItemType.LEMONADE;
        }
        return ItemType.UNDEFINED;
    }

    public static string GetItemSprite(ItemType type)
    {
        switch (type)
        {
            case ItemType.LEMONADE: return "Dispenser-1";
        }
        return "";
    }

    public static string GetPrefab(StoreItemType type)
    {
        StoreGameObject obj = GetStoreObject(type);
        switch (obj.StoreItemType)
        {
            case StoreItemType.WOODEN_TABLE_SINGLE: return Settings.PrefabSingleTable;
            case StoreItemType.SQUARED_WOODEN_TABLE_SINGLE: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_3: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_4: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_5: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_6: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_7: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_8: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_9: return Settings.PrefabSingleTable;
            case StoreItemType.COUNTER: return Settings.PrefabCounter;
            case StoreItemType.LEMONADE_DISPENSER: return Settings.PrefabBaseDispenser;
        }
        return "";
    }

    public static List<StoreGameObject> GetItemList(MenuTab tab)
    {
        switch (tab)
        {
            case MenuTab.TABLES_TAB: return ActionPointItems;
            case MenuTab.DISPENSERS: return BaseDispenserList;
            // case MenuTab.IN_GAME_STORE_TAB: return InGameStoreItems;
            case MenuTab.SETTINGS_TAB: return SettingsItems;
        }

        return new List<StoreGameObject>();
    }

    public static string GetButtonLabel(MenuTab tab)
    {
        switch (tab)
        {
            case MenuTab.TABLES_TAB: return "Tables";
            case MenuTab.DISPENSERS: return "Dispensers";
            case MenuTab.UPGRADE: return "Upgrade";
            case MenuTab.STORAGE_TAB: return "Storage";
            case MenuTab.SETTINGS_TAB: return "Settings";
        }
        return "";
    }

    public static List<DataGameObject> LoadCurrentUserStorage()
    {
        List<DataGameObject> storage = new List<DataGameObject>();

        foreach (DataGameObject obj in PlayerData.GetDataGameUser().OBJECTS)
        {
            if (obj.IS_STORED)
            {
                storage.Add(obj);
            }
        }
        return storage;
    }
}