using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public static class PlayerData
{
    private static TextMeshProUGUI moneyText, levelText;
    private static Slider expirienceSlider;
    private static List<GameGridObject> storedIventory, Inventory;
    private static HashSet<string> setStoredInventory; // Saved stored inventory by ID
    private static DataGameUser user;

    // Recieves the reference to the UI Text
    public static void SetPlayerData(TextMeshProUGUI moneyText, TextMeshProUGUI levelText, Slider expirienceSlider)
    {
        PlayerData.moneyText = moneyText;
        PlayerData.levelText = levelText;
        PlayerData.expirienceSlider = expirienceSlider;

        SetLevel();

        moneyText.text = GetMoney();
        Inventory = new List<GameGridObject>();
        storedIventory = new List<GameGridObject>();
        setStoredInventory = new HashSet<string>();
    }

    private static void SetTopBarValues()
    {
        SetLevel();
        moneyText.text = GetMoney();
    }

    public static void AddExperienve(double amount)
    {
        user.EXPERIENCE += amount;
        SetLevel();
    }

    public static void AddMoney(double amount)
    {
        AddStatData(PlayerStats.MONEY, amount);
        user.GAME_MONEY += amount;
        moneyText.text = GetMoney();
    }

    public static void SetCustomerAttended()
    {
        AddStatData(PlayerStats.CLIENTS_ATTENDED, 1);
    }

    public static bool IncreaseUpgrade(StoreGameObject upgrade)
    {
        // or max level reached we return
        if (!CanSubtract(upgrade.Cost) || user.GetUpgrade(upgrade.UpgradeType) >= upgrade.MaxLevel)
        {
            return false;
        }

        Subtract(upgrade.Cost, ObjectType.UPGRADE_ITEM);
        user.IncreaseUpgrade(upgrade.UpgradeType);

        // if the upgrade requires to increase the game floor 
        if (upgrade.UpgradeType == UpgradeType.GRID_SIZE)
        {
            //we pause the game while reloading
            Time.timeScale = 0;
            BussGrid.ReloadTilemapGameFloor(GameObject.Find(GetTileBussFloor()).GetComponent<Tilemap>());
            Time.timeScale = 1;
        }

        if (upgrade.UpgradeType == UpgradeType.NUMBER_CLIENTS)
        {
            BussGrid.GameController.UpdateClientNumber();
        }

        //UPGRADE: UPGRADE_LOAD_SPEED
        if (upgrade.UpgradeType == UpgradeType.UPGRADE_LOAD_SPEED)
        {
            UpdateObjectLoadSpeed();
        }

        return true;
    }

    private static void UpdateObjectLoadSpeed()
    {
        foreach (GameGridObject obj in Inventory)
        {
            if (!IsItemStored(obj.Name))
            {
                obj.UpdateLoadItemSlider();
            }
        }
    }

    public static void SubtractFromStorage(GameGridObject gameGridObject)
    {
        setStoredInventory.Remove(gameGridObject.Name);
    }

    public static void Subtract(double amount, ObjectType type)
    {
        if (!CanSubtract(amount))
        {
            return;
        }
        user.GAME_MONEY -= amount;
        AddStatData(PlayerStats.MONEY_SPENT, amount);

        if (type != ObjectType.UPGRADE_ITEM)
        {
            AddStatData(PlayerStats.ITEMS_BOUGHT, 1);
        }

        AddExperienve(PlayerLevelCalculator.GetExperienceFromMoneySpent(amount));
        SetLevel();
        moneyText.text = GetMoney();
    }

    public static bool CanSubtract(double amount)
    {
        return user.GAME_MONEY - amount >= 0;
    }

    public static void SetLevel()
    {
        int PrevLevel = user.LEVEL;
        user.LEVEL = PlayerLevelCalculator.GetLevel(user.EXPERIENCE);
        if (PrevLevel < user.LEVEL)
        {
            // TODO: Pop Up Level up, and add to a co-routine
            // We save the data in case of app rewards
            SaveGame();
        }
        levelText.text = TextUI.CurrentLevel + " " + GetLevel();
        expirienceSlider.value = PlayerLevelCalculator.GetExperienceToNextLevelPercentage(user.EXPERIENCE) / 100f;
    }

    public static string GetMoney()
    {
        return Util.convertToTextAndReduceCurrency(Math.Clamp(user.GAME_MONEY, 0, Settings.PlayerMoneyLimit));
    }

    public static string GetLevel()
    {
        return user.LEVEL.ToString();
    }
    public static double GetMoneyDouble()
    {
        return user.GAME_MONEY;
    }

    public static void StoreItem(GameGridObject obj)
    {
        storedIventory.Add(obj);
        setStoredInventory.Add(obj.Name);
    }

    public static bool IsItemStored(string nameID)
    {
        return setStoredInventory.Contains(nameID);
    }

    public static bool IsItemInInventory(GameGridObject obj)
    {
        return Inventory.Contains(obj);
    }

    public static void AddItemToInventory(GameGridObject obj)
    {
        Inventory.Add(obj);
    }

    public static void RemoveFromInventory(GameGridObject obj)
    {
        Inventory.Remove(obj);
    }

    public static string GenerateID()
    {
        return Guid.NewGuid().ToString() + "." + Guid.NewGuid().ToString().Substring(0, 5);
    }

    // Called at the end of Unity Auth service to load the user with the response of the cloud code function
    public static void InitUser()
    {
        string[] filesaves = UtilJSONFile.GetSaveFiles();

        if (filesaves.Length > 0)
        {
            string json = UtilJSONFile.GetJsonFromFile(filesaves[filesaves.Length - 1]);
            user = DataGameUser.CreateFromJSON(json);
        }
        else
        {
            user = GetNewUser();
            user.SaveToJSONFileAsync();
        }
    }

    public static DataGameUser GetNewUser()
    {
        DataGameUser dataGameUser = new DataGameUser
        {
            NAME = "undefined",
            LANGUAGE_CODE = "en_US",
            INTERNAL_ID = "",
            GAME_MONEY = Settings.InitGameMoney,
            EXPERIENCE = Settings.InitExperience,
            LEVEL = Settings.InitLevel,
            EMAIL = "undefined@undefined.com",
            AUTH_TYPE = (int)AuthSource.ANONYMOUS,
            LAST_SAVE = 0,
            CREATED_AT = 0,
            OBJECTS = new List<DataGameObject>{
                    new DataGameObject{
                        ID = (int) StoreItemType.SNACK_MACHINE_1,
                        POSITION = new int[]{Settings.StartStoreItemDispenser[0], Settings.StartStoreItemDispenser[1]},
                        IS_STORED = false,
                        ROTATION = (int) ObjectRotation.BACK,
                    },
                    new DataGameObject{
                        ID = (int) StoreItemType.TABLE_SINGLE_1,
                        POSITION =  new int[]{Settings.StartTable[0], Settings.StartTable[1]},
                        IS_STORED = false,
                        ROTATION = (int) ObjectRotation.FRONT,
                    },
                    new DataGameObject{
                        ID = (int) StoreItemType.COUNTER_1,
                        POSITION =  new int[]{Settings.StartCounter[0], Settings.StartCounter[1]},
                        IS_STORED = false,
                        ROTATION = (int) ObjectRotation.FRONT,
                    },
                },
            UPGRADES = new List<UpgradeGameObject> { },
            DATA_STATS = new DataStatsGameObject()
            {
                MONEY_EARNED = 0,
                MONEY_SPENT = 0,
                CLIENTS_ATTENDED = 0,
                ITEMS_BOUGHT = 0
            }
        };

        foreach (UpgradeType upgradeType in Enum.GetValues(typeof(UpgradeType)))
        {
            dataGameUser.UPGRADES.Add(new UpgradeGameObject
            {
                ID = (int)upgradeType,
                UPGRADE_NUMBER = 0
            });
        }

        dataGameUser.SetUpgrade(UpgradeType.GRID_SIZE, Settings.InitGridSize);
        return dataGameUser;
    }

    public static void AddStatData(PlayerStats stat, int val)
    {
        switch (stat)
        {
            case PlayerStats.ITEMS_BOUGHT: user.DATA_STATS.ITEMS_BOUGHT += val; return;
            case PlayerStats.CLIENTS_ATTENDED: user.DATA_STATS.CLIENTS_ATTENDED += val; return;
        }
    }

    public static void AddStatData(PlayerStats stat, Double val)
    {
        switch (stat)
        {
            case PlayerStats.MONEY_SPENT: user.DATA_STATS.MONEY_SPENT += val; return;
            case PlayerStats.MONEY: user.DATA_STATS.MONEY_EARNED += val; return;
        }
    }

    // Control times in which we save the game
    // Saves when the user closes the app
    private static void Quit()
    {
        // In case the user is not logged in
        if (user != null)
        {
            SaveGame();
        }
    }

    public static void SaveGame()
    {
        user.LAST_SAVE = DateTime.Now.ToFileTimeUtc();
        user.SaveToJSONFileAsync();
    }

    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        Application.quitting += Quit;
    }

    public static bool IsUserSetted()
    {
        return user != null;
    }

    public static List<DataGameObject> GerUserObjects()
    {
        return user.OBJECTS;
    }

    public static int GetUgrade(UpgradeType upgradeType)
    {
        return user.GetUpgrade(upgradeType);
    }

    public static int GetNumberCounters()
    {
        int count = 0;

        foreach (DataGameObject obj in user.OBJECTS)
        {
            count += GameObjectList.IsCounter(obj.GetStoreItemType()) ? 1 : 0;
        }
        return count;
    }

    // Return the buss floor depending on the grid size
    public static string GetTileBussFloor()
    {
        int gridSize = user.GetUpgrade(UpgradeType.GRID_SIZE);

        switch (gridSize)
        {
            case 1: return Settings.TilemapBusinessFloor;
            case 2: return Settings.TilemapBusinessFloor_2;
            case 3: return Settings.TilemapBusinessFloor_3;
            case 4: return Settings.TilemapBusinessFloor_4;
            case 5: return Settings.TilemapBusinessFloor_5;
            case 6: return Settings.TilemapBusinessFloor_6;
            case 7: return Settings.TilemapBusinessFloor_7;
            case 8: return Settings.TilemapBusinessFloor_8;
            case 9: return Settings.TilemapBusinessFloor_9;
            case 10: return Settings.TilemapBusinessFloor_10;
        }

        if (gridSize >= 10)
        {
            return Settings.TilemapBusinessFloor_10;
        }

        return "";
    }
    // For new items, cannot be stored if it was recently added
    public static void AddDataGameObject(GameGridObject obj)
    {
        DataGameObject newObj = new DataGameObject()
        {
            ID = (int)obj.GetStoreGameObject().StoreItemType,
            POSITION = new int[] { obj.GridPosition.x, obj.GridPosition.y },
            IS_STORED = false,
            ROTATION = (int)obj.GetFacingPosition()
        };
        obj.SetDataGameObject(newObj);
        user.OBJECTS.Add(newObj);
    }

    public static List<GameGridObject> GetStoredIventory()
    {
        return storedIventory;
    }

    public static GameGridObject GetGameGridObject(string ID)
    {
        foreach (GameGridObject obj in storedIventory)
        {
            if (obj.Name.Equals(ID))
            {
                return obj;
            }
        }
        return null;
    }

    public static Transform GetMoneyTextTransform()
    {
        return moneyText.transform;
    }

    public static string ToStringDebug()
    {
        return "NAME: " + user.NAME +
        " EMAIL: " + user.EMAIL +
        " EXPERIENCE: " + user.EXPERIENCE +
        " GAME_MONEY: " + user.GAME_MONEY +
        " INTERNAL_ID: " + user.INTERNAL_ID +
        " LANGUAGE_CODE: " + user.LANGUAGE_CODE +
        " LAST_SAVE: " + user.LAST_SAVE +
        " LEVEL: " + user.LEVEL +
        " OBJECTS.Count: " + user.OBJECTS.Count;
    }

    public static string GetStats()
    {
        return " Money Earned: " + user.DATA_STATS.MONEY_EARNED
        + " \n Money spent: " + user.DATA_STATS.MONEY_SPENT
        + " \n Clients attended: " + user.DATA_STATS.CLIENTS_ATTENDED
        + " \n Items bought: " + user.DATA_STATS.ITEMS_BOUGHT;
    }
}