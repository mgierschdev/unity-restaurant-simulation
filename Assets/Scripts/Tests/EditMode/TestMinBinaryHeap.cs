using NUnit.Framework;
using Util;
using Util.PathFinding;

namespace Tests.EditMode
{
    /**
     * Problem: Validate MinBinaryHeap ordering behavior.
     * Goal: Ensure heap returns smallest-cost nodes first.
     * Approach: Insert nodes and assert poll/peek results.
     * Time: O(n log n) per test sequence.
     * Space: O(n) for heap contents.
     */
    public class TestMinBinaryHeap
    {
        private MinBinaryHeap _heap;

        [Test]
        public void TestInsertElements()
        {
            _heap = new MinBinaryHeap(100);
            _heap.Add(new PathNode(new[] { 0, 1 }, 45));
            _heap.Add(new PathNode(new[] { 0, 1 }, 5));
            _heap.Add(new PathNode(new[] { 0, 1 }, 44));
            _heap.Add(new PathNode(new[] { 0, 1 }, 4));
            _heap.Add(new PathNode(new[] { 0, 1 }, 35));
            Assert.AreEqual(_heap.GetSize(), 5);
        }

        [Test]
        public void TestPeekElements()
        {
            _heap = new MinBinaryHeap(100);
            _heap.Add(new PathNode(new[] { 0, 1 }, 10));
            Assert.AreEqual(_heap.Peek().GetFCost(), 10);
            _heap.Add(new PathNode(new[] { 0, 1 }, 20));
            Assert.AreEqual(_heap.Peek().GetFCost(), 10);
            _heap.Add(new PathNode(new[] { 0, 1 }, 30));
            Assert.AreEqual(_heap.Peek().GetFCost(), 10);
        }

        [Test]
        public void TestPeekExtractAndInsert()
        {
            _heap = new MinBinaryHeap(100);
            _heap.Add(new PathNode(new[] { 0, 1 }, 45));
            Assert.AreEqual(_heap.GetSize(), 1);
            _heap.Add(new PathNode(new[] { 0, 1 }, 555));
            _heap.Add(new PathNode(new[] { 0, 1 }, 44));
            _heap.Add(new PathNode(new[] { 0, 1 }, 4));
            _heap.Add(new PathNode(new[] { 0, 1 }, 35));
            Assert.AreEqual(_heap.Poll().GetFCost(), 4);
            Assert.AreEqual(_heap.Poll().GetFCost(), 35);
            _heap.Add(new PathNode(new[] { 0, 1 }, 100));
            _heap.Add(new PathNode(new[] { 0, 1 }, -5));
            Assert.AreEqual(_heap.Peek().GetFCost(), -5);
            _heap.Add(new PathNode(new[] { 0, 1 }, 100));
            _heap.Add(new PathNode(new[] { 0, 1 }, 10));
            _heap.Add(new PathNode(new[] { 0, 1 }, 104));
            _heap.Add(new PathNode(new[] { 0, 1 }, -10));

            PathNode node = _heap.Poll();
            for (int i = 0; i < _heap.GetSize(); i++)
            {
                PathNode p = _heap.Poll();
                GameLog.Log(p.GetFCost() + " > " + node.GetFCost());
                Assert.GreaterOrEqual(p.GetFCost(), node.GetFCost());
                node = p;
            }
        }
    }
}
