using Util.Collections;

namespace Util.PathFinding
{
    public class PathNode
    {
        public PathNode Next; // use to order in the queue
        private PathNode _parent; // Another reference comming from
        private int[] _position;
        private int _gCost; // G cost to the start point
        private int _hCost; // H cost to the end point
        private double _euclideanCost;
        private int _fCost; // value in the position, 0 free, 1 obstacle, 2 visited
        private int _value;

        public PathNode(int[] position)
        {
            _position = position;
            _parent = null;
        }

        public PathNode(int[] position, int fCost)
        {
            _position = position;
            _fCost = fCost;
            _parent = null;
        }

        public PathNode(int[] position, int costToStart, int costToEnd, PathNode parent)
        {
            _position = position;
            _gCost = costToStart;
            _hCost = costToEnd;
            _fCost = _gCost + _hCost;
            _parent = parent;
        }

        public string CoordsToString()
        {
            return "[" + _position[0] + "," + _position[1] + "]";
        }

        public override string ToString()
        {
            string tmp;
            if (_parent == null)
            {
                tmp = "null";
            }
            else
            {
                tmp = "[" + _parent.GetX() + "," + _parent.GetY() + "]";
            }

            return "[" + _position[0] + "," + _position[1] + "] Fcost: " + _fCost + " GCost: " + _gCost + " HCost: " + _hCost + " Parent: " + tmp;
        }

        public void CalculateFCost()
        {
            _fCost = _gCost + _hCost;
        }

        public int GetX()
        {
            return _position[0];
        }

        public int GetY()
        {
            return _position[1];
        }

        public int GetGCost()
        {
            return _gCost;
        }

        public int GetHCost()
        {
            return _hCost;
        }

        public double GetEuclideanCost()
        {
            return _euclideanCost;
        }

        public int[] GetPosition()
        {
            return _position;
        }

        public void SetParent(PathNode parent)
        {
            _parent = parent;
        }

        public PathNode GetParent()
        {
            return _parent;
        }

        // F Cost = G + H
        public int GetFCost()
        {
            return _fCost;
        }

        public int GetValue()
        {
            return _value;
        }

        public void SetValue(int value)
        {
            _value = value;
        }

        public void SetGCost(int costToStart)
        {
            _gCost = costToStart;
        }

        public void SetHCost(int costToEnd)
        {
            _hCost = costToEnd;
        }

        public void SetFCost(int total)
        {
            _fCost = total;
        }

        public void SetEuclideanCost(double euclideanCost)
        {
            _euclideanCost = euclideanCost;
        }

        public void SetPosition(int[] position)
        {
            _position = position;
        }

        public Node GetNode()
        {
            return new Node(_position[0], _position[1]);
        }
    }
}