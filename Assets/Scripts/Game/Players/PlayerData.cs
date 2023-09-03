using System;
using System.Collections.Generic;
using Game.Grid;
using Game.Players.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Util;

namespace Game.Players
{
    public static class PlayerData
    {
        private static TextMeshProUGUI _moneyText, _levelText;
        private static Slider _experienceSlider;
        private static List<GameGridObject> _storedInventory, _inventory;
        private static HashSet<string> _setStoredInventory; // Saved stored inventory by ID
        private static DataGameUser _user;

        // Recieves the reference to the UI Text
        public static void SetPlayerData(TextMeshProUGUI moneyText, TextMeshProUGUI levelText, Slider expirienceSlider)
        {
            PlayerData._moneyText = moneyText;
            PlayerData._levelText = levelText;
            PlayerData._experienceSlider = expirienceSlider;

            SetLevel();

            moneyText.text = GetMoney();
            _inventory = new List<GameGridObject>();
            _storedInventory = new List<GameGridObject>();
            _setStoredInventory = new HashSet<string>();
        }

        private static void SetTopBarValues()
        {
            SetLevel();
            _moneyText.text = GetMoney();
        }

        public static void AddExperience(double amount)
        {
            _user.experience += amount;
            SetLevel();
        }

        public static void AddMoney(double amount)
        {
            AddStatData(PlayerStats.Money, amount);
            _user.gameMoney += amount;
            _moneyText.text = GetMoney();
        }

        public static void SetCustomerAttended()
        {
            AddStatData(PlayerStats.ClientsAttended, 1);
        }

        public static bool IncreaseUpgrade(StoreGameObject upgrade)
        {
            // or max level reached we return
            if (!CanSubtract(upgrade.Cost) || _user.GetUpgrade(upgrade.UpgradeType) >= upgrade.MaxLevel)
            {
                return false;
            }

            Subtract(upgrade.Cost, ObjectType.UpgradeItem);
            _user.IncreaseUpgrade(upgrade.UpgradeType);

            // if the upgrade requires to increase the game floor 
            if (upgrade.UpgradeType == UpgradeType.GridSize)
            {
                //we pause the game while reloading
                Time.timeScale = 0;
                BussGrid.ReloadTilemapGameFloor(GameObject.Find(GetTileBussFloor()).GetComponent<Tilemap>());
                Time.timeScale = 1;
            }

            if (upgrade.UpgradeType == UpgradeType.NumberClients)
            {
                BussGrid.GameController.UpdateClientNumber();
            }

            //UPGRADE: UPGRADE_LOAD_SPEED
            if (upgrade.UpgradeType == UpgradeType.UpgradeLoadSpeed)
            {
                UpdateObjectLoadSpeed();
            }

            return true;
        }

        private static void UpdateObjectLoadSpeed()
        {
            foreach (GameGridObject obj in _inventory)
            {
                if (!IsItemStored(obj.Name))
                {
                    obj.UpdateLoadItemSlider();
                }
            }
        }

        public static void SubtractFromStorage(GameGridObject gameGridObject)
        {
            _setStoredInventory.Remove(gameGridObject.Name);
        }

        public static void Subtract(double amount, ObjectType type)
        {
            if (!CanSubtract(amount))
            {
                return;
            }

            _user.gameMoney -= amount;
            AddStatData(PlayerStats.MoneySpent, amount);

            if (type != ObjectType.UpgradeItem)
            {
                AddStatData(PlayerStats.ItemsBought, 1);
            }

            AddExperience(PlayerLevelCalculator.GetExperienceFromMoneySpent(amount));
            SetLevel();
            _moneyText.text = GetMoney();
        }

        public static bool CanSubtract(double amount)
        {
            return _user.gameMoney - amount >= 0;
        }

        public static void SetLevel()
        {
            var prevLevel = _user.level;
            _user.level = PlayerLevelCalculator.GetLevel(_user.experience);
            if (prevLevel < _user.level)
            {
                SaveGame();
            }

            _levelText.text = TextUI.CurrentLevel + " " + GetLevel();
            _experienceSlider.value = PlayerLevelCalculator.GetExperienceToNextLevelPercentage(_user.experience) / 100f;
        }

        public static string GetMoney()
        {
            return Util.Util.ConvertToTextAndReduceCurrency(Math.Clamp(_user.gameMoney, 0, Settings.PlayerMoneyLimit));
        }

        public static string GetLevel()
        {
            return _user.level.ToString();
        }

        public static double GetMoneyDouble()
        {
            return _user.gameMoney;
        }

        public static void StoreItem(GameGridObject obj)
        {
            _storedInventory.Add(obj);
            _setStoredInventory.Add(obj.Name);
        }

        public static bool IsItemStored(string nameID)
        {
            return _setStoredInventory.Contains(nameID);
        }

        public static bool IsItemInInventory(GameGridObject obj)
        {
            return _inventory.Contains(obj);
        }

        public static void AddItemToInventory(GameGridObject obj)
        {
            _inventory.Add(obj);
        }

        public static void RemoveFromInventory(GameGridObject obj)
        {
            _inventory.Remove(obj);
        }

        public static string GenerateID()
        {
            return Guid.NewGuid().ToString() + "." + Guid.NewGuid().ToString().Substring(0, 5);
        }

        // Called at the end of Unity Auth service to load the user with the response of the cloud code function
        public static void InitUser()
        {
            string[] filesaves = UtilJsonFile.GetSaveFiles();

            if (filesaves.Length > 0)
            {
                string json = UtilJsonFile.GetJsonFromFile(filesaves[filesaves.Length - 1]);
                _user = DataGameUser.CreateFromJson(json);
            }
            else
            {
                _user = GetNewUser();
                _user.SaveToJsonFileAsync();
            }
        }

        public static DataGameUser GetNewUser()
        {
            DataGameUser dataGameUser = new DataGameUser
            {
                name = "undefined",
                languageCode = "en_US",
                internalID = "",
                gameMoney = Settings.InitGameMoney,
                experience = Settings.InitExperience,
                level = Settings.InitLevel,
                email = "undefined@undefined.com",
                authType = (int)AuthSource.Anonymous,
                objects = new List<DataGameObject>
                {
                    new DataGameObject
                    {
                        id = (int)StoreItemType.SnackMachine1,
                        position = new[] { Settings.StartStoreItemDispenser[0], Settings.StartStoreItemDispenser[1] },
                        isStored = false,
                        rotation = (int)ObjectRotation.Front,
                    },
                    new DataGameObject
                    {
                        id = (int)StoreItemType.TableSingle1,
                        position = new[] { Settings.StartTable[0], Settings.StartTable[1] },
                        isStored = false,
                        rotation = (int)ObjectRotation.Back,
                    },
                    new DataGameObject
                    {
                        id = (int)StoreItemType.Counter1,
                        position = new[] { Settings.StartCounter[0], Settings.StartCounter[1] },
                        isStored = false,
                        rotation = (int)ObjectRotation.Back,
                    },
                },
                upgrades = new List<UpgradeGameObject>
                {
                    Capacity = 0
                },
                dataStats = new DataStatsGameObject
                {
                    moneyEarned = 0,
                    moneySpent = 0,
                    clientsAttended = 0,
                    itemsBought = 0
                }
            };

            foreach (UpgradeType upgradeType in Enum.GetValues(typeof(UpgradeType)))
            {
                dataGameUser.upgrades.Add(new UpgradeGameObject
                {
                    id = (int)upgradeType,
                    upgradeNumber = 0
                });
            }

            dataGameUser.SetUpgrade(UpgradeType.GridSize, Settings.InitGridSize);
            return dataGameUser;
        }

        public static void AddStatData(PlayerStats stat, int val)
        {
            switch (stat)
            {
                case PlayerStats.ItemsBought:
                    _user.dataStats.itemsBought += val;
                    return;
                case PlayerStats.ClientsAttended:
                    _user.dataStats.clientsAttended += val;
                    return;
            }
        }

        public static void AddStatData(PlayerStats stat, Double val)
        {
            switch (stat)
            {
                case PlayerStats.MoneySpent:
                    _user.dataStats.moneySpent += val;
                    return;
                case PlayerStats.Money:
                    _user.dataStats.moneyEarned += val;
                    return;
            }
        }

        // Control times in which we save the game
        // Saves when the user closes the app
        private static void Quit()
        {
            // In case the user is not logged in
            if (_user != null)
            {
                SaveGame();
            }
        }

        public static void SaveGame()
        {
            _user.SaveToJsonFileAsync();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart()
        {
            Application.quitting += Quit;
        }

        public static bool IsUserSetted()
        {
            return _user != null;
        }

        public static List<DataGameObject> GerUserObjects()
        {
            return _user.objects;
        }

        public static int GetUgrade(UpgradeType upgradeType)
        {
            return _user.GetUpgrade(upgradeType);
        }

        public static int GetNumberCounters()
        {
            int count = 0;

            foreach (DataGameObject obj in _user.objects)
            {
                count += GameObjectList.IsCounter(obj.GetStoreItemType()) ? 1 : 0;
            }

            return count;
        }

        // Return the buss floor depending on the grid size
        public static string GetTileBussFloor()
        {
            int gridSize = _user.GetUpgrade(UpgradeType.GridSize);

            switch (gridSize)
            {
                case 1: return Settings.TilemapBusinessFloor;
                case 2: return Settings.TilemapBusinessFloor2;
                case 3: return Settings.TilemapBusinessFloor3;
                case 4: return Settings.TilemapBusinessFloor4;
                case 5: return Settings.TilemapBusinessFloor5;
                case 6: return Settings.TilemapBusinessFloor6;
                case 7: return Settings.TilemapBusinessFloor7;
                case 8: return Settings.TilemapBusinessFloor8;
                case 9: return Settings.TilemapBusinessFloor9;
                case 10: return Settings.TilemapBusinessFloor10;
            }

            if (gridSize >= 10)
            {
                return Settings.TilemapBusinessFloor10;
            }

            return "";
        }

        // For new items, cannot be stored if it was recently added
        public static void AddDataGameObject(GameGridObject obj)
        {
            DataGameObject newObj = new DataGameObject()
            {
                id = (int)obj.GetStoreGameObject().StoreItemType,
                position = new[] { obj.GridPosition.x, obj.GridPosition.y },
                isStored = false,
                rotation = (int)obj.GetFacingPosition()
            };
            obj.SetDataGameObject(newObj);
            _user.objects.Add(newObj);
        }

        public static List<GameGridObject> GetStoredIventory()
        {
            return _storedInventory;
        }

        public static GameGridObject GetGameGridObject(string id)
        {
            foreach (var obj in _storedInventory)
            {
                if (obj.Name.Equals(id))
                {
                    return obj;
                }
            }

            return null;
        }

        public static Transform GetMoneyTextTransform()
        {
            return _moneyText.transform;
        }

        public static string ToStringDebug()
        {
            return "NAME: " + _user.name +
                   " EMAIL: " + _user.email +
                   " EXPERIENCE: " + _user.experience +
                   " GAME_MONEY: " + _user.gameMoney +
                   " INTERNAL_ID: " + _user.internalID +
                   " LANGUAGE_CODE: " + _user.languageCode +
                   " LEVEL: " + _user.level +
                   " OBJECTS.Count: " + _user.objects.Count;
        }

        public static string GetStats()
        {
            return " Money Earned: " + _user.dataStats.moneyEarned
                                     + " \n Money spent: " + _user.dataStats.moneySpent
                                     + " \n Clients attended: " + _user.dataStats.clientsAttended
                                     + " \n Items bought: " + _user.dataStats.itemsBought;
        }
    }
}