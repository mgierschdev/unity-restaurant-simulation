using System;
using Game.Players;
using Game.Players.Model;
using NUnit.Framework;
using Util;

namespace Tests.EditMode
{
    public class TestJson
    {
        [Test]
        public void TestLoadUserJson()
        {
            var user = PlayerData.GetNewUser();
            user.SaveToJsonFileAsync();
            var json = UtilJsonFile.GetJsonFromFile(DataGameUser.GetSaveFileName());
            GameLog.Log(DataGameUser.GetSaveFileName());
            GameLog.Log("Content of the json file " + json);
            var loadUser = DataGameUser.CreateFromJson(json);

            if (loadUser == null)
            {
                GameLog.Log("load user null ");

                throw new ArgumentNullException(nameof(loadUser));
            }
            else
            {
                GameLog.Log("Name " + loadUser.name);
            }

            GameLog.Log(user.name + " " + loadUser.name);

            Assert.AreEqual(user.name, loadUser.name);
            Assert.AreEqual(user.authType, loadUser.authType);
            Assert.AreEqual(user.email, loadUser.email);
            Assert.AreEqual(user.experience, loadUser.experience);
            Assert.AreEqual(user.gameMoney, loadUser.gameMoney);
            Assert.AreEqual(user.languageCode, loadUser.languageCode);
            Assert.AreEqual(user.level, loadUser.level);

            for (int i = 0; i < user.objects.Count; i++)
            {
                Assert.AreEqual(user.objects[i].id, loadUser.objects[i].id);
                Assert.AreEqual(user.objects[i].isStored, loadUser.objects[i].isStored);
                Assert.AreEqual(user.objects[i].position, loadUser.objects[i].position);
                Assert.AreEqual(user.objects[i].rotation, loadUser.objects[i].rotation);
            }
        }

        [Test]
        // This saves the default user model e.g: default.v.1.0.0.json
        public void TestSaveDefaultUser()
        {
            DataGameUser user = PlayerData.GetNewUser();
            user.SaveToJsonFileAsync("default.user." + user.version);
        }
    }
}