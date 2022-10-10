namespace Util.Collections;

using UnityEngine;
// Similar to priority Queue with Log(n) insertion
// Only requirement is the FCost setted for prioritizing
public class MaxBinaryHeap : IBaseGameCollections
{
    private PathNode[] nodes;
    private int treeSize;
    private int currentHeapSize;

    public MaxBinaryHeap(int heapSize)
    {
        nodes = new PathNode[heapSize];
        this.currentHeapSize = 0;
    }

    public void Add(PathNode node)
    {
        if (currentHeapSize < 0)
        {
            GameLog.LogWarning("Current heap size is negative");
        }
        else
        {
            nodes[currentHeapSize] = node;
            currentHeapSize++;
            HeapUp();
        }
    }

    private void HeapUp()
    {
        int index = currentHeapSize - 1;
        while (index >= 0 && GetParent(index).GetFCost() < nodes[index].GetFCost())
        {
            int parentIndex = GetParentIndex(index);
            Swap(parentIndex, index);
            index = parentIndex;
        }
    }

    public PathNode Poll()
    {
        if (currentHeapSize == 0)
        {
            GameLog.LogWarning("Cannot extract from an empty heap");
            return null;
        }
        else
        {
            PathNode head = nodes[0];
            nodes[0] = nodes[currentHeapSize - 1];
            currentHeapSize--;
            HeapDown();
            return head;
        }
    }

    private void HeapDown()
    {
        int index = 0;

        while (HasLeftChild(index))
        {
            int bigger = GetLeftChildIndex(index);

            if (HashRightChild(index) && GetRightChild(index).GetFCost() > GetLeftChild(index).GetFCost())
            {
                bigger = GetRightChildIndex(index);
            }

            if (nodes[bigger].GetFCost() < nodes[index].GetFCost())
            {
                break;
            }

            Swap(bigger, index);
            index = bigger;
        }
    }

    public PathNode Peek()
    {
        if (currentHeapSize == 0)
        {
            return null;
        }
        else
        {
            return nodes[0];
        }
    }

    private int GetLeftChildIndex(int index)
    {
        return 2 * index + 1;
    }

    private int GetRightChildIndex(int index)
    {
        return 2 * index + 2;
    }

    private int GetParentIndex(int index)
    {
        return (index - 1) / 2;
    }

    private bool HasLeftChild(int index)
    {
        return GetLeftChildIndex(index) < currentHeapSize;
    }

    private bool HashRightChild(int index)
    {
        return GetRightChildIndex(index) < currentHeapSize;
    }

    private PathNode GetLeftChild(int index)
    {
        return nodes[GetLeftChildIndex(index)];
    }

    private PathNode GetRightChild(int index)
    {
        return nodes[GetRightChildIndex(index)];
    }

    private PathNode GetParent(int index)
    {
        return nodes[GetParentIndex(index)];
    }

    private void PrintHeap()
    {
        string s = "";
        for (int i = 0; i < currentHeapSize; i++)
        {
            s += nodes[i].GetFCost() + " ";
        }
        GameLog.Log(s);
    }

    private void Swap(int i, int j)
    {
        (nodes[i], nodes[j]) = (nodes[j], nodes[i]);
    }

    public int GetSize()
    {
        return currentHeapSize;
    }

    public bool IsEmpty()
    {
        return currentHeapSize == 0;
    }


}