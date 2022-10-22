using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[FirestoreData]
public static class PlayerData
{
    public static List<string> Name;
    public static String EmailID; // mandatory
    public static String InternalID = "undefined"; // internal app id
    public static String FireappAuthID = "undefined"; // Given by firebase
    public static String LanguageCode = "undefined"; // In the standard format (Locale) en_US, es_ES ...
    public static int Level;
    public static Double GameMoney;
    public static Double Gems;
    public static Double Experience;
    public static AuthSource Auth;
    [FirestoreProperty]
    public static object LastLogin { get; set; } // this is the default for the firestore server timestamp
    [FirestoreProperty]
    public static object SignInDate { get; set; } // this is the default for the firestore server timestamp
    private static TextMeshProUGUI moneyText;
    private static TextMeshProUGUI levelText;
    private static TextMeshProUGUI gemsText;
    private static Slider expirienceSlider;
    private static List<GameGridObject> storedIventory;
    private static List<GameGridObject> Inventory;
    private static HashSet<string> setStoredInventory; // Saved stored inventory by ID

    // Recieves the reference to the UI Text
    public static void SetPlayerData(TextMeshProUGUI moneyText, TextMeshProUGUI levelText, TextMeshProUGUI gemsText, Slider expirienceSlider)
    {
        PlayerData.moneyText = moneyText;
        PlayerData.gemsText = gemsText;
        PlayerData.levelText = levelText;
        PlayerData.expirienceSlider = expirienceSlider;

        SetLevel();
        moneyText.text = GetMoney();
        gemsText.text = Gems.ToString();
        Inventory = new List<GameGridObject>();
        storedIventory = new List<GameGridObject>();
        setStoredInventory = new HashSet<string>();
    }

    public static void AddExperienve(double amount)
    {
        Experience += amount;
        SetLevel();
    }

    public static void AddGems(double amount)
    {
        Gems += amount;
        gemsText.text = Gems.ToString();
    }

    public static void AddMoney(double amount)
    {
        GameMoney += amount;
        moneyText.text = GetMoney();
    }

    public static void Subtract(double amount)
    {
        if (!CanSubtract(amount))
        {
            return;
        }
        GameMoney -= amount;
        AddExperienve(PlayerLevelCalculator.GetExperienceFromMoneySpent(amount));
        SetLevel();
        moneyText.text = GetMoney();
    }

    public static bool CanSubtract(double amount)
    {
        return GameMoney - amount >= 0;
    }

    public static void SetLevel()
    {
        int PrevLevel = Level;
        Level = PlayerLevelCalculator.GetLevel(Experience);
        if (PrevLevel < Level)
        {
            //TODO: Pop Up Level up
            //We save the data in case of app rewards
            //GameLog.Log("Setting player data " + GetUserAsMap().ToString());
            //Firestore.SaveUserData(GetUserAsMap());
        }
        levelText.text = GetLevel();
        expirienceSlider.value = PlayerLevelCalculator.GetExperienceToNextLevelPercentage(Experience) / 100f;
    }

    public static string GetMoney()
    {
        return GameMoney + "$";
    }

    public static string GetLevel()
    {
        return Level.ToString();
    }
    public static double GetMoneyDouble()
    {
        return GameMoney;
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
        GameMoney = 20000;
        Gems = 200;
        Experience = 2000;
        Level = PlayerLevelCalculator.GetLevel(Experience);
    }

    public static Dictionary<string, object> GetUserAsMap()
    {
        return new Dictionary<string, object>{
            {FirestorePlayerAttributes.NAME, Name}, // This will append (to the existing fields even if they are the same) to the list if firestore is collec with merge
            {FirestorePlayerAttributes.LANGUAGE_CODE, LanguageCode},
            {FirestorePlayerAttributes.INTERNAL_ID, InternalID},
            {FirestorePlayerAttributes.GAME_MONEY, GameMoney},
            {FirestorePlayerAttributes.GEMS, Gems},
            {FirestorePlayerAttributes.EXPERIENCE, Experience},
            {FirestorePlayerAttributes.LEVEL, Level},
            {FirestorePlayerAttributes.FIREBASE_AUTH_ID, FireappAuthID},
            {FirestorePlayerAttributes.AUTH_TYPE, Auth},
            {FirestorePlayerAttributes.LAST_LOGIN, FieldValue.ServerTimestamp}
            // {FirestorePlayerAttributes.CREATED_AT, FieldValue.ServerTimestamp} //Setted only th first time
        };
    }

    public static void DebugPrint()
    {
        GameLog.Log(Name[0] + "," + Name[1] + "," + EmailID + "," + InternalID + "," + FireappAuthID + "," + Auth + "," + LanguageCode + "," + LastLogin + "," + SignInDate + "," + GameMoney);
    }

    public static void LoadFirebaseDocument(DocumentSnapshot data)
    {
        Debug.Log("LoadFirebaseDocument");
        try
        {
            Dictionary<string, object> dic = data.ToDictionary();
            List<object> genericList = (List<object>)dic[FirestorePlayerAttributes.NAME];
            Name = new List<String>(){
            (String) genericList[0],
            (String) genericList[1],
        };

            GameMoney = (Double)dic[FirestorePlayerAttributes.GAME_MONEY];
            Gems = (Double)dic[FirestorePlayerAttributes.GEMS];
            LanguageCode = (String)dic[FirestorePlayerAttributes.LANGUAGE_CODE];
            InternalID = (String)dic[FirestorePlayerAttributes.INTERNAL_ID];
            FireappAuthID = (String)dic[FirestorePlayerAttributes.FIREBASE_AUTH_ID];
            Auth = (AuthSource)(Int64)dic[FirestorePlayerAttributes.AUTH_TYPE];
            LastLogin = dic[FirestorePlayerAttributes.AUTH_TYPE];
            EmailID = data.Id;
            Experience = (Double)dic[FirestorePlayerAttributes.EXPERIENCE];
            Level = (int)(Int64)dic[FirestorePlayerAttributes.LEVEL];

            // In case of parsing serverside timestamp:
            // (Timestamp) myTimestamp).ToDateTime().ToUniversalTime();
            foreach (KeyValuePair<string, object> pair in dic)
            {
                GameLog.LogAll(pair.Key + " " + pair.Value + " " + pair.Value.GetType());
            }
        }
        catch (SystemException e)
        {
            Debug.Log("Exception " + e);

        }
    }

    // Check if the user exists
    public async static void SetNewUser(FirebaseUser user)
    {
        SetMockUser();
        FireappAuthID = user.UserId;
        Auth = user.IsAnonymous == true ? AuthSource.ANONYMOUS : AuthSource.UNDEFINED;
        await Firestore.SaveUserData(GetUserAsMap());
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