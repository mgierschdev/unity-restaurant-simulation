namespace Game.Players
{
    public class StoreGameObjectItem
    {
        public string Name { get; private set; }
        public string Identifier { get; private set; }
        public int Cost { get; private set; }

        public StoreGameObjectItem(string name, string identifier, int cost)
        {
            Name = name;
            Identifier = identifier;
            Cost = cost;
        }
    }
}