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
        queue.Add(new PathNode(new int[] { 0, 1 }, 45));
        Assert.AreEqual(queue.GetSize(), 1);
    }

    [Test]
    public void TestGetSize()
    {
        queue.Add(new PathNode(new int[] { 0, 1 }, 45));
        queue.Add(new PathNode(new int[] { 0, 1 }, 45));
        queue.Add(new PathNode(new int[] { 0, 1 }, 45));
        queue.Add(new PathNode(new int[] { 0, 1 }, 45));
        queue.Add(new PathNode(new int[] { 0, 1 }, 45));
        queue.Add(new PathNode(new int[] { 0, 1 }, 45));
        Assert.AreEqual(queue.GetSize(), 6);
    }

    [Test]
    public void TestContains()
    {
        PathNode n = new PathNode(new int[] { 0, 1 }, 20);
        queue.Add(n);
        PathNode m = new PathNode(new int[] { 0, 1 }, 20);
        queue.Add(m);
        queue.Add(new PathNode(new int[] { 0, 1 }, 45));
        queue.Add(new PathNode(new int[] { 0, 1 }, 45));
        Assert.True(queue.Contains(n));
        Assert.True(queue.Contains(m));
        Assert.False(queue.Contains(new PathNode(new int[] { 0, 1 }, 45)));
    }

    [Test]
    public void TestPriority()
    {
        queue.Add(new PathNode(new int[] { 0, 2 }, 35));
        queue.Add(new PathNode(new int[] { 0, 3 }, 35));
        queue.Add(new PathNode(new int[] { 0, 4 }, 3));
        queue.Add(new PathNode(new int[] { 0, 5 }, 37));
        queue.Add(new PathNode(new int[] { 0, 6 }, 55));
        queue.Add(new PathNode(new int[] { 0, 7 }, 55));
        queue.Add(new PathNode(new int[] { 0, 8 }, 75));

        PathNode node = queue.Poll();
        for (int i = 0; i < queue.GetSize() - 1; i++)
        {
            PathNode p = queue.Poll();
            Assert.GreaterOrEqual(p.GetFCost(), node.GetFCost());
            node = p;
        }
    }

    [Test]
    public void TestDequeue()
    {
        queue.Add(new PathNode(new int[] { 0, 2 }, 35));
        queue.Add(new PathNode(new int[] { 0, 3 }, 35));
        queue.Add(new PathNode(new int[] { 0, 4 }, 3));
        Assert.AreEqual(queue.Poll().GetFCost(), 3);
    }
}