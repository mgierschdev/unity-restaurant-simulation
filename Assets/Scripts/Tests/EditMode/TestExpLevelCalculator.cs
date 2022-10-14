using System;
using NUnit.Framework;
using UnityEngine;

public class TestExpLevelCalculator
{
    [Test]
    public void TestGetExperienceFromGems()
    {
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromGemsSpent(0), 0.0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromGemsSpent(-1), 0.0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromGemsSpent(Double.MinValue), 0.0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromGemsSpent(Double.MaxValue), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromGemsSpent(Double.MaxValue / 8), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromGemsSpent(Double.MaxValue / 8), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromGemsSpent(1234124), 1234124 * 8);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromGemsSpent(2), 2 * 8);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromGemsSpent(1232), 1232 * 8);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromGemsSpent(33), 33 * 8);
    }

    [Test]
    public void TestGetExperienceFromMoney()
    {
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromMoneySpent(0), 0.0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromMoneySpent(-1), 0.0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromMoneySpent(Double.MinValue), 0.0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromMoneySpent(Double.MaxValue), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromMoneySpent(Double.MaxValue / 2), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromMoneySpent(Double.MaxValue / 2), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromMoneySpent(1234124), 1234124 * 2);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromMoneySpent(2), 2 * 2);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromMoneySpent(1232), 1232 * 2);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceFromMoneySpent(33), 33 * 2);
    }

    [Test]
    public void TestGetExpToNextLevel()
    {
        Assert.AreEqual(PlayerLevelCalculator.GetExpToLevel(0), 20);
        Assert.AreEqual(PlayerLevelCalculator.GetExpToLevel(int.MaxValue), PlayerLevelCalculator.GetExpToLevel(100));

        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(Double.MaxValue), 0); // since not feseable we return 0
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(103936), 1); // level 47
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(343150), 10);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(342160), 1000);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(333160), 10000);

        Assert.AreEqual(PlayerLevelCalculator.GetLevel(-23123), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(0), 0);

        Assert.AreEqual(PlayerLevelCalculator.GetLevel(22), 1);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(31), 2);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(32), 3);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(247), 7);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(255), 7);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(857584), 95);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(857585), 96);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(857586), 97);
    }

    [Test]
    public void TestGetExperiencePercentage()
    {
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(4), 0.2f); // 20%
        Assert.IsTrue(Mathf.Approximately(((float)Math.Round(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(19), 2, MidpointRounding.AwayFromZero)), 0.95f));
        Assert.IsTrue(Mathf.Approximately(((float)Math.Round(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(21), 2, MidpointRounding.AwayFromZero)), 0.91f));
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(double.MinValue), 0, 1f); // 100%
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(-100), 0, 1f); // 100%
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(double.MaxValue), 0.1f);
    }

    [Test]
    public void TestGetCurrentLevel()
    {
        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(-1), 1);
        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(0), 1);
        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(1), 1);
        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(20), 1);
        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(21), 1);
        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(22), 1);

        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(23), 2);
        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(30), 2);
        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(31), 2);

        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(32), 3);
        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(33), 3);
        Assert.AreEqual(PlayerLevelCalculator.CurrentLevel(34), 3);

        // for (int i = 0; i < 100000; i++)
        // {
        //     Debug.Log(i + " " + PlayerLevelCalculator.CurrentLevel(i));
        // }
    }
}