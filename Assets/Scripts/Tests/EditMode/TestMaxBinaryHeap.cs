using NUnit.Framework;
using UnityEngine;

public class TestBinaryHeap
{
    private MaxBinaryHeap heap;

    [Test]
    public void TestInsertElements()
    {
        heap = new MaxBinaryHeap(100);
        heap.Add(new PathNode(new int[] { 0, 1 }, 45));
        heap.Add(new PathNode(new int[] { 0, 1 }, 5));
        heap.Add(new PathNode(new int[] { 0, 1 }, 44));
        heap.Add(new PathNode(new int[] { 0, 1 }, 4));
        heap.Add(new PathNode(new int[] { 0, 1 }, 35));
        Assert.AreEqual(heap.GetSize(), 5);
    }

    [Test]
    public void TestPeekElements()
    {
        heap = new MaxBinaryHeap(100);
        heap.Add(new PathNode(new int[] { 0, 1 }, 10));
        Assert.AreEqual(heap.Peek().GetFCost(), 10);
        heap.Add(new PathNode(new int[] { 0, 1 }, 20));
        Assert.AreEqual(heap.Peek().GetFCost(), 20);
        heap.Add(new PathNode(new int[] { 0, 1 }, 30));
        Assert.AreEqual(heap.Peek().GetFCost(), 30);
    }

    [Test]
    public void TestPeekExtractAndInsert()
    {
        heap = new MaxBinaryHeap(100);
        heap.Add(new PathNode(new int[] { 0, 1 }, 45));
        Assert.AreEqual(heap.GetSize(), 1);
        heap.Add(new PathNode(new int[] { 0, 1 }, 555));
        heap.Add(new PathNode(new int[] { 0, 1 }, 44));
        heap.Add(new PathNode(new int[] { 0, 1 }, 4));
        heap.Add(new PathNode(new int[] { 0, 1 }, 35));
        Assert.AreEqual(heap.Poll().GetFCost(), 555);
        Assert.AreEqual(heap.Poll().GetFCost(), 45);
        heap.Add(new PathNode(new int[] { 0, 1 }, 100));
        heap.Add(new PathNode(new int[] { 0, 1 }, -5));
        Assert.AreEqual(heap.Peek().GetFCost(), 100);
        heap.Add(new PathNode(new int[] { 0, 1 }, 100));
        heap.Add(new PathNode(new int[] { 0, 1 }, 10));
        heap.Add(new PathNode(new int[] { 0, 1 }, 104));
        heap.Add(new PathNode(new int[] { 0, 1 }, -10));

        PathNode node = heap.Poll();
        for (int i = 0; i < heap.GetSize(); i++)
        {
            PathNode p = heap.Poll();
            GameLog.Log(p.GetFCost() + " > " + node.GetFCost());
            Assert.GreaterOrEqual(node.GetFCost(), p.GetFCost());
            node = p;
        }
    }
}