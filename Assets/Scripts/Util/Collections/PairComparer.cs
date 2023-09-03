using System;
using System.Collections.Generic;

namespace Util.Collections
{
    public class PairComparer<T> : IComparer<Pair<T, int>>
    {
        public int Compare(Pair<T, int> a, Pair<T, int> b)
        {
            _ = a ?? throw new ArgumentNullException(nameof(a));
            _ = b ?? throw new ArgumentNullException(nameof(b));
            
            return a.Value - b.Value;
        }
    }
}