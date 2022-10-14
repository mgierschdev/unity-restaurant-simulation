using System;
using System.Collections.Generic;

// calculates the player level based on the experience
// Also gives the amount of experience per money or gems spent
public static class PlayerLevelCalculator
{
    private static List<Double> ExpLevelMap;
    private static int MAX_LEVEL = 100;

    private static void Init()
    {
        if (ExpLevelMap == null)
        {
            ExpLevelMap = new List<Double>();

            for (int i = 0; i <= 100; i++)
            {
                ExpLevelMap.Add(GetExpToLevel(i));//();
                GameLog.Log(ExpLevelMap[i].ToString() + " " + i);//prints current level
            }
        }
        else
        {
            return;
        }
    }

    public static Double GetExperienceFromGemsSpent(Double amount)
    {
        return amount is < 0 or >= (Double.MaxValue / 8) ? 0 : amount * 8;
    }

    public static Double GetExperienceFromMoneySpent(Double amount)
    {
        return amount is < 0 or >= (Double.MaxValue / 8) ? 0 : amount * 2;
    }

    public static Double GetExperienceToNextLevel(Double experience)
    {
        if (experience is < 0 or >= Double.MaxValue)
        {
            return 0;
        }

        Init();
        int index = Util.BinarySearch(ExpLevelMap, experience);
        return index < 0 ? ExpLevelMap[0] - experience : ExpLevelMap[index + 1] - experience;
    }

    public static float GetExperienceToNextLevelPercentage(Double experience)
    {
        if (experience >= Double.MaxValue)
        {
            return 0;
        }

        if (experience < 0)
        {
            return 0.1f;
        }

        Init();
        int index = Util.BinarySearch(ExpLevelMap, experience);
        // current exp * 100 / current level + 1
        GameLog.Log((((float)(experience * 100 / ExpLevelMap[index + 1])) / 100).ToString());
        return ((float)(experience * 100 / ExpLevelMap[index + 1])) / 100;
    }

    public static int GetLevel(Double experience)
    {
        if (experience < 0 || experience == 0)
        {
            return 0;
        }
        Init();
        int index = Util.BinarySearch(ExpLevelMap, experience);
        return index;
    }

    public static Double GetExpToLevel(int level)
    {
        if (level < 0)
        {
            return 0;
        }

        if (level > MAX_LEVEL)
        {
            level = MAX_LEVEL;
        }
        return MathF.Pow(level, 3) + level * 2 + 20;
    }
}