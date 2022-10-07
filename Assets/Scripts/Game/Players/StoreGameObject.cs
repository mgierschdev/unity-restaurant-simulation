public class StoreGameObject
{
    public string Name { get; set; }
    public string Identifier { get; set; }
    public int Cost { get; set; }
    public ObjectType Type { get; set; }
    public StoreItemType StoreItemType { get; set; }
    public string MenuItemSprite { get; set; }
    public string SpriteLibCategory { get; set; }
    public string PrefabLocation { get; set; }
    public bool HasActionPoint {get; set;}

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
}