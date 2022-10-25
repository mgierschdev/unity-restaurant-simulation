using System.Collections.Generic;

public static class MenuObjectList
{
    public static List<StoreGameObject> TableItems;
    public static List<StoreGameObject> CounterItems;
    public static List<StoreGameObject> BaseContainerItems;
    public static List<StoreGameObject> AllStoreItems;
    public static Dictionary<string, StoreGameObject> StoreItemDictionary; //Object Sprite Library Identifier / StoreObject
    public static Dictionary<StoreItemType, StoreGameObject> StoreItemTypeDic; //Object Sprite Library Identifier / StoreObject

    public static void Init()
    {
        SetAllItems();
    }
    public static void SetAllItems()
    {
        TableItems = new List<StoreGameObject>();
        CounterItems = new List<StoreGameObject>();
        BaseContainerItems = new List<StoreGameObject>();

        StoreItemDictionary = new Dictionary<string, StoreGameObject>();
        StoreItemTypeDic = new Dictionary<StoreItemType, StoreGameObject>();

        AllStoreItems = new List<StoreGameObject>{
            new StoreGameObject("Wooden table", "SingleTable-1", ObjectType.NPC_SINGLE_TABLE, StoreItemType.WOODEN_TABLE_SINGLE, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 20, true),
            new StoreGameObject("Wooden squared table", "SingleTable-2", ObjectType.NPC_SINGLE_TABLE, StoreItemType.SQUARED_WOODEN_TABLE_SINGLE, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 40, true),
            new StoreGameObject("Wooden table", "SingleTable-3", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_3, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 50, true),
            new StoreGameObject("Dark wood table", "SingleTable-4", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_4, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 60, true),
            new StoreGameObject("Wooden table", "SingleTable-5", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_5, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 70, true),
            new StoreGameObject("Dark wood table", "SingleTable-6", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_6, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 80, true),
            new StoreGameObject("Wooden table", "SingleTable-7", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_7, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 90, true),
            new StoreGameObject("Dark wood table", "SingleTable-8", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_8, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 100, true),
            new StoreGameObject("Iron table", "SingleTable-9", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_9, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 200, true),
            new StoreGameObject("Iron table", "SingleTable-10", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_10, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 200, true),
            new StoreGameObject("Counter", "Counter-1", ObjectType.NPC_COUNTER, StoreItemType.COUNTER, Settings.SpriteLibCategoryStoreObjects, Settings.PrefabCounter, 999, true),
            new StoreGameObject("Wooden container", "BaseContainer-1", ObjectType.BASE_CONTAINER, StoreItemType.WOODEN_BASE_CONTAINER, Settings.SpriteLibCategoryContainers, Settings.PrefabBaseContainer, 40, false),
            new StoreGameObject("UNDEFINED", "UNDEFINED", ObjectType.UNDEFINED, StoreItemType.UNDEFINED, "UNDEFINED", "UNDEFINED", 999, false)
        };

        foreach (StoreGameObject storeItem in AllStoreItems)
        {
            StoreItemDictionary.Add(storeItem.Identifier, storeItem);
            StoreItemTypeDic.Add(storeItem.StoreItemType, storeItem);

            if (storeItem.Type == ObjectType.NPC_SINGLE_TABLE)
            {
                TableItems.Add(storeItem);
            }

            if (storeItem.Type == ObjectType.NPC_COUNTER)
            {
                CounterItems.Add(storeItem);
            }

            if (storeItem.Type == ObjectType.BASE_CONTAINER)
            {
                CounterItems.Add(storeItem);
            }
        }
    }

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

    public static string GetPrefab(StoreItemType type)
    {
        StoreGameObject obj = MenuObjectList.GetStoreObject(type);
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
            case StoreItemType.WOODEN_BASE_CONTAINER: return Settings.PrefabBaseContainer;
        }
        return "";
    }
}