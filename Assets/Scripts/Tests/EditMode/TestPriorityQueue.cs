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
    public void TestQueueOperations(){
        queue.Enqueue(new int[]{0, 1}, 0.45f);
        Assert.AreEqual(queue.GetSize(), 1);
        queue.Enqueue(new int[]{0, 2}, 0.35f);
        queue.Enqueue(new int[]{0, 3}, 0.35f);
        queue.Enqueue(new int[]{0, 4}, 0.30f);
        queue.Enqueue(new int[]{0, 5}, 0.37f);
        queue.Enqueue(new int[]{0, 6}, 0.55f);
        queue.Enqueue(new int[]{0, 7}, 0.55f);
        queue.Enqueue(new int[]{0, 8}, 0.75f);
        Assert.AreEqual(queue.GetSize(), 8);

        QueueNode node = queue.Dequeue();
        for(int i = 0; i < queue.GetSize() - 1; i++){
            QueueNode p = queue.Dequeue();
            Assert.GreaterOrEqual(node.GetPriority(), p.GetPriority());
            node = p;
        }
    }
}