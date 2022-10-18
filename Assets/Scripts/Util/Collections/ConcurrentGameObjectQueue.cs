using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class ConcurrentGameObjectQueue<T> : ConcurrentQueue<T>
{
    HashSet<T> set;

    public ConcurrentGameObjectQueue()
    {
        set = new HashSet<T>();
    }

    public bool Contains(T obj)
    {
        return set.Contains(obj);
    }

    public new void Enqueue(T obj)
    {
        if (set.Contains(obj))
        {
            return;
        }

        base.Enqueue(obj);
        set.Add(obj);
    }

    public T TryDequeue()
    {
        TryDequeue(out T tmp);
        set.Remove(tmp);
        return tmp;
    }

    public void Remove(T element)
    {
        if (!set.Contains(element))
        {
            return;
        }

        Queue<T> queue = new Queue<T>();

        while (Count > 0)
        {
            TryDequeue(out T tmp);

            if (!tmp.Equals(element))
            {
                queue.Enqueue(tmp);
            }
        }

        while (queue.Count > 0)
        {
            Enqueue(queue.Dequeue());
        }
    }
}