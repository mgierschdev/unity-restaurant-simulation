using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class PlayerData
{
    private static TextMeshProUGUI moneyText, levelText, gemsText;
    private static Slider expirienceSlider;
    private static List<GameGridObject> storedIventory, Inventory;
    private static HashSet<string> setStoredInventory; // Saved stored inventory by ID
    private static DataGameUser user;

    // Recieves the reference to the UI Text
    public static void SetPlayerData(TextMeshProUGUI moneyText, TextMeshProUGUI levelText, TextMeshProUGUI gemsText, Slider expirienceSlider)
    {
        PlayerData.moneyText = moneyText;
        PlayerData.gemsText = gemsText;
        PlayerData.levelText = levelText;
        PlayerData.expirienceSlider = expirienceSlider;

        SetLevel();

        moneyText.text = GetMoney();
        gemsText.text = GetGems();
        Inventory = new List<GameGridObject>();
        storedIventory = new List<GameGridObject>();
        setStoredInventory = new HashSet<string>();
    }

    private static void SetTopBarValues()
    {
        SetLevel();
        moneyText.text = GetMoney();
        gemsText.text = GetGems();
    }

    public static void AddExperienve(double amount)
    {
        user.EXPERIENCE += amount;
        SetLevel();
    }

    public static void AddGems(double amount)
    {
        user.GEMS += amount;
        gemsText.text = GetGems();
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
        return true;
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
        return Util.convertToTextAndReduceCurrency(user.GAME_MONEY) + "$";
    }

    public static string GetGems()
    {
        return Util.convertToTextAndReduceCurrency(user.GEMS);
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
            INTERNAL_ID = GenerateID(),
            GAME_MONEY = Settings.InitGameMoney,
            GEMS = Settings.InitGems,
            EXPERIENCE = Settings.InitExperience,
            LEVEL = Settings.InitLevel,
            EMAIL = "undefined@undefined.com",
            AUTH_TYPE = (int)AuthSource.ANONYMOUS,
            LAST_SAVE = DateTime.Now.ToFileTimeUtc(),
            CREATED_AT = DateTime.Now.ToFileTimeUtc(),
            OBJECTS = new List<DataGameObject>{
                    new DataGameObject{
                        ID = (int) StoreItemType.STORE_ITEM_ORANGE_JUICE,
                        POSITION = new int[]{Settings.StartStoreItemDispenser[0], Settings.StartStoreItemDispenser[1]},
                        IS_STORED = false,
                        ROTATION = (int) ObjectRotation.FRONT,
                    },
                    new DataGameObject{
                        ID = (int) StoreItemType.WOODEN_TABLE_SINGLE,
                        POSITION =  new int[]{Settings.StartTable[0], Settings.StartTable[1]},
                        IS_STORED = false,
                        ROTATION = (int) ObjectRotation.FRONT,
                    },
                    new DataGameObject{
                        ID = (int) StoreItemType.COUNTER,
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
    // TODO: Saves every 10 minutes, add to coroutine
    private static void Quit()
    {
        SaveGame();
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

    public static DataGameUser GetDataGameUser()
    {
        return user;
    }

    public static int GetUgrade(UpgradeType upgradeType)
    {
        return user.GetUpgrade(upgradeType);
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

    public static string GetStats()
    {
        return " Money Earned: " + user.DATA_STATS.MONEY_EARNED
        + " \n Money spent: " + user.DATA_STATS.MONEY_SPENT
        + " \n Clients attended: " + user.DATA_STATS.CLIENTS_ATTENDED
        + " \n Items bought: " + user.DATA_STATS.ITEMS_BOUGHT;
    }
}