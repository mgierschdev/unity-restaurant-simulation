using System;
using System.Collections.Generic;
using System.Linq;
using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Assertions;

public class FirebaseQueue
{
    private Queue<Action> actionQueue = new Queue<Action>();

    public void EnqueueAction(Action action)
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
            Action action;
            lock (actionQueue)
            {
                action = actionQueue.Dequeue();
            }

            action();
        }
    }
}