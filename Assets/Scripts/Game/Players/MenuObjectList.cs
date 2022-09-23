using System.Collections.Generic;
public class MenuObjectList
{
    public List<StoreGameObject> Tables;
    public Dictionary<string, StoreGameObject> StoreItemDictionary; //Object Sprite Library Identifier / StoreObject
    public Dictionary<StoreItemType, StoreGameObject> StoreItemTypeDic; //Object Sprite Library Identifier / StoreObject

    public MenuObjectList()
    {
        SetAllItems();
    }
    public void SetAllItems()
    {
        Tables = new List<StoreGameObject>();
        StoreItemDictionary = new Dictionary<string, StoreGameObject>();
        StoreItemTypeDic = new Dictionary<StoreItemType, StoreGameObject>();

        Tables.Add(new StoreGameObject("Wooden table", "SingleTable-1", ObjectType.NPC_SINGLE_TABLE, StoreItemType.WOONDEN_TABLE_SINGLE, 20));
        StoreItemDictionary.Add(Tables[0].Identifier, Tables[0]);
        StoreItemTypeDic.Add(Tables[0].StoreItemType, Tables[0]);

        Tables.Add(new StoreGameObject("Wooden squared table", "SingleTable-2", ObjectType.NPC_SINGLE_TABLE, StoreItemType.SQUARED_WOONDEN_TABLE_SINGLE, 40));
        StoreItemDictionary.Add(Tables[1].Identifier, Tables[1]);
        StoreItemTypeDic.Add(Tables[1].StoreItemType, Tables[1]);

        Tables.Add(new StoreGameObject("Wooden table", "SingleTable-3", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_3, 50));
        StoreItemDictionary.Add(Tables[2].Identifier, Tables[2]);
        StoreItemTypeDic.Add(Tables[2].StoreItemType, Tables[2]);

        Tables.Add(new StoreGameObject("Dark wood table", "SingleTable-4", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_4, 60));
        StoreItemDictionary.Add(Tables[3].Identifier, Tables[3]);
        StoreItemTypeDic.Add(Tables[3].StoreItemType, Tables[3]);

        Tables.Add(new StoreGameObject("Wooden table", "SingleTable-5", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_5, 70));
        StoreItemDictionary.Add(Tables[4].Identifier, Tables[4]);
        StoreItemTypeDic.Add(Tables[4].StoreItemType, Tables[4]);

        Tables.Add(new StoreGameObject("Dark wood table", "SingleTable-6", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_6, 80));
        StoreItemDictionary.Add(Tables[5].Identifier, Tables[5]);
        StoreItemTypeDic.Add(Tables[5].StoreItemType, Tables[5]);

        Tables.Add(new StoreGameObject("Wooden table", "SingleTable-7", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_7, 90));
        StoreItemDictionary.Add(Tables[6].Identifier, Tables[6]);
        StoreItemTypeDic.Add(Tables[6].StoreItemType, Tables[6]);

        Tables.Add(new StoreGameObject("Dark wood table", "SingleTable-8", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_8, 100));
        StoreItemDictionary.Add(Tables[7].Identifier, Tables[7]);
        StoreItemTypeDic.Add(Tables[7].StoreItemType, Tables[7]);

        Tables.Add(new StoreGameObject("Iron table", "SingleTable-9", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_9, 200));
        StoreItemDictionary.Add(Tables[8].Identifier, Tables[8]);
        StoreItemTypeDic.Add(Tables[8].StoreItemType, Tables[8]);

        Tables.Add(new StoreGameObject("Iron table", "SingleTable-10", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_10, 200));
        StoreItemDictionary.Add(Tables[9].Identifier, Tables[9]);
        StoreItemTypeDic.Add(Tables[9].StoreItemType, Tables[9]);

        Tables.Add(new StoreGameObject("Counter", "Counter-1", ObjectType.NPC_COUNTER, StoreItemType.COUNTER, 999999));
        StoreItemDictionary.Add(Tables[10].Identifier, Tables[10]);
        StoreItemTypeDic.Add(Tables[10].StoreItemType, Tables[10]);

        Tables.Add(new StoreGameObject("UNDEFINED", "UNDEFINED", ObjectType.UNDEFINED, StoreItemType.UNDEFINED, 999999));
        StoreItemDictionary.Add(Tables[11].Identifier, Tables[11]);
        StoreItemTypeDic.Add(Tables[11].StoreItemType, Tables[11]);
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

    public StoreGameObject GetStoreObject(StoreItemType table)
    {
        if (!StoreItemTypeDic.ContainsKey(table))
        {
            GameLog.LogWarning("Storeitem with id " + table + " does not exist");
            return null;
        }
        return StoreItemTypeDic[table];
    }
}