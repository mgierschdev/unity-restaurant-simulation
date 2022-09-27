public class StoreGameObject
{
    public string Name { get; set; }
    public string Identifier { get; set; }
    public int Cost { get; set; }
    public ObjectType Type { get; set; }
    public StoreItemType StoreItemType { get; set; }
    public string MenuItemSprite { get; set; }
    public string SpriteLibCategory { get; set; }

    public StoreGameObject(string name, string identifier, ObjectType type, StoreItemType tableType, string CategorySprite, int cost)
    {
        Identifier = identifier;
        Name = name;
        Cost = cost;
        Type = type;
        StoreItemType = tableType;
        MenuItemSprite = Settings.StoreSpritePath + identifier;
    }
}
