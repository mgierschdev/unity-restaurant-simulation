using System;
using System.Collections.Generic;
using Firebase.Firestore;
using TMPro;

[FirestoreData]
public static class PlayerData
{
    public static string FirstName = "undefined"; // optional
    public static string LastName = "undefined"; // optional
    public static string EmailID; // mandatory
    public static string InternalID; // internal app id
    public static string FireappAuthID = "undefined"; // Given by firebase
    public static AuthSource Auth;
    public static string LanguageCode = "undefined"; // In the standard format (Locale) en_US, es_ES ...
    [FirestoreProperty]
    public static object LastLogin { get; set; } // this is the default for the firestore server timestamp
    [FirestoreProperty]
    public static object SignInDate { get; set; } // this is the default for the firestore server timestamp
    private static double money;
    private static TextMeshProUGUI moneyText;
    private static List<GameGridObject> storedIventory;
    private static List<GameGridObject> Inventory;
    private static HashSet<string> setStoredInventory; // Saved stored inventory by ID

    public static void SetPlayerData(double mn, TextMeshProUGUI text) // Recieves the reference to the UI Text
    {
        money = mn;
        moneyText = text;
        text.text = GetMoney();
        Inventory = new List<GameGridObject>();
        storedIventory = new List<GameGridObject>();
        setStoredInventory = new HashSet<string>();
    }

    public static void AddMoney(double amount)
    {
        money += amount;
        moneyText.text = GetMoney();
    }

    public static void Subtract(double amount)
    {
        if (!CanSubtract(amount))
        {
            return;
        }
        money -= amount;
        moneyText.text = GetMoney();
    }

    public static bool CanSubtract(double amount)
    {
        return money - amount >= 0;
    }

    public static string GetMoney()
    {
        return money + "$";
    }

    public static double GetMoneyDouble()
    {
        return money;
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
    }

    public static Dictionary<string, object> GetNewMockUserAsMap()
    {
        return new Dictionary<string, object>{
            {"Name", new List<object>(){FirstName, LastName}},
            {"LanguageCode", LanguageCode},
            {"ID", InternalID},
            {"FireappAuthID", FireappAuthID},
            {"Auth", Auth},
            {"LastLogin", FieldValue.ServerTimestamp},
            {"CreatedAt", FieldValue.ServerTimestamp}
        };
    }
}