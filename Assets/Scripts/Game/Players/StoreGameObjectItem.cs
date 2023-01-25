public class StoreGameObjectItem
{
    public string Name { get; private set; }
    public string Identifier { get; private set; }
    public int Cost { get; private set; }

    public StoreGameObjectItem(string Name, string Identifier, int Cost)
    {
        this.Name = Name;
        this.Identifier = Identifier;
        this.Cost = Cost;
    }
}