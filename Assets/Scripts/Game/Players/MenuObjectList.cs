using System.Collections.Generic;

public static class MenuObjectList
{
    public static List<StoreGameObject> ActionPointItems;
    public static List<StoreGameObject> BaseStoreItemList;
    public static List<StoreGameObject> AllStoreItems;
    public static List<StoreGameObject> UpgradeItems;
    public static List<StoreGameObject> SettingsItems;
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
        BaseStoreItemList = new List<StoreGameObject>();
        UpgradeItems = new List<StoreGameObject>();
        SettingsItems = new List<StoreGameObject>();


        StoreItemDictionary = new Dictionary<string, StoreGameObject>();
        StoreItemTypeDic = new Dictionary<StoreItemType, StoreGameObject>();

        AllStoreItems = new List<StoreGameObject>{
            new StoreGameObject("Wooden table", "SingleTable-1", ObjectType.NPC_SINGLE_TABLE, StoreItemType.WOODEN_TABLE_SINGLE, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 20, true),
            new StoreGameObject("Wooden squared table", "SingleTable-2", ObjectType.NPC_SINGLE_TABLE, StoreItemType.SQUARED_WOODEN_TABLE_SINGLE,  Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 40, true),
            // new StoreGameObject("Wooden table", "SingleTable-3", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_3, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 50, true),
            // new StoreGameObject("Dark wood table", "SingleTable-4", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_4, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 60, true),
            // new StoreGameObject("Wooden table", "SingleTable-5", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_5, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 70, true),
            // new StoreGameObject("Dark wood table", "SingleTable-6", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_6, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 80, true),
            // new StoreGameObject("Wooden table", "SingleTable-7", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_7, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 90, true),
            // new StoreGameObject("Dark wood table", "SingleTable-8", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_8, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 100, true),
            // new StoreGameObject("Iron table", "SingleTable-9", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_9, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 200, true),
            // new StoreGameObject("Iron table", "SingleTable-10", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_10, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 200, true),
            new StoreGameObject("Counter", "Counter-1", ObjectType.NPC_COUNTER, StoreItemType.COUNTER, Settings.SpriteLibCategoryStoreObjects, Settings.PrefabCounter, 50, true),

            new StoreGameObject("Wooden container", "Store-1", ObjectType.STORE_ITEM, StoreItemType.LEMONADE_STORE_ITEM,  Settings.SpriteLibCategoryStoreItems, Settings.PrefabBaseStoreItem, 40, false),
            new StoreGameObject("Wooden container 2", "Store-2", ObjectType.STORE_ITEM, StoreItemType.SODA_STORE_ITEM, Settings.SpriteLibCategoryStoreItems, Settings.PrefabBaseStoreItem, 50, false),
            new StoreGameObject("Wooden container 3", "Store-3", ObjectType.STORE_ITEM, StoreItemType.LEMONADE_STORE_ITEM_TEST_3, Settings.SpriteLibCategoryStoreItems, Settings.PrefabBaseStoreItem, 60, false),
            new StoreGameObject("Wooden container 4", "Store-4", ObjectType.STORE_ITEM, StoreItemType.LEMONADE_STORE_ITEM_TEST_4, Settings.SpriteLibCategoryStoreItems, Settings.PrefabBaseStoreItem, 70, false),
            new StoreGameObject("Wooden container 5", "Store-5", ObjectType.STORE_ITEM, StoreItemType.LEMONADE_STORE_ITEM_TEST_5,  Settings.SpriteLibCategoryStoreItems, Settings.PrefabBaseStoreItem, 70, false),

            new StoreGameObject("GRID SIZE", "Upgrade-1", ObjectType.UPGRADE_ITEM, StoreItemType.GRID_SIZE, Settings.SpriteLibCategoryUpgradeItems, "", 1, false),
            new StoreGameObject("CLIENT_MAX_WAITING_TIME", "Upgrade-2", ObjectType.UPGRADE_ITEM, StoreItemType.CLIENT_MAX_WAITING_TIME, Settings.SpriteLibCategoryUpgradeItems, "", 2, false),
            new StoreGameObject("CLIENT_SPEED", "Upgrade-3", ObjectType.UPGRADE_ITEM, StoreItemType.CLIENT_SPEED, Settings.SpriteLibCategoryUpgradeItems, "", 3, false),
            new StoreGameObject("NUMBER_CLIENTS", "Upgrade-4", ObjectType.UPGRADE_ITEM, StoreItemType.NUMBER_CLIENTS, Settings.SpriteLibCategoryUpgradeItems, "", 4, false),
            new StoreGameObject("NUMBER_WAITERS", "Upgrade-5", ObjectType.UPGRADE_ITEM, StoreItemType.NUMBER_WAITERS, Settings.SpriteLibCategoryUpgradeItems, "", 5, false),
            new StoreGameObject("OFFLINE_MONEY_LIMIT", "Upgrade-6", ObjectType.UPGRADE_ITEM, StoreItemType.OFFLINE_MONEY_LIMIT, Settings.SpriteLibCategoryUpgradeItems, "", 6, false),
            new StoreGameObject("OFFLINE_MONEY_PERCENTAGE_INCREASE", "Upgrade-7", ObjectType.UPGRADE_ITEM, StoreItemType.OFFLINE_MONEY_PERCENTAGE_INCREASE, Settings.SpriteLibCategoryUpgradeItems, "", 7, false),
            new StoreGameObject("ORDER_COST", "Upgrade-8", ObjectType.UPGRADE_ITEM, StoreItemType.ORDER_COST, Settings.SpriteLibCategoryUpgradeItems, "", 8, false),
            new StoreGameObject("WAITER_SPEED", "Upgrade-9", ObjectType.UPGRADE_ITEM, StoreItemType.WAITER_SPEED, Settings.SpriteLibCategoryUpgradeItems, "", 9, false),

            new StoreGameObject("UNDEFINED", "UNDEFINED", ObjectType.UNDEFINED, StoreItemType.UNDEFINED, "UNDEFINED", "UNDEFINED", 999, false)
        };

        foreach (StoreGameObject storeItem in AllStoreItems)
        {
            StoreItemDictionary.Add(storeItem.Identifier, storeItem);
            StoreItemTypeDic.Add(storeItem.StoreItemType, storeItem);

            if (storeItem.HasActionPoint)
            {
                ActionPointItems.Add(storeItem);
            }

            if (storeItem.Type == ObjectType.STORE_ITEM)
            {
                BaseStoreItemList.Add(storeItem);
            }

            if (storeItem.Type == ObjectType.UPGRADE_ITEM)
            {
                UpgradeItems.Add(storeItem);
            }
        }

        UpgradeItems.Sort();
        ActionPointItems.Sort();
        BaseStoreItemList.Sort();
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

    public static ItemType GetItemGivenStoreItem(StoreItemType type)
    {
        switch (type)
        {
            case StoreItemType.LEMONADE_STORE_ITEM: return ItemType.LEMONADE;
        }
        return ItemType.UNDEFINED;
    }

    public static string GetItemSprite(ItemType type)
    {
        switch (type)
        {
            case ItemType.LEMONADE: return "Store-1";
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
            case StoreItemType.LEMONADE_STORE_ITEM: return Settings.PrefabBaseStoreItem;
        }
        return "";
    }

    public static List<StoreGameObject> GetItemList(MenuTab tab)
    {
        switch (tab)
        {
            case MenuTab.TABLES_TAB: return ActionPointItems;
            case MenuTab.STORE_ITEMS: return BaseStoreItemList;
            case MenuTab.UPGRADE: return UpgradeItems;
            case MenuTab.SETTINGS_TAB: return SettingsItems;
        }

        return new List<StoreGameObject>();
    }

    public static string GetButtonLabel(MenuTab tab)
    {
        switch (tab)
        {
            case MenuTab.TABLES_TAB: return "Tables";
            case MenuTab.STORE_ITEMS: return "Store";
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