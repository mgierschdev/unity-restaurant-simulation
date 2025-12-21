using System;
using System.Collections.Generic;
using Util;

namespace Game.Players
{
    /**
     * Problem: Represent purchasable or placeable store items.
     * Goal: Store metadata for costs, prefabs, and upgrade info.
     * Approach: Encapsulate item data and comparison logic.
     * Time: O(1) per access; O(1) compare.
     * Space: O(1) per instance.
     */
    public class StoreGameObject : IEquatable<StoreGameObject>, IComparable<StoreGameObject>
    {
        public string Name { get; private set; }
        public string Identifier { get; private set; }
        public int Cost { get; }
        public ObjectType Type { get; private set; }
        public StoreItemType StoreItemType { get; private set; }
        public UpgradeType UpgradeType { get; private set; }
        public string MenuItemSprite { get; private set; }
        public string SpriteLibCategory { get; private set; }
        public string PrefabLocation { get; private set; }
        public bool HasActionPoint { get; private set; }
        public int MaxLevel { get; private set; }
        public List<StoreGameObjectItem> Items { get; }

        private readonly StoreGameObjectItem _currentSelected;

        public StoreGameObject(string name, string identifier, ObjectType type, StoreItemType tableType,
            string categorySprite, string prefabLocation, int cost, bool hasActionPoint)
        {
            Identifier = identifier;
            Name = name;
            Cost = cost;
            Type = type;
            StoreItemType = tableType;
            MenuItemSprite = Settings.StoreSpritePath + identifier;
            SpriteLibCategory = categorySprite;
            PrefabLocation = prefabLocation;
            HasActionPoint = hasActionPoint;
        }

        public StoreGameObject(string name, string identifier, ObjectType type, UpgradeType upgradeType,
            string categorySprite, string prefabLocation, int cost, bool hasActionPoint, int maxLevel)
        {
            Identifier = identifier;
            Name = name;
            Cost = cost;
            Type = type;
            MaxLevel = maxLevel;
            UpgradeType = upgradeType;
            MenuItemSprite = Settings.StoreSpritePath + identifier;
            SpriteLibCategory = categorySprite;
            PrefabLocation = prefabLocation;
            HasActionPoint = hasActionPoint;
            StoreItemType = StoreItemType.Undefined;
        }

        public StoreGameObject(string name, string identifier, ObjectType type, StoreItemType tableType,
            string categorySprite, string prefabLocation, int cost, bool hasActionPoint,
            List<StoreGameObjectItem> objects)
        {
            Identifier = identifier;
            Name = name;
            Cost = cost;
            Type = type;
            StoreItemType = tableType;
            MenuItemSprite = Settings.StoreSpritePath + identifier;
            SpriteLibCategory = categorySprite;
            PrefabLocation = prefabLocation;
            HasActionPoint = hasActionPoint;
            Items = objects;
            _currentSelected = objects[0];
        }

        public int GetIdentifierNumber()
        {
            var strNumber = Identifier.Split("-")[1];
            var value = int.Parse(strNumber);
            return value >= 0 ? value : 0;
        }

        // Default comparer for StoreGameObject cost type.
        public int CompareTo(StoreGameObject obj2)
        {
            // A null value means that this object is greater.
            if (obj2 == null)
            {
                return 1;
            }
            else
            {
                return Cost - obj2.Cost;
            }
        }

        public override int GetHashCode()
        {
            return Cost;
        }

        public bool Equals(StoreGameObject obj2)
        {
            if (obj2 == null)
            {
                return false;
            }

            return Cost == obj2.Cost;
        }

        public override string ToString()
        {
            return Identifier + "-" + Name + "-" + Cost + "-" + Type + "-" + StoreItemType + "-" + SpriteLibCategory +
                   "-" + MenuItemSprite + "-" + PrefabLocation + "-" + HasActionPoint;
        }

        public StoreGameObjectItem GetCurrentSelectedObject()
        {
            return _currentSelected;
        }
    }
}
