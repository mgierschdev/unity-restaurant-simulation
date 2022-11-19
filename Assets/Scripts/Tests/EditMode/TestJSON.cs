using System;
using System.Collections.Generic;
using NUnit.Framework;

public class TestJSON
{
    [Test]
    public void TestSaveJSON()
    {
        DataGameUser user = PlayerData.GetNewUser();
        user.SaveToJSONFileAsync();

        string[] files = UtilJSONFile.GetSaveFiles();
        string json = UtilJSONFile.GetJsonFromFile(files[0]);
        GameLog.Log("Loading user from file " + files[0]);
        GameLog.Log("Content of the json file " + json);
        DataGameUser loadUser = DataGameUser.CreateFromJSON(json);
        //GameLog.Log("User created " + loadUser.NAME + " " + loadUser.INTERNAL_ID);
    }
}