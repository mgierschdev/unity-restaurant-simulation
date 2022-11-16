using System.Collections.Generic;

public class PairComparer<T> : IComparer<Pair<T, int>>
{
    public int Compare(Pair<T, int> a, Pair<T, int> b)
    {
        return a.Value - b.Value;
    }
}