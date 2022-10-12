using System;
using System.Collections.Generic;
using Firebase.Firestore;
using TMPro;

[FirestoreData]
public class PlayerData
{
    public string FirstName = "undefined"; // optional
    public string LastName = "undefined"; // optional
    public string EmailID; // mandatory
    public string InternalID; // internal app id
    public string FireappAuthID = "undefined";
    public string Auth = "undefined"; // TODO: Type of auth, we can serialized to an enum
    public string LanguageCode = "undefined"; // In the standard format (Locale) en_US, es_ES 
    [FirestoreProperty]
    public object LastLogin { get; set; } // this is the default for the firestore server timestamp
    [FirestoreProperty]
    public object SignInDate { get; set; } // this is the default for the firestore server timestamp

    private double money;
    private TextMeshProUGUI moneyText;
    private List<GameGridObject> storedIventory;
    private List<GameGridObject> Inventory;
    private HashSet<string> setStoredInventory; // Saved stored inventory by ID

    public PlayerData(double money, TextMeshProUGUI moneyText) // Recieves the reference to the UI Text
    {
        this.money = money;
        this.moneyText = moneyText;
        moneyText.text = GetMoney();
        Inventory = new List<GameGridObject>();
        storedIventory = new List<GameGridObject>();
        setStoredInventory = new HashSet<string>();
    }

    public PlayerData()
    {

    }

    public void AddMoney(double amount)
    {
        money += amount;
        moneyText.text = GetMoney();
    }

    public void Subtract(double amount)
    {
        if (!CanSubtract(amount))
        {
            return;
        }
        money -= amount;
        moneyText.text = GetMoney();
    }

    public bool CanSubtract(double amount)
    {
        return money - amount >= 0;
    }

    public string GetMoney()
    {
        return money + "$";
    }

    public double GetMoneyDouble()
    {
        return money;
    }

    public void StoreItem(GameGridObject obj)
    {
        storedIventory.Add(obj);
        setStoredInventory.Add(obj.Name);
    }

    public bool IsItemStored(string nameID)
    {
        return setStoredInventory.Contains(nameID);
    }

    public void AddItemToInventory(GameGridObject obj)
    {
        Inventory.Add(obj);
    }

    private string GenerateID()
    {
        return Guid.NewGuid().ToString();
    }


    public void SetMockUpUser()
    {
        InternalID = GenerateID();
        EmailID = InternalID + "@gmail.com";
        FirstName = "FirstName";
        LastName = "LastName";
        LanguageCode = "es_ES";
        Auth = "google play";
        FireappAuthID = "Test FireappAuthID";
    }

    public Dictionary<string, object> GetNewMockUserAsMap()
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