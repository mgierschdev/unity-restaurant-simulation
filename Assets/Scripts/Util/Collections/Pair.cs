public class Pair<T, S>
{
    public T key { get; set; }
    public S value { get; set; }

    public Pair()
    {
    }
    
    public Pair(T key, S value)
    {
        this.key = key;
        this.value = value;
    }
}