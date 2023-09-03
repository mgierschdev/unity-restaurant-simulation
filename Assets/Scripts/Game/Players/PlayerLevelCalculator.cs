using System;
using System.Collections.Generic;

// calculates the player level based on the experience
// Also gives the amount of experience per money or gems spent
namespace Game.Players
{
    public static class PlayerLevelCalculator
    {
        private static List<Double> _expLevelMap;
        private const int MaxLevel = 100;

        private static void Init()
        {
            if (_expLevelMap != null)
            {
                return;
            }

            _expLevelMap = new List<Double>();

            for (int i = 0; i <= 100; i++)
            {
                _expLevelMap.Add(GetExpToLevel(i));
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
            return _expLevelMap[index] - experience;
        }

        // Returns an integer
        public static int GetExperienceToNextLevelPercentage(Double experience)
        {
            // Edge cases
            if (experience >= Double.MaxValue)
            {
                return 100;
            }

            if (experience < 0)
            {
                return 100;
            }

            Init();
            int index = CurrentLevel(experience);
            //base case
            if (index == 1)
            {
                return (int)(experience * 100 / _expLevelMap[1]);
            }

            if (index == 100)
            {
                return 0;
            }

            // General cases
            // total (100 next level - previous) ----- 100
            // current level - current exp
            double total = _expLevelMap[index] - _expLevelMap[index - 1]; //total required
            double current = experience - _expLevelMap[index - 1]; //current so far, inside level
            return (int)(current * 100 / total);
        }

        public static int GetLevel(Double experience)
        {
            if (experience < 0 || experience == 0)
            {
                return 0;
            }

            Init();
            return CurrentLevel(experience);
        }

        // Recieves a 0 indexed level
        public static Double GetExpToLevel(int level)
        {
            if (level < 0)
            {
                return 0;
            }

            if (level > MaxLevel)
            {
                level = MaxLevel;
            }

            return MathF.Pow(level, 3) + level * 2 + 20;
        }

        //returns the current level given the experience, starting at 1
        //could be improved into log(n) but 100 entries is already constant
        public static int CurrentLevel(Double exp)
        {
            Init();
            for (int i = 0; i < _expLevelMap.Count; i++)
            {
                if (_expLevelMap[i] > exp)
                {
                    return i == 0 ? 1 : i;
                }
            }

            return 100;
        }
    }
}