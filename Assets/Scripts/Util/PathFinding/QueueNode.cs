namespace Util.PathFinding
{
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