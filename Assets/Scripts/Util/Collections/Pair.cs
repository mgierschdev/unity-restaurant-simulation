public class Pair<T, S>
{
    public T Key { get; set; }
    public S Value { get; set; }

    public Pair()
    {
    }

    public Pair(T Key, S Value)
    {
        this.Key = Key;
        this.Value = Value;
    }
}