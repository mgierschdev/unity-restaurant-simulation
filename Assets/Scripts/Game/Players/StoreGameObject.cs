using System;

public class StoreGameObject : IEquatable<StoreGameObject>, IComparable<StoreGameObject>
{
    public string Name { get; set; }
    public string Identifier { get; set; }
    public int Cost { get; set; }
    public ObjectType Type { get; set; }
    public StoreItemType StoreItemType { get; set; }
    public UpgradeType UpgradeType { get; set; }
    public string MenuItemSprite { get; set; }
    public string SpriteLibCategory { get; set; }
    public string PrefabLocation { get; set; }
    public bool HasActionPoint { get; set; }
    public int MaxLevel { get; set; }

    public StoreGameObject(string name, string identifier, ObjectType type, StoreItemType tableType, string categorySprite, string prefabLocation, int cost, bool hasActionPoint)
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

    public StoreGameObject(string name, string identifier, ObjectType type, UpgradeType upgradeType, string categorySprite, string prefabLocation, int cost, bool hasActionPoint, int maxLevel)
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
        StoreItemType = StoreItemType.UNDEFINED;
    }

    public int GetIdentifierNumber()
    {
        string strNumber = Identifier.ToString().Split("-")[1];
        int value = Int32.Parse(strNumber);
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
        if (obj2 == null) { return false; }
        return Cost == obj2.Cost;
    }

    public override string ToString()
    {
        return Identifier + "-" + Name + "-" + Cost + "-" + Type + "-" + StoreItemType + "-" + SpriteLibCategory + "-" + MenuItemSprite + "-" + PrefabLocation + "-" + HasActionPoint;
    }
}