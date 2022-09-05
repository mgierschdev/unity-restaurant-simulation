using System.Runtime.InteropServices.WindowsRuntime;

namespace Game.Players
{
    public class PlayerData
    {
        private double money;

        public PlayerData(double money)
        {
            this.money = money;
        }

        public void AddMoney(double amount)
        {
            money += amount;
        }

        public void Subtract(double amount)
        {
            if (!CanSubtract(amount))
            {
                return;
            }
            money -= amount;
        }

        public bool CanSubtract(double amount)
        {
            return money - amount < 0;
        }

        public double GetMoney()
        {
            return money;
        }
    }
}