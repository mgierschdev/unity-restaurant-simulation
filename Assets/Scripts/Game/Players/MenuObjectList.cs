using System.Collections.Generic;
using Game.Players.Model;
using Util;

namespace Game.Players
{
    public static class MenuObjectList
    {
        public static List<StoreGameObject> CounterItems;
        public static List<StoreGameObject> ContainerItems;
        public static List<StoreGameObject> InGameStoreItems;
        public static List<StoreGameObject> EmployeeItems;
        public static List<StoreGameObject> Storage;

        private static List<StoreGameObject> _baseDispenserList;
        private static List<StoreGameObject> _allStoreItems;
        private static List<StoreGameObject> _settingsItems;

        private static List<StoreGameObject> _actionPointItems;

        //Object Sprite Library Identifier / StoreObject
        private static Dictionary<string, StoreGameObject> _storeItemDictionary;

        //Object Sprite Library Identifier / StoreObject
        private static Dictionary<StoreItemType, StoreGameObject> _storeItemTypeDic;

        public static void Init()
        {
            SetAllItems();
        }

        public static void SetAllItems()
        {
            _actionPointItems = new List<StoreGameObject>();
            CounterItems = new List<StoreGameObject>();
            _baseDispenserList = new List<StoreGameObject>();
            ContainerItems = new List<StoreGameObject>();
            InGameStoreItems = new List<StoreGameObject>();
            _settingsItems = new List<StoreGameObject>();
            EmployeeItems = new List<StoreGameObject>();
            Storage = new List<StoreGameObject>();

            _storeItemDictionary = new Dictionary<string, StoreGameObject>();
            _storeItemTypeDic = new Dictionary<StoreItemType, StoreGameObject>();

            _allStoreItems = new List<StoreGameObject>
            {
                new StoreGameObject("Wooden table", "SingleTable-1", ObjectType.NpcSingleTable,
                    StoreItemType.TableSingle1, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 20,
                    true),
                new StoreGameObject("Wooden squared table", "SingleTable-2", ObjectType.NpcSingleTable,
                    StoreItemType.TableSingle2, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 40,
                    true),
                // new StoreGameObject("Wooden table", "SingleTable-3", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_3, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 50, true),
                // new StoreGameObject("Dark wood table", "SingleTable-4", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_4, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 60, true),
                // new StoreGameObject("Wooden table", "SingleTable-5", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_5, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 70, true),
                // new StoreGameObject("Dark wood table", "SingleTable-6", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_6, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 80, true),
                // new StoreGameObject("Wooden table", "SingleTable-7", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_7, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 90, true),
                // new StoreGameObject("Dark wood table", "SingleTable-8", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_8, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 100, true),
                // new StoreGameObject("Iron table", "SingleTable-9", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_9, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 200, true),
                // new StoreGameObject("Iron table", "SingleTable-10", ObjectType.NPC_SINGLE_TABLE, StoreItemType.TABLE_SINGLE_10, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 200, true),
                new StoreGameObject("Counter", "Counter-1", ObjectType.NpcCounter, StoreItemType.Counter1,
                    Settings.SpriteLibCategoryStoreObjects, Settings.PrefabCounter, 50, true),

                new StoreGameObject("Wooden container", "Dispenser-1", ObjectType.StoreItem,
                    StoreItemType.SnackMachine1, Settings.SpriteLibCategoryStoreItems, Settings.PrefabSnackMachine,
                    40, false),

                new StoreGameObject("UNDEFINED", "UNDEFINED", ObjectType.Undefined, StoreItemType.Undefined,
                    "UNDEFINED", "UNDEFINED", 999, false)
            };

            foreach (StoreGameObject storeItem in _allStoreItems)
            {
                _storeItemDictionary.Add(storeItem.Identifier, storeItem);
                _storeItemTypeDic.Add(storeItem.StoreItemType, storeItem);

                if (storeItem.HasActionPoint)
                {
                    _actionPointItems.Add(storeItem);
                }

                if (storeItem.Type == ObjectType.StoreItem)
                {
                    _baseDispenserList.Add(storeItem);
                }
            }

            _actionPointItems.Sort();
            _baseDispenserList.Sort();
        }

        // The id = StoreGameObject.Identifier
        public static StoreGameObject GetStoreObject(string id)
        {
            if (!_storeItemDictionary.ContainsKey(id))
            {
                GameLog.LogWarning("Storeitem with id " + id + " does not exist");
                return null;
            }

            return _storeItemDictionary[id];
        }

        public static StoreGameObject GetStoreObject(StoreItemType storeItem)
        {
            if (!_storeItemTypeDic.ContainsKey(storeItem))
            {
                GameLog.LogWarning("Storeitem with id " + storeItem + " does not exist");
                return null;
            }

            return _storeItemTypeDic[storeItem];
        }

        public static ItemType GetItemGivenDispenser(StoreItemType type)
        {
            switch (type)
            {
                case StoreItemType.SnackMachine1: return ItemType.OrangeJuice;
            }

            return ItemType.Undefined;
        }

        public static string GetItemSprite(ItemType type)
        {
            switch (type)
            {
                case ItemType.OrangeJuice: return "Dispenser-1";
            }

            return "";
        }

        public static string GetPrefab(StoreItemType type)
        {
            StoreGameObject obj = GetStoreObject(type);
            switch (obj.StoreItemType)
            {
                case StoreItemType.TableSingle1: return Settings.PrefabSingleTable;
                case StoreItemType.TableSingle2: return Settings.PrefabSingleTable;
                case StoreItemType.TableSingle3: return Settings.PrefabSingleTable;
                case StoreItemType.TableSingle4: return Settings.PrefabSingleTable;
                case StoreItemType.TableSingle5: return Settings.PrefabSingleTable;
                case StoreItemType.TableSingle6: return Settings.PrefabSingleTable;
                case StoreItemType.TableSingle7: return Settings.PrefabSingleTable;
                case StoreItemType.TableSingle8: return Settings.PrefabSingleTable;
                case StoreItemType.TableSingle9: return Settings.PrefabSingleTable;
                case StoreItemType.Counter1: return Settings.PrefabCounter;
                case StoreItemType.SnackMachine1: return Settings.PrefabSnackMachine;
            }

            return "";
        }

        public static List<StoreGameObject> GetItemList(MenuTab tab)
        {
            switch (tab)
            {
                case MenuTab.StoreItems: return _actionPointItems;
                case MenuTab.StorageTab: return _baseDispenserList;
                // case MenuTab.IN_GAME_STORE_TAB: return InGameStoreItems;
                case MenuTab.SettingsTab: return _settingsItems;
            }

            return new List<StoreGameObject>();
        }

        public static string GetButtonLabel(MenuTab tab)
        {
            switch (tab)
            {
                case MenuTab.TablesTab: return "Tables";
                case MenuTab.SnacksTab: return "Dispensers";
                case MenuTab.Upgrade: return "Research";
                case MenuTab.StorageTab: return "Storage";
                case MenuTab.SettingsTab: return "Settings";
            }

            return "";
        }

        public static List<DataGameObject> LoadCurrentUserStorage()
        {
            List<DataGameObject> storage = new List<DataGameObject>();

            foreach (DataGameObject obj in PlayerData.GerUserObjects())
            {
                if (obj.isStored)
                {
                    storage.Add(obj);
                }
            }

            return storage;
        }
    }
}