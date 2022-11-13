using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class PlayerData
{
    private static TextMeshProUGUI moneyText, levelText, gemsText;
    private static Slider expirienceSlider;
    private static List<GameGridObject> storedIventory, Inventory;
    private static HashSet<string> setStoredInventory; // Saved stored inventory by ID
    private static FirebaseGameUser user;

    // Recieves the reference to the UI Text
    public static void SetPlayerData(TextMeshProUGUI moneyText, TextMeshProUGUI levelText, TextMeshProUGUI gemsText, Slider expirienceSlider)
    {
        PlayerData.moneyText = moneyText;
        PlayerData.gemsText = gemsText;
        PlayerData.levelText = levelText;
        PlayerData.expirienceSlider = expirienceSlider;

        SetLevel();

        moneyText.text = GetMoney();
        gemsText.text = user.GEMS.ToString();
        Inventory = new List<GameGridObject>();
        storedIventory = new List<GameGridObject>();
        setStoredInventory = new HashSet<string>();
    }

    private static void SetTopBarValues()
    {
        SetLevel();
        moneyText.text = GetMoney();
        gemsText.text = user.GEMS.ToString();
    }

    public static void AddExperienve(double amount)
    {
        user.EXPERIENCE += amount;
        SetLevel();
    }

    public static void AddGems(double amount)
    {
        user.GEMS += amount;
        gemsText.text = user.GEMS.ToString();
    }

    public static void AddMoney(double amount)
    {
        user.GAME_MONEY += amount;
        moneyText.text = GetMoney();
    }

    public static void SubtractFromStorage(GameGridObject gameGridObject)
    {
        setStoredInventory.Remove(gameGridObject.Name);
    }

    public static void Subtract(double amount)
    {
        if (!CanSubtract(amount))
        {
            return;
        }
        user.GAME_MONEY -= amount;
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
            // TODO: Pop Up Level up
            // We save the data in case of app rewards
            Firestore.SaveUser();
        }
        levelText.text = GetLevel();
        expirienceSlider.value = PlayerLevelCalculator.GetExperienceToNextLevelPercentage(user.EXPERIENCE) / 100f;
    }

    public static string GetMoney()
    {
        return user.GAME_MONEY + "$";
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

    private static string GenerateID()
    {
        return Guid.NewGuid().ToString() + "." + Guid.NewGuid().ToString().Substring(0, 5);
    }

    public static async void InitUser(FirebaseAuth auth)
    {
        string userId;
        FirebaseUser firebaseUser = auth.CurrentUser;
        // Init user, worst case it will be replaced by a new user, to avoid any async exception
        SetEmptyUser();

        // TODO: validation cloud functions
        if (Settings.IsFirebaseEmulatorEnabled)
        {
            userId = Settings.TEST_USER;
        }
        else if (firebaseUser != null)
        {
            userId = firebaseUser.UserId;
        }
        else
        {
            //we set one temporal id
            userId = GenerateID();
        }

        DocumentSnapshot snapshot = await Firestore.GetUserData(userId);

        // The player ID does not exist, we create a new one
        if (snapshot == null)
        {
            user = new FirebaseGameUser
            {
                NAME = firebaseUser == null ? "undefined" : firebaseUser.DisplayName,
                LANGUAGE_CODE = auth.LanguageCode,
                INTERNAL_ID = GenerateID(),
                GAME_MONEY = Settings.InitGameMoney,
                GEMS = Settings.InitGems,
                EXPERIENCE = Settings.InitExperience,
                LEVEL = Settings.InitLevel,
                FIREBASE_AUTH_ID = userId, // Is set to temp 213213123scsacsc.asdas2 if offline
                EMAIL = firebaseUser == null ? "undefined@undefined.com" : firebaseUser.Email,
                AUTH_TYPE = (int)(firebaseUser.IsAnonymous ? AuthSource.ANONYMOUS : AuthSource.UNDEFINED),
                LAST_LOGIN = FieldValue.ServerTimestamp,
                CREATED_AT = FieldValue.ServerTimestamp,
                GRID_SIZE = Settings.InitGridSize,
                OBJECTS = new List<FirebaseGameObject>{
                    new FirebaseGameObject{
                        ID = (int) StoreItemType.WOODEN_BASE_CONTAINER,
                        POSITION = new int[]{Settings.StartContainer[0], Settings.StartContainer[1]},
                        IS_STORED = false,
                        ROTATION = (int) ObjectRotation.FRONT
                    },
                    new FirebaseGameObject{
                        ID = (int) StoreItemType.WOODEN_TABLE_SINGLE,
                        POSITION =  new int[]{Settings.StartTable[0], Settings.StartTable[1]},
                        IS_STORED = false,
                        ROTATION = (int) ObjectRotation.FRONT
                    },
                }
            };

            await Firestore.SaveUser();
        }
        else
        {
            user = snapshot.ConvertTo<FirebaseGameUser>();
            if (user.OBJECTS == null)
            {
                user.OBJECTS = new List<FirebaseGameObject>();
            }
        }
    }

    public static void SetEmptyUser()
    {
        user = new FirebaseGameUser
        {
            NAME = "undefined",
            LANGUAGE_CODE = "es_ES",
            INTERNAL_ID = GenerateID(),
            GAME_MONEY = Settings.InitGameMoney,
            GEMS = Settings.InitGems,
            EXPERIENCE = Settings.InitExperience,
            LEVEL = Settings.InitLevel,
            FIREBASE_AUTH_ID = GenerateID(),
            EMAIL = "undefined@undefined.com",
            AUTH_TYPE = (int)AuthSource.ANONYMOUS,
            LAST_LOGIN = FieldValue.ServerTimestamp,
            CREATED_AT = FieldValue.ServerTimestamp,
            GRID_SIZE = Settings.InitGridSize,
            OBJECTS = new List<FirebaseGameObject>{
                    new FirebaseGameObject{
                        ID = (int) StoreItemType.WOODEN_BASE_CONTAINER,
                        POSITION = new int[]{Settings.StartContainer[0], Settings.StartContainer[1]},
                        IS_STORED = false,
                        ROTATION = (int) ObjectRotation.FRONT
                    },
                    new FirebaseGameObject{
                        ID = (int) StoreItemType.WOODEN_TABLE_SINGLE,
                        POSITION =  new int[]{Settings.StartTable[0], Settings.StartTable[1]},
                        IS_STORED = false,
                        ROTATION = (int) ObjectRotation.FRONT
                    },
                }
        };
    }

    // Control times in which we save the game
    // Saves when the user closes the app
    // TODO: Saves every 10 minutes
    private async static void Quit()
    {
        Task task = Firestore.SaveUser();
        await task;
    }

    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        Application.quitting += Quit;
    }

    public static FirebaseGameUser GetFirebaseGameUser()
    {
        return user;
    }

    // Return the buss floor depending on the grid size
    public static string GetTileBussFloor()
    {
        int gridSize = user.GRID_SIZE;

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
    public static void AddFirebaseGameObject(GameGridObject obj)
    {
        FirebaseGameObject newObj = new FirebaseGameObject()
        {
            ID = (int)obj.GetStoreGameObject().StoreItemType,
            POSITION = new int[] { obj.GridPosition.x, obj.GridPosition.y },
            IS_STORED = false,
            ROTATION = (int)obj.GetFacingPosition()
        };
        obj.SetFirebaseGameObject(newObj);
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
}