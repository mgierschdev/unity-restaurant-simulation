// Similar to priority Queue with Log(n) insertion
// Only requirement is the FCost setted for prioritizing

using Util.Collections;

namespace Util.PathFinding
{
    /**
     * Problem: Provide a max-heap for pathfinding nodes.
     * Goal: Retrieve the highest-cost PathNode efficiently.
     * Approach: Maintain a binary heap array with heapify operations.
     * Time: O(log n) for insert/remove.
     * Space: O(n) for heap storage.
     */
    public class MaxBinaryHeap : IBaseGameCollections
    {
        private readonly PathNode[] _nodes;
        private int _currentHeapSize;

        public MaxBinaryHeap(int heapSize)
        {
            _nodes = new PathNode[heapSize];
            _currentHeapSize = 0;
        }

        public void Add(PathNode node)
        {
            if (_currentHeapSize < 0)
            {
                GameLog.LogWarning("Current heap size is negative");
            }
            else
            {
                _nodes[_currentHeapSize] = node;
                _currentHeapSize++;
                HeapUp();
            }
        }

        private void HeapUp()
        {
            int index = _currentHeapSize - 1;
            while (index >= 0 && GetParent(index).GetFCost() < _nodes[index].GetFCost())
            {
                int parentIndex = GetParentIndex(index);
                Swap(parentIndex, index);
                index = parentIndex;
            }
        }

        public PathNode Poll()
        {
            if (_currentHeapSize == 0)
            {
                GameLog.LogWarning("Cannot extract from an empty heap");
                return null;
            }
            else
            {
                PathNode head = _nodes[0];
                _nodes[0] = _nodes[_currentHeapSize - 1];
                _currentHeapSize--;
                HeapDown();
                return head;
            }
        }

        private void HeapDown()
        {
            int index = 0;

            while (HasLeftChild(index))
            {
                int bigger = GetLeftChildIndex(index);

                if (HashRightChild(index) && GetRightChild(index).GetFCost() > GetLeftChild(index).GetFCost())
                {
                    bigger = GetRightChildIndex(index);
                }

                if (_nodes[bigger].GetFCost() < _nodes[index].GetFCost())
                {
                    break;
                }

                Swap(bigger, index);
                index = bigger;
            }
        }

        public PathNode Peek()
        {
            if (_currentHeapSize == 0)
            {
                return null;
            }
            else
            {
                return _nodes[0];
            }
        }

        private int GetLeftChildIndex(int index)
        {
            return 2 * index + 1;
        }

        private int GetRightChildIndex(int index)
        {
            return 2 * index + 2;
        }

        private int GetParentIndex(int index)
        {
            return (index - 1) / 2;
        }

        private bool HasLeftChild(int index)
        {
            return GetLeftChildIndex(index) < _currentHeapSize;
        }

        private bool HashRightChild(int index)
        {
            return GetRightChildIndex(index) < _currentHeapSize;
        }

        private PathNode GetLeftChild(int index)
        {
            return _nodes[GetLeftChildIndex(index)];
        }

        private PathNode GetRightChild(int index)
        {
            return _nodes[GetRightChildIndex(index)];
        }

        private PathNode GetParent(int index)
        {
            return _nodes[GetParentIndex(index)];
        }

        private void PrintHeap()
        {
            var s = "";
        
            for (var i = 0; i < _currentHeapSize; i++)
            {
                s += _nodes[i].GetFCost() + " ";
            }
        
            GameLog.Log(s);
        }

        private void Swap(int i, int j)
        {
            (_nodes[i], _nodes[j]) = (_nodes[j], _nodes[i]);
        }

        public int GetSize()
        {
            return _currentHeapSize;
        }

        public bool IsEmpty()
        {
            return _currentHeapSize == 0;
        }
    }
}
