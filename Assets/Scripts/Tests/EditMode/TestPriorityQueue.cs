using NUnit.Framework;

public class TestPriorityQueue
{
    private PriorityQueue queue;

    [SetUp]
    public void Setup()
    {
        queue = new PriorityQueue();
    }

    [Test]
    public void TestAddElements()
    {
        queue.Enqueue(new PathNode(new int[] { 0, 1 }, 45));
        Assert.AreEqual(queue.GetSize(), 1);
    }

    [Test]
    public void TestGetSize()
    {
        queue.Enqueue(new PathNode(new int[] { 0, 1 }, 45));
        queue.Enqueue(new PathNode(new int[] { 0, 1 }, 45));
        queue.Enqueue(new PathNode(new int[] { 0, 1 }, 45));
        queue.Enqueue(new PathNode(new int[] { 0, 1 }, 45));
        queue.Enqueue(new PathNode(new int[] { 0, 1 }, 45));
        queue.Enqueue(new PathNode(new int[] { 0, 1 }, 45));
        Assert.AreEqual(queue.GetSize(), 6);
    }

    [Test]
    public void TestContains()
    {
        PathNode n = new PathNode(new int[] { 0, 1 }, 20);
        queue.Enqueue(n);
        PathNode m = new PathNode(new int[] { 0, 1 }, 20);
        queue.Enqueue(m);
        queue.Enqueue(new PathNode(new int[] { 0, 1 }, 45));
        queue.Enqueue(new PathNode(new int[] { 0, 1 }, 45));
        Assert.True(queue.Contains(n));
        Assert.True(queue.Contains(m));
        Assert.False(queue.Contains(new PathNode(new int[] { 0, 1 }, 45)));
    }

    [Test]
    public void TestPriority()
    {
        queue.Enqueue(new PathNode(new int[] { 0, 2 }, 35));
        queue.Enqueue(new PathNode(new int[] { 0, 3 }, 35));
        queue.Enqueue(new PathNode(new int[] { 0, 4 }, 3));
        queue.Enqueue(new PathNode(new int[] { 0, 5 }, 37));
        queue.Enqueue(new PathNode(new int[] { 0, 6 }, 55));
        queue.Enqueue(new PathNode(new int[] { 0, 7 }, 55));
        queue.Enqueue(new PathNode(new int[] { 0, 8 }, 75));

        PathNode node = queue.Dequeue();
        for (int i = 0; i < queue.GetSize() - 1; i++)
        {
            PathNode p = queue.Dequeue();
            Assert.GreaterOrEqual(p.GetFCost(), node.GetFCost());
            node = p;
        }
    }
}