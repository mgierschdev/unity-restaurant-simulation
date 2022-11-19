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

        string[] files = UtilSaveFile.GetSaveFiles();
        GameLog.Log("Loading user from file " + files[0]);
        DataGameUser loadUser = DataGameUser.CreateFromJSON(files[0]);
        GameLog.Log("User created " + loadUser.NAME + " " + loadUser.INTERNAL_ID);

        
    }
}