namespace Util.Collections
{
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