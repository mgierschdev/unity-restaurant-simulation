using System;
using System.Collections.Generic;
using UnityEngine;

// calculates the player level based on the experience
// Also gives the amount of experience per money or gems spent
public static class PlayerLevelCalculator
{
    private static List<Double> ExpLevelMap;
    private static int MAX_LEVEL = 100;

    private static void Init()
    {
        if (ExpLevelMap != null)
        {
            return;
        }

        ExpLevelMap = new List<Double>();

        for (int i = 0; i <= 100; i++)
        {
            ExpLevelMap.Add(GetExpToLevel(i));//();

            //Debug.Log(ExpLevelMap[i].ToString() + " " + (i + 1));//prints current level
            // if (i > 0)
            // {
            //     GameLog.Log("Level " + i + " " + (ExpLevelMap[i] - ExpLevelMap[i - 1]).ToString());
            // }
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

    // Returns the experience left for the next level, given the total experience so far
    public static Double GetExperienceToNextLevel(Double experience)
    {
        if (experience >= Double.MaxValue)
        {
            return 0;
        }

        if (experience < 0)
        {
            throw new Exception("Experience cannot be negative " + experience);
        }
        
        Init();
        int index = CurrentLevel(experience); // Returns indexed + 1
        return ExpLevelMap[index] - experience;
    }

    public static float GetExperienceToNextLevelPercentage(Double experience)
    {
        if (experience >= Double.MaxValue)
        {
            return 0.1f;
        }

        if (experience < 0)
        {
            return 0.1f;
        }

        Init();
        //Should return the current level 
        int index = CurrentLevel(experience);

        Debug.Log("Level " + index + " experience " + experience);

        // total (100 next level - previous) ----- 100
        // current level - current exp
        //base case 
        if (index == -1)
        {
            return ((float)(experience / 20));
        }


        double total = ExpLevelMap[index + 2] - ExpLevelMap[index + 1];
        double current = ExpLevelMap[index + 1] - experience;

        Debug.Log("Index " + index + " total " + total + " target " + current);
        return (float)(current / total);
    }

    public static int GetLevel(Double experience)
    {
        if (experience < 0 || experience == 0)
        {
            return 0;
        }
        Init();
        int index = CurrentLevel(experience);
        return index - 1;
    }

    // Recieves a 0 indexed level
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

    //returns the current level given the experience, starting at 1
    //could be improved into log(n) but 100 entries is already constant
    public static int CurrentLevel(Double exp)
    {
        Init();
        for (int i = 0; i < ExpLevelMap.Count; i++)
        {
            if (ExpLevelMap[i] > exp)
            {
                return i == 0 ? 1 : i;
            }
        }
        return -1;
    }
}