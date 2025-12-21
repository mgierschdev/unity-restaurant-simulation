namespace Util.Collections
{
    /**
     * Problem: Store a simple key/value pair.
     * Goal: Provide a lightweight generic container.
     * Approach: Expose Key and Value properties.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public class Pair<T, TS>
    {
        public T Key { get; set; }
        public TS Value { get; set; }

        public Pair()
        {
        }

        public Pair(T key, TS value)
        {
            Key = key;
            Value = value;
        }
    }
}
