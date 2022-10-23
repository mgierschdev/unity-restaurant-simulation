using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// [FirestoreData]
public static class PlayerData
{
    public static List<string> Name;
    public static String EmailID; // mandatory
    public static String InternalID = "undefined"; // internal app id
    public static String FireappAuthID = "undefined"; // Given by firebase
    public static String LanguageCode = "undefined"; // In the standard format (Locale) en_US, es_ES ...
    // [FirestoreProperty]
    // public static object LastLogin { get; set; } // this is the default for the firestore server timestamp
    // [FirestoreProperty]
    // public static object SignInDate { get; set; } // this is the default for the firestore server timestamp
    private static TextMeshProUGUI moneyText;
    private static TextMeshProUGUI levelText;
    private static TextMeshProUGUI gemsText;
    private static Slider expirienceSlider;
    private static List<GameGridObject> storedIventory;
    private static List<GameGridObject> Inventory;
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
            //TODO: Pop Up Level up
            //We save the data in case of app rewards
            // GameLog.Log("Setting player data " + GetUserAsMap().ToString());
            //Firestore.SaveUserData(GetUserAsMap());
            Firestore.SaveObject(user);
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
        return Guid.NewGuid().ToString();
    }

    public static async void InitUser(FirebaseUser firebaseUser)
    {
        bool isNewUser = false;
        string userId;

        if (Settings.IsFirebaseEmulatorEnabled)
        {
            userId = Settings.TEST_USER;
        }
        else
        {

        }

        Debug.Log("firebase " + firebaseUser.UserId);

        if (isNewUser)
        {
            user = new FirebaseGameUser
            {
                NAME = "Name", // This will append (to the existing fields even if they are the same) to the list if firestore is collec with merge
                LANGUAGE_CODE = "LanguageCode",
                INTERNAL_ID = GenerateID(),
                GAME_MONEY = 0,
                GEMS = 40,
                EXPERIENCE = 0,
                LEVEL = 0,
                FIREBASE_AUTH_ID = firebaseUser == null ? GenerateID() : firebaseUser.UserId,
                EMAIL = firebaseUser == null ? "test@gmail.com" : firebaseUser.Email,
                AUTH_TYPE = (int)(firebaseUser.IsAnonymous ? AuthSource.ANONYMOUS : AuthSource.UNDEFINED),
                LAST_LOGIN = FieldValue.ServerTimestamp,
                CREATED_AT = FieldValue.ServerTimestamp
            };
            await Firestore.SaveObject(user);
        }
        else
        {



        }

    }

    public static void SetMockUser()
    {
        InitUser(null);
    }

    // public static Dictionary<string, object> GetUserAsMap()
    // {
    //     return new Dictionary<string, object>{
    //         {FirestorePlayerAttributes.NAME, Name}, // This will append (to the existing fields even if they are the same) to the list if firestore is collec with merge
    //         {FirestorePlayerAttributes.LANGUAGE_CODE, LanguageCode},
    //         {FirestorePlayerAttributes.INTERNAL_ID, InternalID},
    //         {FirestorePlayerAttributes.GAME_MONEY, user.GAME_MONEY},
    //         {FirestorePlayerAttributes.GEMS, user.GEMS},
    //         {FirestorePlayerAttributes.EXPERIENCE, user.EXPERIENCE},
    //         {FirestorePlayerAttributes.LEVEL, user.LEVEL},
    //         {FirestorePlayerAttributes.FIREBASE_AUTH_ID, FireappAuthID},
    //         {FirestorePlayerAttributes.AUTH_TYPE, user.AUTH_TYPE},
    //         {FirestorePlayerAttributes.LAST_LOGIN, FieldValue.ServerTimestamp}
    //         // {FirestorePlayerAttributes.CREATED_AT, FieldValue.ServerTimestamp} //Setted only th first time
    //     };
    // }

    public static void LoadFirebaseDocument(DocumentSnapshot data)
    {
        // Debug.Log("LoadFirebaseDocument");
        // try
        // {
        //     Dictionary<string, object> dic = data.ToDictionary();
        //     List<object> genericList = (List<object>)dic[FirestorePlayerAttributes.NAME];
        //     Name = new List<String>(){
        //     (String) genericList[0],
        //     (String) genericList[1],
        // };


        //     GameMoney = (Double)dic[FirestorePlayerAttributes.GAME_MONEY];
        //     Gems = (Double)dic[FirestorePlayerAttributes.GEMS];
        //     LanguageCode = (String)dic[FirestorePlayerAttributes.LANGUAGE_CODE];
        //     InternalID = (String)dic[FirestorePlayerAttributes.INTERNAL_ID];
        //     FireappAuthID = (String)dic[FirestorePlayerAttributes.FIREBASE_AUTH_ID];
        //     Auth = (AuthSource)(Int64)dic[FirestorePlayerAttributes.AUTH_TYPE];
        //     // LastLogin = dic[FirestorePlayerAttributes.AUTH_TYPE];
        //     EmailID = data.Id;
        //     Experience = (Double)dic[FirestorePlayerAttributes.EXPERIENCE];
        //     Level = (int)(Int64)dic[FirestorePlayerAttributes.LEVEL];

        //     // In case of parsing serverside timestamp:
        //     // (Timestamp) myTimestamp).ToDateTime().ToUniversalTime();
        //     foreach (KeyValuePair<string, object> pair in dic)
        //     {
        //         GameLog.LogAll(pair.Key + " " + pair.Value + " " + pair.Value.GetType());
        //     }
        // }
        // catch (SystemException e)
        // {
        //     Debug.Log("Exception " + e);

        // }
    }

    // Control times in which we save the game
    //Saves when the user closes the app
    //TODO: Saves every 10 minutes
    private async static void Quit()
    {
        //Task task = Firestore.SaveUserData(GetUserAsMap());
        Task task = Firestore.SaveObject(user);
        await task;
    }

    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        Application.quitting += Quit;
    }
}