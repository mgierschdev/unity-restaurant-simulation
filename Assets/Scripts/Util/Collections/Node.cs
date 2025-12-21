using UnityEngine;

namespace Util.Collections
{
    /**
     * Problem: Represent a 2D grid coordinate.
     * Goal: Provide simple conversions and comparisons for nodes.
     * Approach: Store x/y values and expose helper methods.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public class Node
    {
        private readonly int _x;
        private readonly int _y;

        public Node(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public Node(int[] a)
        {
            _x = a[0];
            _y = a[1];
        }

        public override string ToString()
        {
            return "[" + _x + "," + _y + "]";
        }

        public bool Compare(Node n)
        {
            return _x == n.GetX() && _y == n.GetY();
        }

        public Vector3 GetVector3()
        {
            return new Vector3(_x, _y);
        }

        public Vector3Int GetVector3Int()
        {
            return new Vector3Int(_x, _y);
        }
    
        private int GetX()
        {
            return _x;
        }

        private int GetY()
        {
            return _y;
        }
    }
}
