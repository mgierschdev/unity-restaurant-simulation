using System.Collections.Generic;
using Game.Players.Model;
using UnityEngine;
using Util;

namespace Game.Players
{
    /**
     * Problem: Provide definitions for store items and upgrades.
     * Goal: Build and expose lists/dictionaries of game objects for UI and logic.
     * Approach: Initialize static collections with StoreGameObject metadata.
     * Time: O(n) for initialization (n = items).
     * Space: O(n) for stored collections.
     */
    public static class GameObjectList
    {
        public static List<StoreGameObject> ActionPointItems;
        public static List<StoreGameObject> SnackMachineList;
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
            SnackMachineList = new List<StoreGameObject>();
            UpgradeItems = new List<StoreGameObject>();
            SettingsItems = new List<StoreGameObject>();


            StoreItemDictionary = new Dictionary<string, StoreGameObject>();
            StoreItemTypeDic = new Dictionary<StoreItemType, StoreGameObject>();
            ObjectSprites = new Dictionary<string, Sprite>();

            AllStoreItems = new List<StoreGameObject>
            {
                new StoreGameObject("Single table", "SingleTable-1", ObjectType.NpcSingleTable,
                    StoreItemType.TableSingle1, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 320,
                    true),
                new StoreGameObject("Single Wood table", "SingleTable-2", ObjectType.NpcSingleTable,
                    StoreItemType.TableSingle2, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 320,
                    true),
                new StoreGameObject("Single Dark Wood table", "SingleTable-3", ObjectType.NpcSingleTable,
                    StoreItemType.TableSingle3, Settings.SpriteLibCategoryTables, Settings.PrefabSingleTable, 320,
                    true),

                new StoreGameObject("Counter", "Counter-1", ObjectType.NpcCounter, StoreItemType.Counter1,
                    Settings.SpriteLibCategoryStoreObjects, Settings.PrefabCounter, 3000, true),
                new StoreGameObject("Wood Counter", "Counter-2", ObjectType.NpcCounter, StoreItemType.Counter2,
                    Settings.SpriteLibCategoryStoreObjects, Settings.PrefabCounter, 3000, true),
                new StoreGameObject("Dark Wood Counter", "Counter-3", ObjectType.NpcCounter, StoreItemType.Counter3,
                    Settings.SpriteLibCategoryStoreObjects, Settings.PrefabCounter, 3000, true),


                new StoreGameObject("Soda Machine", "Store-1", ObjectType.StoreItem, StoreItemType.SnackMachine1,
                    Settings.SpriteLibCategoryStoreItems, Settings.PrefabSnackMachine, 1000, true,
                    new List<StoreGameObjectItem>
                    {
                        new StoreGameObjectItem("Store-1-Item-1", "Store-1-Item-1", 10),
                        new StoreGameObjectItem("Store-1-Item-2", "Store-1-Item-2", 10),
                        new StoreGameObjectItem("Store-1-Item-3", "Store-1-Item-3", 10),
                        new StoreGameObjectItem("Store-1-Item-4", "Store-1-Item-4", 10),
                    }),

                new StoreGameObject("Snack Machine", "Store-2", ObjectType.StoreItem, StoreItemType.SnackMachine2,
                    Settings.SpriteLibCategoryStoreItems, Settings.PrefabSnackMachine, 2000, true,
                    new List<StoreGameObjectItem>
                    {
                        new StoreGameObjectItem("Store-2-Item-1", "Store-2-Item-1", 10),
                        new StoreGameObjectItem("Store-2-Item-2", "Store-2-Item-2", 10),
                        new StoreGameObjectItem("Store-2-Item-3", "Store-2-Item-3", 10),
                    }),

                new StoreGameObject("Coffee Machine", "Store-3", ObjectType.StoreItem, StoreItemType.SnackMachine3,
                    Settings.SpriteLibCategoryStoreItems, Settings.PrefabSnackMachine, 6000, true,
                    new List<StoreGameObjectItem>
                    {
                        new StoreGameObjectItem("Store-3-Item-1", "Store-3-Item-1", 10),
                        new StoreGameObjectItem("Store-3-Item-2", "Store-3-Item-2", 10),
                        new StoreGameObjectItem("Store-3-Item-3", "Store-3-Item-3", 10),
                        new StoreGameObjectItem("Store-3-Item-4", "Store-3-Item-3", 10),
                    }),

                new StoreGameObject("Bigger Bussiness", "Upgrade-1", ObjectType.UpgradeItem, UpgradeType.GridSize,
                    Settings.SpriteLibCategoryUpgradeItems, "", 1000, false, 10),
                new StoreGameObject("Faster Clients", "Upgrade-3", ObjectType.UpgradeItem, UpgradeType.ClientSpeed,
                    Settings.SpriteLibCategoryUpgradeItems, "", 3000, false, 3),
                new StoreGameObject("More Clients", "Upgrade-4", ObjectType.UpgradeItem, UpgradeType.NumberClients,
                    Settings.SpriteLibCategoryUpgradeItems, "", 4000, false, 10),
                new StoreGameObject("More Waiters", "Upgrade-5", ObjectType.UpgradeItem, UpgradeType.NumberWaiters,
                    Settings.SpriteLibCategoryUpgradeItems, "", 5000, false, 3),
                new StoreGameObject("More profit (%)", "Upgrade-8", ObjectType.UpgradeItem,
                    UpgradeType.OrderCostPercentage, Settings.SpriteLibCategoryUpgradeItems, "", 8000, false, 5),
                new StoreGameObject("Waiter speed", "Upgrade-9", ObjectType.UpgradeItem, UpgradeType.WaiterSpeed,
                    Settings.SpriteLibCategoryUpgradeItems, "", 9000, false, 5),
                new StoreGameObject("Store Auto load", "Upgrade-10", ObjectType.UpgradeItem,
                    UpgradeType.UpgradeAutoLoad,
                    Settings.SpriteLibCategoryUpgradeItems, "", 10000, false, 5),
                new StoreGameObject("Store Speed load", "Upgrade-11", ObjectType.UpgradeItem,
                    UpgradeType.UpgradeLoadSpeed, Settings.SpriteLibCategoryUpgradeItems, "", 12000, false, 5),

                new StoreGameObject("UNDEFINED", "UNDEFINED", ObjectType.Undefined, StoreItemType.Undefined,
                    "UNDEFINED",
                    "UNDEFINED", 999, false)
            };

            //Max number of waiters 3, max number of clients  20, max numbers of machines/kitchens working 10
            foreach (StoreGameObject storeItem in AllStoreItems)
            {
                StoreItemDictionary.TryAdd(storeItem.Identifier, storeItem);
                StoreItemTypeDic.TryAdd(storeItem.StoreItemType, storeItem);

                if (storeItem.Type == ObjectType.StoreItem || storeItem.HasActionPoint)
                {
                    SnackMachineList.Add(storeItem);
                }

                if (storeItem.Type == ObjectType.UpgradeItem)
                {
                    UpgradeItems.Add(storeItem);
                }
            }

            UpgradeItems.Sort();
            ActionPointItems.Sort();
            SnackMachineList.Sort();
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
                case StoreItemType.SnackMachine1: return ItemType.OrangeJuice;
            }

            return ItemType.Undefined;
        }

        public static string GetItemSprite(ItemType type)
        {
            switch (type)
            {
                case ItemType.OrangeJuice: return "Store-1";
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
                return Settings.PrefabSnackMachine;
            }
            else if (IsKitchen(obj.StoreItemType))
            {
                return "";
            }

            return "";
        }

        public static List<StoreGameObject> GetItemList(MenuTab tab)
        {
            switch (tab)
            {
                case MenuTab.StoreItems: return SnackMachineList;
                case MenuTab.Upgrade: return UpgradeItems;
                case MenuTab.SettingsTab: return SettingsItems;
            }

            return new List<StoreGameObject>();
        }

        public static string GetButtonLabel(MenuTab tab)
        {
            switch (tab)
            {
                case MenuTab.StoreItems: return TextUI.Store;
                case MenuTab.Upgrade: return TextUI.Upgrade;
                case MenuTab.StorageTab: return TextUI.Storage;
                case MenuTab.SettingsTab: return TextUI.Settings;
                //       case MenuTab.TUTORIAL_TAB: return TextUI.Tutorial;
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

        public static void LoadItemsSprites()
        {
            foreach (StoreGameObject storeGameObject in AllStoreItems)
            {
                if (!ObjectSprites.ContainsKey(storeGameObject.MenuItemSprite) &&
                    !storeGameObject.MenuItemSprite.Contains("UNDEFINED"))
                {
                    ObjectSprites.Add(storeGameObject.MenuItemSprite,
                        Util.Util.LoadSpriteResource(storeGameObject.MenuItemSprite));
                }
            }
        }

        public static bool IsCounter(StoreItemType type)
        {
            return type == StoreItemType.Counter1 ||
                   type == StoreItemType.Counter2 ||
                   type == StoreItemType.Counter3 ||
                   type == StoreItemType.Counter4 ||
                   type == StoreItemType.Counter5 ||
                   type == StoreItemType.Counter6 ||
                   type == StoreItemType.Counter7 ||
                   type == StoreItemType.Counter8 ||
                   type == StoreItemType.Counter9 ||
                   type == StoreItemType.Counter10;
        }

        public static bool IsTable(StoreItemType type)
        {
            return type == StoreItemType.TableSingle1 ||
                   type == StoreItemType.TableSingle2 ||
                   type == StoreItemType.TableSingle3 ||
                   type == StoreItemType.TableSingle4 ||
                   type == StoreItemType.TableSingle5 ||
                   type == StoreItemType.TableSingle6 ||
                   type == StoreItemType.TableSingle7 ||
                   type == StoreItemType.TableSingle8 ||
                   type == StoreItemType.TableSingle9 ||
                   type == StoreItemType.TableSingle10;
        }

        public static bool IsSnackMachine(StoreItemType type)
        {
            return type == StoreItemType.SnackMachine1 ||
                   type == StoreItemType.SnackMachine2 ||
                   type == StoreItemType.SnackMachine3 ||
                   type == StoreItemType.SnackMachine4 ||
                   type == StoreItemType.SnackMachine5 ||
                   type == StoreItemType.SnackMachine6 ||
                   type == StoreItemType.SnackMachine7 ||
                   type == StoreItemType.SnackMachine8 ||
                   type == StoreItemType.SnackMachine9 ||
                   type == StoreItemType.SnackMachine10;
        }

        public static bool IsKitchen(StoreItemType type)
        {
            return type == StoreItemType.Kitchen1 ||
                   type == StoreItemType.Kitchen2 ||
                   type == StoreItemType.Kitchen3 ||
                   type == StoreItemType.Kitchen4 ||
                   type == StoreItemType.Kitchen5 ||
                   type == StoreItemType.Kitchen6 ||
                   type == StoreItemType.Kitchen7 ||
                   type == StoreItemType.Kitchen8 ||
                   type == StoreItemType.Kitchen9 ||
                   type == StoreItemType.Kitchen10;
        }
    }
}
