using System;
using System.Collections.Generic;

namespace Util.Collections
{
    /**
     * Problem: Provide ordering for Pair values with integer weights.
     * Goal: Compare pairs by their integer value field.
     * Approach: Implement IComparer with value comparison.
     * Time: O(1) per compare.
     * Space: O(1).
     */
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
