using System;
using System.Collections.Generic;
using Util.Collections;

// Only requirement is a node.next in the PathNode and the FCost setted for prioritizing
namespace Util.PathFinding
{
    /**
     * Problem: Provide a simple priority queue for PathNode ordering.
     * Goal: Insert and remove nodes by F-cost priority.
     * Approach: Maintain a sorted linked list using PathNode.Next.
     * Time: O(n) insert, O(1) peek/poll.
     * Space: O(n) for node storage.
     */
    public class PriorityQueue : IBaseGameCollections
    {
        private readonly HashSet<PathNode> _nodes;
        private readonly PathNode _rootNode;
        private int _size;

        public PriorityQueue()
        {
            _nodes = new HashSet<PathNode>();
            _rootNode = new PathNode(new int[] { int.MinValue, int.MinValue }, int.MaxValue); // The head is the max Value
            _size = 0;
        }

        // Peeks the head Node
        public PathNode Peek()
        {
            if (_rootNode.Next == null)
            {
                return null;
            }

            return _rootNode.Next;
        }

        public PathNode Poll()
        {
            if (_rootNode.Next == null)
            {
                GameLog.LogWarning("The Queue is empty");
                return null;
            }
            _size--;
            PathNode n = _rootNode.Next;
            _nodes.Remove(n);
            _rootNode.Next = _rootNode.Next.Next;
            return n;
        }

        public bool IsEmpty()
        {
            return _size == 0;
        }

        // In O(n)
        public void Add(PathNode node)
        {
            _nodes.Add(node);
            _size++;
            PathNode runner = _rootNode;

            while (runner.Next != null && runner.Next.GetFCost() < node.GetFCost())
            {
                runner = runner.Next;
            }

            PathNode tmp = runner.Next;
            runner.Next = node;
            node.Next = tmp;
        }

        public bool Contains(PathNode n)
        {
            return _nodes.Contains(n);
        }
    
        private void PrintNodeList()
        {
            PathNode q = _rootNode;
            String s = "";
            while (q != null)
            {
                s += q.GetFCost() + " -> ";
                q = q.Next;
            }
            GameLog.Log(s);
        }

        public int GetSize()
        {
            return this._size;
        }
    }
}
