using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using TMPro;
using UnityEngine;

[FirestoreData]
public static class PlayerData
{
    public static List<string> Name;
    public static String EmailID; // mandatory
    public static String InternalID; // internal app id
    public static String FireappAuthID = "undefined"; // Given by firebase
    public static String LanguageCode = "undefined"; // In the standard format (Locale) en_US, es_ES ...
    private static Double gameMoney;
    private static Double gems;
    private static Double experience;
    public static AuthSource Auth;
    [FirestoreProperty]
    public static object LastLogin { get; set; } // this is the default for the firestore server timestamp
    [FirestoreProperty]
    public static object SignInDate { get; set; } // this is the default for the firestore server timestamp
    private static TextMeshProUGUI moneyText;
    private static TextMeshProUGUI levelText;
    private static TextMeshProUGUI gemsText;
    private static List<GameGridObject> storedIventory;
    private static List<GameGridObject> Inventory;
    private static HashSet<string> setStoredInventory; // Saved stored inventory by ID

    public static void SetPlayerData(TextMeshProUGUI moneyText, TextMeshProUGUI levelText, TextMeshProUGUI gemsText) // Recieves the reference to the UI Text
    {
        PlayerData.moneyText = moneyText;
        PlayerData.gemsText = gemsText;
        PlayerData.levelText = levelText;

        moneyText.text = GetMoney();
        levelText.text = experience.ToString();
        gemsText.text = gems.ToString();

        Inventory = new List<GameGridObject>();
        storedIventory = new List<GameGridObject>();
        setStoredInventory = new HashSet<string>();
    }

    public static void AddExperienve(double amount)
    {
        experience += amount;
        levelText.text = experience.ToString();
    }

    public static void AddGems(double amount)
    {
        gems += amount;
        gemsText.text = gems.ToString();
    }

    public static void AddMoney(double amount)
    {
        gameMoney += amount;
        moneyText.text = GetMoney();
    }

    public static void Subtract(double amount)
    {
        if (!CanSubtract(amount))
        {
            return;
        }
        gameMoney -= amount;
        moneyText.text = GetMoney();
    }

    public static bool CanSubtract(double amount)
    {
        return gameMoney - amount >= 0;
    }

    public static string GetMoney()
    {
        return gameMoney + "$";
    }

    public static double GetMoneyDouble()
    {
        return gameMoney;
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

    public static void AddItemToInventory(GameGridObject obj)
    {
        Inventory.Add(obj);
    }

    private static string GenerateID()
    {
        return Guid.NewGuid().ToString();
    }

    public static void SetMockUser()
    {
        if (Name == null)
        {
            Name = new List<string>();
        }

        InternalID = GenerateID();
        EmailID = InternalID + "@gmail.com";
        Name.Add("FirstName");
        Name.Add("LastName");
        LanguageCode = "es_ES";
        Auth = AuthSource.GOOGLE_PLAY;
        FireappAuthID = "Test FireappAuthID";
        gameMoney = 20000;
        gems = 200;
        experience = 0;
    }

    public static Dictionary<string, object> GetUserAsMap()
    {
        return new Dictionary<string, object>{
            {FirestorePlayerAttributes.NAME, Name}, // This will append (to the existing fields even if they are the same) to the list if firestore is collec with merge
            {FirestorePlayerAttributes.LANGUAGE_CODE, LanguageCode},
            {FirestorePlayerAttributes.INTERNAL_ID, InternalID},
            {FirestorePlayerAttributes.GAME_MONEY, gameMoney},
            {FirestorePlayerAttributes.GEMS, gems},
            {FirestorePlayerAttributes.EXPERIENCE, experience},
            {FirestorePlayerAttributes.FIREBASE_AUTH_ID, FireappAuthID},
            {FirestorePlayerAttributes.AUTH_TYPE, Auth},
            {FirestorePlayerAttributes.LAST_LOGIN, FieldValue.ServerTimestamp},
            {FirestorePlayerAttributes.CREATED_AT, FieldValue.ServerTimestamp}
        };
    }

    public static void DebugPrint()
    {
        GameLog.Log(Name[0] + "," + Name[1] + "," + EmailID + "," + InternalID + "," + FireappAuthID + "," + Auth + "," + LanguageCode + "," + LastLogin + "," + SignInDate + "," + gameMoney);
    }

    public static void LoadFirebaseDocument(DocumentSnapshot data)
    {
        Dictionary<string, object> dic = data.ToDictionary();
        List<object> genericList = (List<object>)dic[FirestorePlayerAttributes.NAME];
        Name = new List<String>(){
            (String) genericList[0],
            (String) genericList[1],
        };

        gameMoney = (Double)dic[FirestorePlayerAttributes.GAME_MONEY];
        LanguageCode = (String)dic[FirestorePlayerAttributes.LANGUAGE_CODE];
        InternalID = (String)dic[FirestorePlayerAttributes.INTERNAL_ID];
        FireappAuthID = (String)dic[FirestorePlayerAttributes.FIREBASE_AUTH_ID];
        Auth = (AuthSource)(Int64)dic[FirestorePlayerAttributes.AUTH_TYPE];
        LastLogin = dic[FirestorePlayerAttributes.AUTH_TYPE];
        EmailID = data.Id;
        // In case of parsing serverside timestamp:
        // (Timestamp) myTimestamp).ToDateTime().ToUniversalTime();
        // foreach (KeyValuePair<string, object> pair in dic)
        // {
        //     GameLog.Log(pair.Key + " " + pair.Value + " " + pair.Value.GetType());
        // }
    }

    // Control times in which we save the game

    //Saves when the user closes the app
    //TODO: Saves every 10 minutes
    private async static void Quit()
    {
        Task task = Firestore.SaveUserData(GetUserAsMap());
        await task;
    }

    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        Application.quitting += Quit;
    }
}