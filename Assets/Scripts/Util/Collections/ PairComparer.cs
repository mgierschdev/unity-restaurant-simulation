using System.Collections.Generic;

public class PairComparer : IComparer<Pair<StoreGameObject, int>>
{
    public int Compare(Pair<StoreGameObject, int> a, Pair<StoreGameObject, int> b)
    {
        return a.Value - b.Value;
    }
}