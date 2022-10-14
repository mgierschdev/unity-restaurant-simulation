using System;
using NUnit.Framework;
using UnityEngine;

public class TestExpLevelCalculator
{
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
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(103936), 1); // level 48 =103937, 1 exp to 48

        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(343150), 10);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(342160), 1000);
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevel(333160), 10000);

        Assert.AreEqual(PlayerLevelCalculator.GetLevel(-23123), 0);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(0), 0);

        Assert.AreEqual(PlayerLevelCalculator.GetLevel(22), 1);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(31), 2);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(32), 3);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(247), 6);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(255), 7);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(857584), 95);
        Assert.AreEqual(PlayerLevelCalculator.GetLevel(857585), 96);
    }

    [Test]
    public void TestGetExperiencePercentage()
    {
        //Base cases 0-22
        Debug.Log(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(19));
        Assert.IsTrue(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(19) == 82);
        Debug.Log(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(21));
        Assert.IsTrue(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(21) == 91);

        //Edge cases
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(double.MinValue), 100); // 100%
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(-100), 100); // 100%
        Assert.AreEqual(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(double.MaxValue), 100);

        //General cases
        Debug.Log(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(32851));
        Assert.IsTrue(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(32851) == 99); // 0.99f

        Debug.Log(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(53168));
        Assert.IsTrue(PlayerLevelCalculator.GetExperienceToNextLevelPercentage(53168) == 57); // 0.99f
    }
}

// temporal level list, formula likely to be adjusted
// 0-22 1
// 23-31 2
// 32-52 3
// 53 4
// 92 5
// 155 6
// 248 7
// 377 8
// 548 9
// 767 10
// 1040 11
// 1373 12
// 1772 13
// 2243 14
// 2792 15
// 3425 16
// 4148 17
// 4967 18
// 5888 19
// 6917 20
// 8060 21
// 9323 22
// 10712 23
// 12233 24
// 13892 25
// 15695 26
// 17648 27
// 19757 28
// 22028 29
// 24467 30
// 27080 31
// 29873 32
// 32852 33
// 36023 34
// 39392 35
// 42965 36
// 46748 37
// 50747 38
// 54968 39
// 59417 40
// 64100 41
// 69023 42
// 74192 43
// 79613 44
// 85292 45
// 91235 46
// 97448 47
// 103937 48
// 110708 49
// 117767 50
// 125120 51
// 132773 52
// 140732 53
// 149003 54
// 157592 55
// 166505 56
// 175748 57
// 185327 58
// 195248 59
// 205517 60
// 216140 61
// 227123 62
// 238472 63
// 250193 64
// 262292 65
// 274775 66
// 287648 67
// 300917 68
// 314588 69
// 328667 70
// 343160 71
// 358073 72
// 373412 73
// 389183 74
// 405392 75
// 422045 76
// 439148 77
// 456707 78
// 474728 79
// 493217 80
// 512180 81
// 531623 82
// 551552 83
// 571973 84
// 592892 85
// 614315 86
// 636248 87
// 658697 88
// 681668 89
// 705167 90
// 729200 91
// 753773 92
// 778892 93
// 804563 94
// 830792 95
// 857585 96
// 884948 97
// 912887 98
// 941408 99
// 970517 100
// 1000220 101