using NUnit.Framework;

public class TestJSON
{
    [Test]
    public void TestSaveLoadUserJSON()
    {
        DataGameUser user = PlayerData.GetNewUser();
        user.SaveToJSONFileAsync();

        string[] files = UtilJSONFile.GetSaveFiles();
        string json = UtilJSONFile.GetJsonFromFile(files[0]);
        GameLog.Log("Loading user from file " + files[0]);
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

        Assert.Equals(user.NAME, loadUser.NAME);
        Assert.Equals(user.AUTH_TYPE, loadUser.AUTH_TYPE);
        Assert.Equals(user.CREATED_AT, loadUser.CREATED_AT);
        Assert.Equals(user.EMAIL, loadUser.EMAIL);
        Assert.Equals(user.EXPERIENCE, loadUser.EXPERIENCE);
        Assert.Equals(user.FIREBASE_AUTH_ID, loadUser.FIREBASE_AUTH_ID);
        Assert.Equals(user.GAME_MONEY, loadUser.GAME_MONEY);
        Assert.Equals(user.GEMS, loadUser.GEMS);
        Assert.Equals(user.GRID_SIZE, loadUser.GRID_SIZE);
        Assert.Equals(user.INTERNAL_ID, loadUser.INTERNAL_ID);
        Assert.Equals(user.LANGUAGE_CODE, loadUser.LANGUAGE_CODE);
        Assert.Equals(user.LAST_LOGIN, loadUser.LAST_LOGIN);
        Assert.Equals(user.LEVEL, loadUser.LEVEL);

        for (int i = 0; i < user.OBJECTS.Count; i++)
        {
            Assert.Equals(user.OBJECTS[i].ID, loadUser.OBJECTS[i].ID);
            Assert.Equals(user.OBJECTS[i].IS_STORED, loadUser.OBJECTS[i].IS_STORED);
            Assert.Equals(user.OBJECTS[i].POSITION, loadUser.OBJECTS[i].POSITION);
            Assert.Equals(user.OBJECTS[i].ROTATION, loadUser.OBJECTS[i].ROTATION);
        }
    }
}