using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Game.Players.Model
{
    [Serializable] // The same as [System.Serializable]
    public class DataGameUser
    {
        [FormerlySerializedAs("VERSION")] [SerializeField]
        public string version = "1.0.3"; //most be changed on every new edit to the model

        [FormerlySerializedAs("NAME")] [SerializeField]
        public string name;

        [FormerlySerializedAs("GAME_MONEY")] [SerializeField]
        public Double gameMoney;

        [FormerlySerializedAs("EXPERIENCE")] [SerializeField]
        public Double experience;

        [FormerlySerializedAs("LEVEL")] [SerializeField]
        public int level;

        [FormerlySerializedAs("LANGUAGE_CODE")] [SerializeField]
        public string languageCode;

        [FormerlySerializedAs("INTERNAL_ID")] [SerializeField]
        public string internalID;

        [FormerlySerializedAs("EMAIL")] [SerializeField]
        public string email;

        [FormerlySerializedAs("AUTH_TYPE")] [SerializeField]
        public int authType;

        [FormerlySerializedAs("OBJECTS")] [SerializeField]
        public List<DataGameObject> objects;

        [FormerlySerializedAs("UPGRADES")] public List<UpgradeGameObject> upgrades;
        [FormerlySerializedAs("DATA_STATS")] public DataStatsGameObject dataStats;

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