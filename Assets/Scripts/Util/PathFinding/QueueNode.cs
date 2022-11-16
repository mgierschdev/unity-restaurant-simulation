public class QueueNode
{
    private int[] data;
    private double priority;
    public QueueNode next;

    public QueueNode(int[] data, double priority)
    {
        this.data = data;
        this.priority = priority;
    }

    public QueueNode()
    {

    }

    public int[] GetData()
    {
        return data;
    }

    public double GetPriority()
    {
        return priority;
    }
}