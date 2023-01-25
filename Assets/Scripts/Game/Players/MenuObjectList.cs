using System.Collections.Generic;
using UnityEngine;

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
    public static Dictionary<string, Sprite> ObjectSprites;

    public static void Init()
    {
        SetAllItems();
        LoadItemsSprites();
    }
    public static void SetAllItems()
    {
        ActionPointItems = new List<StoreGameObject>();
        BaseStoreItemList = new List<StoreGameObject>();
        UpgradeItems = new List<StoreGameObject>();
        SettingsItems = new List<StoreGameObject>();


        StoreItemDictionary = new Dictionary<string, StoreGameObject>();
        StoreItemTypeDic = new Dictionary<StoreItemType, StoreGameObject>();
        ObjectSprites = new Dictionary<string, Sprite>();

        AllStoreItems = new List<StoreGameObject>{
            new StoreGameObject("Single blue table", "SingleTable-1", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_1, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 320, true),
            //new StoreGameObject("Wooden squared table", "SingleTable-2", ObjectType.NPC_SINGLE_TABLE, StoreItemType.SQUARED_WOODEN_TABLE_SINGLE,  Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 40, true),
            // new StoreGameObject("Wooden table", "SingleTable-3", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_3, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 50, true),
            // new StoreGameObject("Dark wood table", "SingleTable-4", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_4, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 60, true),
            // new StoreGameObject("Wooden table", "SingleTable-5", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_5, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 70, true),
            // new StoreGameObject("Dark wood table", "SingleTable-6", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_6, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 80, true),
            // new StoreGameObject("Wooden table", "SingleTable-7", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_7, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 90, true),
            // new StoreGameObject("Dark wood table", "SingleTable-8", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_8, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 100, true),
            // new StoreGameObject("Iron table", "SingleTable-9", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_9, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 200, true),
            // new StoreGameObject("Iron table", "SingleTable-10", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_10, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 200, true),
            new StoreGameObject("Counter", "Counter-1", ObjectType.NPC_COUNTER, StoreItemType.COUNTER, Settings.SpriteLibCategoryStoreObjects, Settings.PrefabCounter, 3000, true),

            new StoreGameObject("Soda Dispenser", "Store-1", ObjectType.STORE_ITEM, StoreItemType.STORE_ITEM_ORANGE_JUICE,  Settings.SpriteLibCategoryStoreItems, Settings.PrefabBaseStoreItem, 1000, true,
            new List<StoreGameObjectItem>{
                new StoreGameObjectItem("Store-1-Item-1","Store-1-Item-1", 10),
                new StoreGameObjectItem("Store-1-Item-2","Store-1-Item-2", 10),
                new StoreGameObjectItem("Store-1-Item-3","Store-1-Item-3", 10),
                new StoreGameObjectItem("Store-1-Item-4","Store-1-Item-4", 10),
            }),
            
            new StoreGameObject("SODA_STORE_ITEM", "Store-2", ObjectType.STORE_ITEM, StoreItemType.STORE_ITEM_SODA, Settings.SpriteLibCategoryStoreItems, Settings.PrefabBaseStoreItem, 2000, true),
            new StoreGameObject("TODO_STORE_ITEM_TEST_3", "Store-3", ObjectType.STORE_ITEM, StoreItemType.LEMONADE_STORE_ITEM_TEST_3, Settings.SpriteLibCategoryStoreItems, Settings.PrefabBaseStoreItem, 6000, true),

            new StoreGameObject("Bigger Bussiness", "Upgrade-1", ObjectType.UPGRADE_ITEM, UpgradeType.GRID_SIZE, Settings.SpriteLibCategoryUpgradeItems, "", 1000, false, 10),
            new StoreGameObject("Faster Clients", "Upgrade-3", ObjectType.UPGRADE_ITEM, UpgradeType.CLIENT_SPEED, Settings.SpriteLibCategoryUpgradeItems, "", 3000, false, 3),
            new StoreGameObject("More Clients", "Upgrade-4", ObjectType.UPGRADE_ITEM, UpgradeType.NUMBER_CLIENTS, Settings.SpriteLibCategoryUpgradeItems, "", 4000, false, 10),
            new StoreGameObject("More Waiters", "Upgrade-5", ObjectType.UPGRADE_ITEM, UpgradeType.NUMBER_WAITERS, Settings.SpriteLibCategoryUpgradeItems, "", 5000, false, 3),
            new StoreGameObject("More profit (%)", "Upgrade-8", ObjectType.UPGRADE_ITEM, UpgradeType.ORDER_COST_PERCENTAGE, Settings.SpriteLibCategoryUpgradeItems, "", 8000, false, 5),
            new StoreGameObject("Waiter speed", "Upgrade-9", ObjectType.UPGRADE_ITEM, UpgradeType.WAITER_SPEED, Settings.SpriteLibCategoryUpgradeItems, "", 9000, false, 5),
            new StoreGameObject("Store Auto load", "Upgrade-10", ObjectType.UPGRADE_ITEM, UpgradeType.UPGRADE_AUTO_LOAD, Settings.SpriteLibCategoryUpgradeItems, "", 10000, false, 5),
            new StoreGameObject("Store Speed load", "Upgrade-11", ObjectType.UPGRADE_ITEM, UpgradeType.UPGRADE_LOAD_SPEED, Settings.SpriteLibCategoryUpgradeItems, "", 12000, false, 5),

            new StoreGameObject("UNDEFINED", "UNDEFINED", ObjectType.UNDEFINED, StoreItemType.UNDEFINED, "UNDEFINED", "UNDEFINED", 999, false)
        };

        //Max number of waiters 3, max number of clients  20, max numbers of machines/kitchens working 10
        foreach (StoreGameObject storeItem in AllStoreItems)
        {
            StoreItemDictionary.TryAdd(storeItem.Identifier, storeItem);
            StoreItemTypeDic.TryAdd(storeItem.StoreItemType, storeItem);

            if (storeItem.Type == ObjectType.STORE_ITEM || storeItem.HasActionPoint)
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
            case StoreItemType.STORE_ITEM_ORANGE_JUICE: return ItemType.ORANGE_JUICE;
        }
        return ItemType.UNDEFINED;
    }

    public static string GetItemSprite(ItemType type)
    {
        switch (type)
        {
            case ItemType.ORANGE_JUICE: return "Store-1";
        }
        return "";
    }

    public static string GetPrefab(StoreItemType type)
    {
        StoreGameObject obj = GetStoreObject(type);
        switch (obj.StoreItemType)
        {
            case StoreItemType.TABLE_SINGLE_1: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_2: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_3: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_4: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_5: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_6: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_7: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_8: return Settings.PrefabSingleTable;
            case StoreItemType.TABLE_SINGLE_9: return Settings.PrefabSingleTable;
            case StoreItemType.COUNTER: return Settings.PrefabCounter;
            case StoreItemType.STORE_ITEM_ORANGE_JUICE: return Settings.PrefabBaseStoreItem;
            case StoreItemType.STORE_ITEM_SODA: return Settings.PrefabBaseStoreItem;
        }
        return "";
    }

    public static List<StoreGameObject> GetItemList(MenuTab tab)
    {
        switch (tab)
        {
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
            case MenuTab.STORE_ITEMS: return TextUI.Store;
            case MenuTab.UPGRADE: return TextUI.Upgrade;
            case MenuTab.STORAGE_TAB: return TextUI.Storage;
            case MenuTab.SETTINGS_TAB: return TextUI.Settings;
                //       case MenuTab.TUTORIAL_TAB: return TextUI.Tutorial;
        }
        return "";
    }

    public static List<DataGameObject> LoadCurrentUserStorage()
    {
        List<DataGameObject> storage = new List<DataGameObject>();

        foreach (DataGameObject obj in PlayerData.GerUserObjects())
        {
            if (obj.IS_STORED)
            {
                storage.Add(obj);
            }
        }
        return storage;
    }

    public static void LoadItemsSprites()
    {
        foreach (StoreGameObject storeGameObject in AllStoreItems)
        {
            if (!ObjectSprites.ContainsKey(storeGameObject.MenuItemSprite) && !storeGameObject.MenuItemSprite.Contains("UNDEFINED"))
            {
                ObjectSprites.Add(storeGameObject.MenuItemSprite, Util.LoadSpriteResource(storeGameObject.MenuItemSprite));
            }
        }
    }
}