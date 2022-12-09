using System;
using NUnit.Framework;

public class TestJSON
{

    [Test]
    public void TestSaveUserJSON()
    {
        DataGameUser user = PlayerData.GetNewUser();

        GameLog.Log("Datetime " + user.LAST_SAVE);
        user.SaveToJSONFileAsync();
    }

    [Test]
    public void TestLoadUserJSON()
    {
        DataGameUser user = PlayerData.GetNewUser();
        string json = UtilJSONFile.GetJsonFromFile(DataGameUser.GetSaveFileName());
        GameLog.Log(DataGameUser.GetSaveFileName());
        GameLog.Log("Content of the json file " + json);
        DataGameUser loadUser = DataGameUser.CreateFromJSON(json);

        if (loadUser == null)
        {
            GameLog.Log("load user null ");
        }
        else
        {
            GameLog.Log("Name " + loadUser.NAME);
        }

        GameLog.Log("loadUser.LAST_LOGIN " + new DateTime(loadUser.LAST_SAVE));
        GameLog.Log("user.LAST_LOGIN " + new DateTime(user.LAST_SAVE));
        GameLog.Log(user.NAME + " " + loadUser.NAME);

        Assert.AreEqual(user.NAME, loadUser.NAME);
        Assert.AreEqual(user.AUTH_TYPE, loadUser.AUTH_TYPE);
        Assert.AreEqual(user.EMAIL, loadUser.EMAIL);
        Assert.AreEqual(user.EXPERIENCE, loadUser.EXPERIENCE);
        Assert.AreEqual(user.GAME_MONEY, loadUser.GAME_MONEY);
        Assert.AreEqual(user.GEMS, loadUser.GEMS);
        Assert.AreEqual(user.GRID_SIZE, loadUser.GRID_SIZE);
        Assert.AreEqual(user.LANGUAGE_CODE, loadUser.LANGUAGE_CODE);
        Assert.AreEqual(user.LEVEL, loadUser.LEVEL);

        for (int i = 0; i < user.OBJECTS.Count; i++)
        {
            Assert.AreEqual(user.OBJECTS[i].ID, loadUser.OBJECTS[i].ID);
            Assert.AreEqual(user.OBJECTS[i].IS_STORED, loadUser.OBJECTS[i].IS_STORED);
            Assert.AreEqual(user.OBJECTS[i].POSITION, loadUser.OBJECTS[i].POSITION);
            Assert.AreEqual(user.OBJECTS[i].ROTATION, loadUser.OBJECTS[i].ROTATION);
        }
    }
}