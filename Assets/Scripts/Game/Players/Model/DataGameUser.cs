using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Game.Players.Model
{
    [Serializable]
    public class DataGameUser
    {
        public string version = "1.0.4";
        
        public string name;
        
        public Double gameMoney;
        
        public Double experience;

        public int level;
        
        public string languageCode;

        public string internalID;

        public string email;
        
        public int authType;
        
        public List<DataGameObject> objects;
        
        public List<UpgradeGameObject> upgrades;
        
        public DataStatsGameObject dataStats;

        // Convert to JSON string
        public string ToJsonString()
        {
#if UNITY_EDITOR
            return JsonUtility.ToJson(this, true);
#else
        return JsonUtility.ToJson(this, false);
#endif
        }

        // Create object from JSON
        public static DataGameUser CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<DataGameUser>(jsonString);
        }

        // load from JSON data
        public void Load(string savedData)
        {
            JsonUtility.FromJsonOverwrite(savedData, this);
        }

        // Load object from json file inside the resource folder
        public void LoadFromJsonFile(string file)
        {
            TextAsset jsonTextFile = Resources.Load<TextAsset>(file);
            Load(jsonTextFile.text);
        }

        // Saves to a json file 
        private async void SaveToJsonFile(string filename)
        {
            filename = filename == "" ? GetSaveFileName() : GetSaveFileName(filename);

            await System.IO.File.WriteAllTextAsync(filename, ToJsonString());
        }

        public void SaveToJsonFileAsync(string filename)
        {
            SaveToJsonFile(filename);
        }

        public void SaveToJsonFileAsync()
        {
            SaveToJsonFile("");
        }

        public static string GetSaveFileName(string filename)
        {
            filename += ".json";
#if UNITY_EDITOR
            return Settings.DevSaveDirectory + "/" + filename;
#else
        return Application.persistentDataPath + "/" + filename;
#endif
        }

        public static string GetSaveFileName()
        {
#if UNITY_EDITOR
            return Settings.DevSaveDirectory + "/" + Settings.SaveFileSuffix;
#else
        return Application.persistentDataPath + "/" + Settings.SaveFileSuffix;
#endif
        }

        public void SetUpgrade(UpgradeType type, int value)
        {
            upgrades[(int)type].upgradeNumber = value;
        }

        public void IncreaseUpgrade(UpgradeType type)
        {
            upgrades[(int)type].upgradeNumber++;
        }

        public int GetUpgrade(UpgradeType type)
        {
            return upgrades[(int)type].upgradeNumber;
        }
    }
}