public class ObjectPair<S, T>
{
    public S val1 { get; set; }
    public T val2 { get; set; }

    public ObjectPair(S val1, T val2)
    {
        this.val1 = val1;
        this.val2 = val2;
    }
}