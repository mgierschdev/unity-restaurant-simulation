using System;
using UnityEngine;
using NUnit.Framework;

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
    public void TestGetExpToLevel()
    {
        Assert.AreEqual(PlayerLevelCalculator.GetExpToLevel(0), 20);
        Assert.AreEqual(PlayerLevelCalculator.GetExpToLevel(-1), 0.0);
        Assert.AreEqual(PlayerLevelCalculator.GetExpToLevel(int.MinValue), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetExpToLevel(int.MaxValue), PlayerLevelCalculator.GetExpToLevel(100));
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(Double.MinValue), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(Double.MaxValue), 0); // since not feseable we return 0
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(103936), 1);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(343150), 10);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(342160), 1000);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(333160), 10000);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(-23123), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(0), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(857580), 94);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(32), 2);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(31), 1);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(22), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(247), 5);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(255), 6);
    }

    [Test]
    public void TestGetExperiencePercentage()
    {
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(4), 0.2f); // 20%
        GameLog.Log(((float)Math.Round(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(19), 2, MidpointRounding.AwayFromZero)).ToString());
        Assert.IsTrue(Mathf.Approximately(((float)Math.Round(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(19), 2, MidpointRounding.AwayFromZero)), 0.95f)); 
        Assert.IsTrue(Mathf.Approximately(((float)Math.Round(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(21), 2, MidpointRounding.AwayFromZero)), 0.91f));
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(double.MinValue), 0,1f); // 100%
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(-100), 0,1f); // 100%
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(double.MaxValue), 0);
    }
}