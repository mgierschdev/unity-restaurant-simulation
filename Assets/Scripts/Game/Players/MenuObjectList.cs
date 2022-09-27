using System.Collections.Generic;
public class MenuObjectList
{
    public List<StoreGameObject> AllStoreItems;
    public Dictionary<string, StoreGameObject> StoreItemDictionary; //Object Sprite Library Identifier / StoreObject
    public Dictionary<StoreItemType, StoreGameObject> StoreItemTypeDic; //Object Sprite Library Identifier / StoreObject

    public MenuObjectList()
    {
        SetAllItems();
    }
    public void SetAllItems()
    {
        AllStoreItems = new List<StoreGameObject>();
        StoreItemDictionary = new Dictionary<string, StoreGameObject>();
        StoreItemTypeDic = new Dictionary<StoreItemType, StoreGameObject>();

        AllStoreItems.Add(new StoreGameObject("Wooden table", "SingleTable-1", ObjectType.NPC_SINGLE_TABLE, StoreItemType.WOODEN_TABLE_SINGLE, Settings.SpriteLibCategoryTables, 20));
        AllStoreItems.Add(new StoreGameObject("Wooden squared table", "SingleTable-2", ObjectType.NPC_SINGLE_TABLE, StoreItemType.SQUARED_WOODEN_TABLE_SINGLE, Settings.SpriteLibCategoryTables, 40));
        AllStoreItems.Add(new StoreGameObject("Wooden table", "SingleTable-3", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_3, Settings.SpriteLibCategoryTables, 50));
        AllStoreItems.Add(new StoreGameObject("Dark wood table", "SingleTable-4", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_4, Settings.SpriteLibCategoryTables, 60));
        AllStoreItems.Add(new StoreGameObject("Wooden table", "SingleTable-5", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_5, Settings.SpriteLibCategoryTables, 70));
        AllStoreItems.Add(new StoreGameObject("Dark wood table", "SingleTable-6", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_6, Settings.SpriteLibCategoryTables, 80));
        AllStoreItems.Add(new StoreGameObject("Wooden table", "SingleTable-7", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_7, Settings.SpriteLibCategoryTables, 90));
        AllStoreItems.Add(new StoreGameObject("Dark wood table", "SingleTable-8", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_8, Settings.SpriteLibCategoryTables, 100));
        AllStoreItems.Add(new StoreGameObject("Iron table", "SingleTable-9", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_9, Settings.SpriteLibCategoryTables, 200));
        AllStoreItems.Add(new StoreGameObject("Iron table", "SingleTable-10", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_10, Settings.SpriteLibCategoryTables, 200));

        AllStoreItems.Add(new StoreGameObject("Counter", "Counter-1", ObjectType.NPC_COUNTER, StoreItemType.COUNTER, Settings.SpriteLibCategoryStoreObjects, 999999));

        AllStoreItems.Add(new StoreGameObject("Wooden container", "BaseContainer-1", ObjectType.SINGLE_CONTAINER, StoreItemType.WOODEN_BASE_CONTAINER, Settings.SpriteLibCategoryContainers, 40));

        AllStoreItems.Add(new StoreGameObject("UNDEFINED", "UNDEFINED", ObjectType.UNDEFINED, StoreItemType.UNDEFINED, "UNDEFINED", 999999));

        foreach (StoreGameObject storeItem in AllStoreItems)
        {
            StoreItemDictionary.Add(storeItem.Identifier, storeItem);
            StoreItemTypeDic.Add(storeItem.StoreItemType, storeItem);
        }
    }

    public StoreGameObject GetStoreObject(string id)
    {
        if (!StoreItemDictionary.ContainsKey(id))
        {
            GameLog.LogWarning("Storeitem with id " + id + " does not exist");
            return null;
        }
        return StoreItemDictionary[id];
    }

    public StoreGameObject GetStoreObject(StoreItemType storeItem)
    {
        if (!StoreItemTypeDic.ContainsKey(storeItem))
        {
            GameLog.LogWarning("Storeitem with id " + storeItem + " does not exist");
            return null;
        }
        return StoreItemTypeDic[storeItem];
    }
}