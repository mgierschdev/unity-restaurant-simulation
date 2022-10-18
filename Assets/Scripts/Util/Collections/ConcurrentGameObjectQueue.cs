using System.Collections.Concurrent;
using System.Collections.Generic;

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
        base.TryDequeue(out T tmp);
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

        while (base.Count > 0)
        {
            base.TryDequeue(out T tmp);

            if (!tmp.Equals(element))
            {
                queue.Enqueue(tmp);
            }
        }

        while (queue.Count > 0)
        {
            base.Enqueue(queue.Dequeue());
        }
    }
}