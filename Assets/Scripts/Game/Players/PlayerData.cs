using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;

namespace Game.Players
{
    public class PlayerData
    {
        private double money;
        private TextMeshProUGUI moneyText;

        public PlayerData(double money, TextMeshProUGUI moneyText)
        {
            this.money = money;
            this.moneyText = moneyText;
            moneyText.text = GetMoney();
        }

        public void AddMoney(double amount)
        {
            money += amount;
            moneyText.text = GetMoney();
        }

        public void Subtract(double amount)
        {
            if (!CanSubtract(amount))
            {
                return;
            }
            money -= amount;
            moneyText.text = GetMoney();
        }

        public bool CanSubtract(double amount)
        {
            return money - amount < 0;
        }

        public string GetMoney()
        {
            return money+"$";
        }
    }
}