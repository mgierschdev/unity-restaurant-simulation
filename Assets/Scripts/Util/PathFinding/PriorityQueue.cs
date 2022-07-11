using System;
using UnityEngine;

public class PriorityQueue
{
    private QueueNode rootNode;
    private int size;

    public PriorityQueue()
    {
        rootNode = new QueueNode(new int[] { int.MinValue, int.MinValue }, float.MinValue); // The head is the max Value
        size = 0;
    }

    // Peeks the head Node
    public int[] Peek()
    {
        if (rootNode.next == null)
        {
            return new int[] { };
        }

        return rootNode.next.GetData();
    }

    public QueueNode Dequeue()
    {
        if (rootNode.next == null)
        {
            Debug.LogWarning("The Queue is empty");
            return null;
        }
        size--;
        QueueNode n = rootNode.next;
        rootNode.next = rootNode.next.next;
        return n;
    }

    public bool IsEmpty()
    {
        return size == 0;
    }

    // In O(n)
    public void Enqueue(int[] d, double p)
    {
        size++;
        QueueNode runner = rootNode;
        QueueNode newNode = new QueueNode(d, p);

        while (runner.next != null && runner.next.GetPriority() < newNode.GetPriority())
        {
            runner = runner.next;
        }

        QueueNode tmp = runner.next;
        runner.next = newNode;
        newNode.next = tmp;
    }

    // DEBUG
    private void PrintNodeList()
    {
        QueueNode q = rootNode;
        String s = "";
        while (q != null)
        {
            s += q.GetPriority() + " -> ";
            q = q.next;
        }
        Debug.Log(s);
    }

    public int GetSize()
    {
        return this.size;
    }
}
