using NUnit.Framework;
using Util.PathFinding;

namespace Tests.EditMode
{
    /**
     * Problem: Validate PriorityQueue ordering and behavior.
     * Goal: Ensure nodes are dequeued by ascending F-cost.
     * Approach: Insert nodes and assert poll/contains results.
     * Time: O(n) per insert sequence (due to list insertion).
     * Space: O(n) for queue contents.
     */
    public class TestPriorityQueue
    {
        private PriorityQueue _queue;

        [SetUp]
        public void Setup()
        {
            _queue = new PriorityQueue();
        }

        [Test]
        public void TestAddElements()
        {
            _queue.Add(new PathNode(new[] { 0, 1 }, 45));
            Assert.AreEqual(_queue.GetSize(), 1);
        }

        [Test]
        public void TestGetSize()
        {
            _queue.Add(new PathNode(new[] { 0, 1 }, 45));
            _queue.Add(new PathNode(new[] { 0, 1 }, 45));
            _queue.Add(new PathNode(new[] { 0, 1 }, 45));
            _queue.Add(new PathNode(new[] { 0, 1 }, 45));
            _queue.Add(new PathNode(new[] { 0, 1 }, 45));
            _queue.Add(new PathNode(new[] { 0, 1 }, 45));
            Assert.AreEqual(_queue.GetSize(), 6);
        }

        [Test]
        public void TestContains()
        {
            PathNode n = new PathNode(new[] { 0, 1 }, 20);
            _queue.Add(n);
            PathNode m = new PathNode(new[] { 0, 1 }, 20);
            _queue.Add(m);
            _queue.Add(new PathNode(new[] { 0, 1 }, 45));
            _queue.Add(new PathNode(new[] { 0, 1 }, 45));
            Assert.True(_queue.Contains(n));
            Assert.True(_queue.Contains(m));
            Assert.False(_queue.Contains(new PathNode(new[] { 0, 1 }, 45)));
        }

        [Test]
        public void TestPriority()
        {
            _queue.Add(new PathNode(new[] { 0, 2 }, 35));
            _queue.Add(new PathNode(new[] { 0, 3 }, 35));
            _queue.Add(new PathNode(new[] { 0, 4 }, 3));
            _queue.Add(new PathNode(new[] { 0, 5 }, 37));
            _queue.Add(new PathNode(new[] { 0, 6 }, 55));
            _queue.Add(new PathNode(new[] { 0, 7 }, 55));
            _queue.Add(new PathNode(new[] { 0, 8 }, 75));

            PathNode node = _queue.Poll();
            for (int i = 0; i < _queue.GetSize() - 1; i++)
            {
                PathNode p = _queue.Poll();
                Assert.GreaterOrEqual(p.GetFCost(), node.GetFCost());
                node = p;
            }
        }

        [Test]
        public void TestDequeue()
        {
            _queue.Add(new PathNode(new[] { 0, 2 }, 35));
            _queue.Add(new PathNode(new[] { 0, 3 }, 35));
            _queue.Add(new PathNode(new[] { 0, 4 }, 3));
            Assert.AreEqual(_queue.Poll().GetFCost(), 3);
        }
    }
}
