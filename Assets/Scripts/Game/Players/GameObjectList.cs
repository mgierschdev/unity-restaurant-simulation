using System.Collections.Generic;
using UnityEngine;

public static class GameObjectList
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
            new StoreGameObject("Single table", "SingleTable-1", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_1, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 320, true),
            new StoreGameObject("Single Wood table", "SingleTable-2", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_2, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 320, true),
            new StoreGameObject("Single Dark Wood table", "SingleTable-3", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_3, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 320, true),

            new StoreGameObject("Counter", "Counter-1", ObjectType.NPC_COUNTER, StoreItemType.COUNTER_1, Settings.SpriteLibCategoryStoreObjects, Settings.PrefabCounter, 3000, true),
            new StoreGameObject("Wood Counter", "Counter-2", ObjectType.NPC_COUNTER, StoreItemType.COUNTER_2, Settings.SpriteLibCategoryStoreObjects, Settings.PrefabCounter, 3000, true),
            new StoreGameObject("Dark Wood Counter", "Counter-3", ObjectType.NPC_COUNTER, StoreItemType.COUNTER_3, Settings.SpriteLibCategoryStoreObjects, Settings.PrefabCounter, 3000, true),


            new StoreGameObject("Sodas", "Store-1", ObjectType.STORE_ITEM, StoreItemType.SNACK_MACHINE_1,  Settings.SpriteLibCategoryStoreItems, Settings.PrefabBaseStoreItem, 1000, true,
            new List<StoreGameObjectItem>{
                new StoreGameObjectItem("Store-1-Item-1","Store-1-Item-1", 10),
                new StoreGameObjectItem("Store-1-Item-2","Store-1-Item-2", 10),
                new StoreGameObjectItem("Store-1-Item-3","Store-1-Item-3", 10),
                new StoreGameObjectItem("Store-1-Item-4","Store-1-Item-4", 10),
            }),

            new StoreGameObject("Snacks", "Store-2", ObjectType.STORE_ITEM, StoreItemType.SNACK_MACHINE_2, Settings.SpriteLibCategoryStoreItems, Settings.PrefabBaseStoreItem, 2000, true,
            new List<StoreGameObjectItem>{
                new StoreGameObjectItem("Store-2-Item-1","Store-1-Item-1", 10),
                new StoreGameObjectItem("Store-2-Item-2","Store-1-Item-2", 10),
                new StoreGameObjectItem("Store-2-Item-3","Store-1-Item-3", 10),
            }),

            new StoreGameObject("Coffee", "Store-3", ObjectType.STORE_ITEM, StoreItemType.SNACK_MACHINE_3, Settings.SpriteLibCategoryStoreItems, Settings.PrefabBaseStoreItem, 6000, true,
             new List<StoreGameObjectItem>{
                new StoreGameObjectItem("Store-3-Item-1","Store-1-Item-1", 10),
                new StoreGameObjectItem("Store-3-Item-2","Store-1-Item-2", 10),
                new StoreGameObjectItem("Store-3-Item-3","Store-1-Item-3", 10),
                new StoreGameObjectItem("Store-3-Item-4","Store-1-Item-3", 10),
             }),

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
            case StoreItemType.SNACK_MACHINE_1: return ItemType.ORANGE_JUICE;
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

        if (IsTable(obj.StoreItemType))
        {
            return Settings.PrefabSingleTable;
        }
        else if (IsCounter(obj.StoreItemType))
        {
            return Settings.PrefabCounter;
        }
        else if (IsSnackMachine(obj.StoreItemType))
        {
            return Settings.PrefabBaseStoreItem;
        }
        else if (IsKitchen(obj.StoreItemType))
        {
            return "";//TODO
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

    public static bool IsCounter(StoreItemType type)
    {
        return type == StoreItemType.COUNTER_1 ||
                type == StoreItemType.COUNTER_2 ||
                type == StoreItemType.COUNTER_3 ||
                type == StoreItemType.COUNTER_4 ||
                type == StoreItemType.COUNTER_5 ||
                type == StoreItemType.COUNTER_6 ||
                type == StoreItemType.COUNTER_7 ||
                type == StoreItemType.COUNTER_8 ||
                type == StoreItemType.COUNTER_9 ||
                type == StoreItemType.COUNTER_10;
    }

    public static bool IsTable(StoreItemType type)
    {
        return type == StoreItemType.TABLE_SINGLE_1 ||
        type == StoreItemType.TABLE_SINGLE_2 ||
        type == StoreItemType.TABLE_SINGLE_3 ||
        type == StoreItemType.TABLE_SINGLE_4 ||
        type == StoreItemType.TABLE_SINGLE_5 ||
        type == StoreItemType.TABLE_SINGLE_6 ||
        type == StoreItemType.TABLE_SINGLE_7 ||
        type == StoreItemType.TABLE_SINGLE_8 ||
        type == StoreItemType.TABLE_SINGLE_9 ||
        type == StoreItemType.TABLE_SINGLE_10;
    }

    public static bool IsSnackMachine(StoreItemType type)
    {
        return type == StoreItemType.SNACK_MACHINE_1 ||
                type == StoreItemType.SNACK_MACHINE_2 ||
                type == StoreItemType.SNACK_MACHINE_3 ||
                type == StoreItemType.SNACK_MACHINE_4 ||
                type == StoreItemType.SNACK_MACHINE_5 ||
                type == StoreItemType.SNACK_MACHINE_6 ||
                type == StoreItemType.SNACK_MACHINE_7 ||
                type == StoreItemType.SNACK_MACHINE_8 ||
                type == StoreItemType.SNACK_MACHINE_9 ||
                type == StoreItemType.SNACK_MACHINE_10;
    }

    public static bool IsKitchen(StoreItemType type)
    {
        return type == StoreItemType.KITCHEN_1 ||
                type == StoreItemType.KITCHEN_2 ||
                type == StoreItemType.KITCHEN_3 ||
                type == StoreItemType.KITCHEN_4 ||
                type == StoreItemType.KITCHEN_5 ||
                type == StoreItemType.KITCHEN_6 ||
                type == StoreItemType.KITCHEN_7 ||
                type == StoreItemType.KITCHEN_8 ||
                type == StoreItemType.KITCHEN_9 ||
                type == StoreItemType.KITCHEN_10;
    }
}