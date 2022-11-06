using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestNPCStateMachine
{
    [Test]
    public void TestNPCStateMachineClient()
    {
        StateMachine stateMachine = NPCStateMachineFactory.GetClientStateMachine();
        Assert.True(AssertStates(stateMachine.Map[NpcState.IDLE], 9));
    }

    // connected states not necesarily all of them
    private bool AssertStates(StateMachineNode start, int states)
    {
        int count = 0;
        Queue<StateMachineNode> queue = new Queue<StateMachineNode>();
        HashSet<StateMachineNode> visited = new HashSet<StateMachineNode>();
        queue.Enqueue(start);

        while (queue.Count != 0)
        {
            int size = queue.Count;

            while (size-- > 0)
            {
                StateMachineNode current = queue.Dequeue();
                count++;
                visited.Add(current);
                foreach (StateMachineNode node in current.TransitionStates)
                {
                    if (!visited.Contains(node))
                    {
                        queue.Enqueue(node);
                    }
                }
            }
        }

        Debug.Log(count);

        return count == states && count <= Enum.GetNames(typeof(NpcState)).Length;
    }
}