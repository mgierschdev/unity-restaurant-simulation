namespace Util.PathFinding
{
    /**
     * Problem: Store queue node data for priority queues.
     * Goal: Hold pathfinding data and a priority value.
     * Approach: Keep data array and priority with next pointer.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public class QueueNode
    {
        public QueueNode Next;
    
        private readonly int[] _data;
        private readonly double _priority;

        public QueueNode(int[] data, double priority)
        {
            _data = data;
            _priority = priority;
        }

        public QueueNode()
        {

        }

        public int[] GetData()
        {
            return _data;
        }

        public double GetPriority()
        {
            return _priority;
        }
    }
}
