using NUnit.Framework;
using Util;
using Util.PathFinding;

namespace Tests.EditMode
{
    public class TestBinaryHeap
    {
        private MaxBinaryHeap _heap;

        [Test]
        public void TestInsertElements()
        {
            _heap = new MaxBinaryHeap(100);
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
            _heap = new MaxBinaryHeap(100);
            _heap.Add(new PathNode(new[] { 0, 1 }, 10));
            Assert.AreEqual(_heap.Peek().GetFCost(), 10);
            _heap.Add(new PathNode(new[] { 0, 1 }, 20));
            Assert.AreEqual(_heap.Peek().GetFCost(), 20);
            _heap.Add(new PathNode(new[] { 0, 1 }, 30));
            Assert.AreEqual(_heap.Peek().GetFCost(), 30);
        }

        [Test]
        public void TestPeekExtractAndInsert()
        {
            _heap = new MaxBinaryHeap(100);
            _heap.Add(new PathNode(new[] { 0, 1 }, 45));
            Assert.AreEqual(_heap.GetSize(), 1);
            _heap.Add(new PathNode(new[] { 0, 1 }, 555));
            _heap.Add(new PathNode(new[] { 0, 1 }, 44));
            _heap.Add(new PathNode(new[] { 0, 1 }, 4));
            _heap.Add(new PathNode(new[] { 0, 1 }, 35));
            Assert.AreEqual(_heap.Poll().GetFCost(), 555);
            Assert.AreEqual(_heap.Poll().GetFCost(), 45);
            _heap.Add(new PathNode(new[] { 0, 1 }, 100));
            _heap.Add(new PathNode(new[] { 0, 1 }, -5));
            Assert.AreEqual(_heap.Peek().GetFCost(), 100);
            _heap.Add(new PathNode(new[] { 0, 1 }, 100));
            _heap.Add(new PathNode(new[] { 0, 1 }, 10));
            _heap.Add(new PathNode(new[] { 0, 1 }, 104));
            _heap.Add(new PathNode(new[] { 0, 1 }, -10));

            var node = _heap.Poll();

            for (var i = 0; i < _heap.GetSize(); i++)
            {
                var p = _heap.Poll();
                GameLog.Log(p.GetFCost() + " > " + node.GetFCost());
                Assert.GreaterOrEqual(node.GetFCost(), p.GetFCost());
                node = p;
            }
        }
    }
}