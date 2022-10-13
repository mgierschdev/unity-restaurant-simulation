using System;
using System.Collections.Generic;
using Firebase.Firestore;
using TMPro;
using UnityEngine;

[FirestoreData]
public static class PlayerData
{
    public static string FirstName = "undefined"; // optional
    public static string LastName = "undefined"; // optional
    public static string EmailID; // mandatory
    public static string InternalID; // internal app id
    public static string FireappAuthID = "undefined"; // Given by firebase
    public static string LanguageCode = "undefined"; // In the standard format (Locale) en_US, es_ES ...
    private static double gameMoney;
    public static AuthSource Auth;
    [FirestoreProperty]
    public static object LastLogin { get; set; } // this is the default for the firestore server timestamp
    [FirestoreProperty]
    public static object SignInDate { get; set; } // this is the default for the firestore server timestamp
    private static TextMeshProUGUI moneyText;
    private static List<GameGridObject> storedIventory;
    private static List<GameGridObject> Inventory;
    private static HashSet<string> setStoredInventory; // Saved stored inventory by ID

    public static void SetPlayerData(TextMeshProUGUI text) // Recieves the reference to the UI Text
    {
        moneyText = text;
        text.text = GetMoney();
        Inventory = new List<GameGridObject>();
        storedIventory = new List<GameGridObject>();
        setStoredInventory = new HashSet<string>();
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

    public static void SetMockUpUser()
    {
        InternalID = GenerateID();
        EmailID = InternalID + "@gmail.com";
        FirstName = "FirstName";
        LastName = "LastName";
        LanguageCode = "es_ES";
        Auth = AuthSource.GOOGLE_PLAY;
        FireappAuthID = "Test FireappAuthID";
        gameMoney = 20000;
    }

    public static Dictionary<string, object> GetNewMockUserAsMap()
    {
        return new Dictionary<string, object>{
            {FirestorePlayerAttributes.NAME, new List<object>(){FirstName, LastName}},
            {FirestorePlayerAttributes.LANGUAGE_CODE, LanguageCode},
            {FirestorePlayerAttributes.INTERNAL_ID, InternalID},
            {FirestorePlayerAttributes.GAME_MONEY, gameMoney},
            {FirestorePlayerAttributes.FIREBASE_AUTH_ID, FireappAuthID},
            {FirestorePlayerAttributes.AUTH_TYPE, Auth},
            {FirestorePlayerAttributes.LAST_LOGIN, FieldValue.ServerTimestamp},
            {FirestorePlayerAttributes.CREATED_AT, FieldValue.ServerTimestamp}
        };
    }

    public static void DebugPrint()
    {
        GameLog.Log(FirstName + "," + LastName + "," + EmailID + "," + InternalID + "," + FireappAuthID + "," + Auth + "," + LanguageCode + "," + LastLogin + "," + SignInDate + "," + gameMoney);
    }

    public static void LoadFirebaseDocument(DocumentSnapshot data)
    {
        // KeyValuePair<string, object> pair 
        Dictionary<string, object> dic = data["data"];
    }
}