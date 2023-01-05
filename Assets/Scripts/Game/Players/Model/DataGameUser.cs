using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable] // The same as [System.Serializable]
public class DataGameUser
{
    [SerializeField]
    public string VERSION = "1.0.1"; //most be changed on every new edit to the model
    [SerializeField]
    public string NAME;
    [SerializeField]
    public Double GAME_MONEY;
    [SerializeField]
    public Double GEMS;
    [SerializeField]
    public Double EXPERIENCE;
    [SerializeField]
    public int LEVEL;
    [SerializeField]
    public string LANGUAGE_CODE;
    [SerializeField]
    public string INTERNAL_ID;
    [SerializeField]
    public string EMAIL;
    [SerializeField]
    public int AUTH_TYPE;
    [SerializeField]
    public long LAST_SAVE;
    [SerializeField]
    public long CREATED_AT;
    [SerializeField]
    public List<DataGameObject> OBJECTS;
    public List<UpgradeGameObject> UPGRADES;
    public DataStatsGameObject DATA_STATS;

    // Convert to JSON string
    public string ToJSONString(Boolean pretty)
    {
        return JsonUtility.ToJson(this, pretty);
    }

    // Create object from JSON
    public static DataGameUser CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<DataGameUser>(jsonString);
    }

    // load from JSON data
    public void Load(string savedData)
    {
        JsonUtility.FromJsonOverwrite(savedData, this);
    }

    // Load object from json file inside the resource folder
    public void LoadFromJSONFile(string file)
    {
        TextAsset jsonTextFile = Resources.Load<TextAsset>(file);
        Load(jsonTextFile.text);
    }

    // Saves to a json file 
    private async void SaveToJSONFile(string filename)
    {
        filename = filename == "" ? GetSaveFileName() : GetSaveFileName(filename);

        await System.IO.File.WriteAllTextAsync(filename, ToJSONString(Settings.devEnv));
    }

    public void SaveToJSONFileAsync(string filename)
    {
        SaveToJSONFile(filename);
    }

    public void SaveToJSONFileAsync()
    {
        SaveToJSONFile("");
    }

    public static string GetSaveFileName(string filename)
    {
        filename += ".json";
        return Settings.devEnv ? Settings.DevSaveDirectory + "/" + filename : Application.persistentDataPath + "/" + filename;
    }

    public static string GetSaveFileName()
    {
        return Settings.devEnv ? Settings.DevSaveDirectory + "/" + Settings.SaveFileSuffix : Application.persistentDataPath + "/" + Settings.SaveFileSuffix;
    }

    public void SetUpgrade(UpgradeType type, int value)
    {
        UPGRADES[(int)type].UPGRADE_NUMBER = value;
    }

    public int GetUpgrade(UpgradeType type)
    {
        return UPGRADES[(int)type].UPGRADE_NUMBER;
    }
}