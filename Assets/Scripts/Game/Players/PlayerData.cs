using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Players
{
    public class PlayerData
    {
        private double money;
        private TextMeshProUGUI moneyText;
        private List<GameGridObject> storedIventory;
        private List<GameGridObject> Inventory;
        private HashSet<string> setStoredInventory; // Saved stored inventory by ID

        public PlayerData(double money, TextMeshProUGUI moneyText)
        {
            this.money = money;
            this.moneyText = moneyText;
            moneyText.text = GetMoney();
            Inventory = new List<GameGridObject>();
            storedIventory = new List<GameGridObject>();
            setStoredInventory = new HashSet<string>();
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
            return money - amount >= 0;
        }

        public string GetMoney()
        {
            return money + "$";
        }

        public double GetMoneyDouble()
        {
            return money;
        }

        public void StoreItem(GameGridObject obj)
        {
            storedIventory.Add(obj);
            setStoredInventory.Add(obj.Name);
        }

        public bool IsItemStored(string nameID)
        {
            return setStoredInventory.Contains(nameID);
        }

        public void AddItemToInventory(GameGridObject obj)
        {
            Inventory.Add(obj);
        }
    }
}