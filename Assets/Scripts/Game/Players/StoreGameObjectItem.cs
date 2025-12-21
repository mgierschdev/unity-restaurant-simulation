namespace Game.Players
{
    /**
     * Problem: Represent a sub-item for a store game object.
     * Goal: Store basic metadata for item variants and costs.
     * Approach: Keep immutable name/identifier/cost fields.
     * Time: O(1) per access.
     * Space: O(1).
     */
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
