using System;
using UnityEngine;
using System.Collections.Generic;

// Only requirement is a node.next in the PathNode and the FCost setted for prioritizing
public class PriorityQueue : IBaseGameCollections
{
    private HashSet<PathNode> nodes;
    private PathNode rootNode;
    private int size;

    public PriorityQueue()
    {
        nodes = new HashSet<PathNode>();
        rootNode = new PathNode(new int[] { int.MinValue, int.MinValue }, int.MaxValue); // The head is the max Value
        size = 0;
    }

    // Peeks the head Node
    public PathNode Peek()
    {
        if (rootNode.next == null)
        {
            return null;
        }

        return rootNode.next;
    }

    public PathNode Poll()
    {
        if (rootNode.next == null)
        {
            GameLog.LogWarning("The Queue is empty");
            return null;
        }
        size--;
        PathNode n = rootNode.next;
        nodes.Remove(n);
        rootNode.next = rootNode.next.next;
        return n;
    }

    public bool IsEmpty()
    {
        return size == 0;
    }

    // In O(n)
    public void Add(PathNode node)
    {
        nodes.Add(node);
        size++;
        PathNode runner = rootNode;

        while (runner.next != null && runner.next.GetFCost() < node.GetFCost())
        {
            runner = runner.next;
        }

        PathNode tmp = runner.next;
        runner.next = node;
        node.next = tmp;
    }

    public bool Contains(PathNode n)
    {
        return nodes.Contains(n);
    }

    // DEBUG
    private void PrintNodeList()
    {
        PathNode q = rootNode;
        String s = "";
        while (q != null)
        {
            s += q.GetFCost() + " -> ";
            q = q.next;
        }
        GameLog.Log(s);
    }

    public int GetSize()
    {
        return this.size;
    }
}
