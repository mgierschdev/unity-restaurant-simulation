using System;
using System.Collections.Generic;
using System.Linq;
using Firebase;
using UnityEngine;
using UnityEngine.Assertions;
using System.Threading.Tasks;

// For keeping track and processing queue of async firebase messages
public class FirebaseQueue
{
    private Queue<Task> actionQueue = new Queue<Task>();

    public void EnqueueAction(Task action)
    {
        lock (actionQueue)
        {
            actionQueue.Enqueue(action);
        }
    }

    public void ProcessQueue()
    {
        while (actionQueue.Any())
        {
            Task action;
            lock (actionQueue)
            {
                action = actionQueue.Dequeue();
            }
            action.Start();
        }
    }
}