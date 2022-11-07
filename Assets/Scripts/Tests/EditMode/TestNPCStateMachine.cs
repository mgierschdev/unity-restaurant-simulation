using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestNPCStateMachine
{
    [Test]
    public void TestNPCStateMachineClient()
    {
        StateMachine<NpcState, NpcStateTransitions> stateMachine = NPCStateMachineFactory.GetClientStateMachine();
        stateMachine.printStateMachine();
        Assert.True(AssertStates(stateMachine.Map[NpcState.IDLE], 9));
    }

    [Test]
    public void TestNPCStateMachineEmployee()
    {
        StateMachine<NpcState, NpcStateTransitions> stateMachine = NPCStateMachineFactory.GetEmployeeStateMachine();
        stateMachine.printStateMachine();
        Assert.True(AssertStates(stateMachine.Map[NpcState.IDLE], 9));
    }

    // connected states not necesarily all of them
    private bool AssertStates(StateMachineNode<NpcState> start, int states)
    {
        int count = 0;
        Queue<StateMachineNode<NpcState>> queue = new Queue<StateMachineNode<NpcState>>();
        HashSet<StateMachineNode<NpcState>> visited = new HashSet<StateMachineNode<NpcState>>();
        queue.Enqueue(start);

        while (queue.Count != 0)
        {
            int size = queue.Count;

            while (size-- > 0)
            {
                StateMachineNode<NpcState> current = queue.Dequeue();
                count++;
                visited.Add(current);
                foreach (StateMachineNode<NpcState> node in current.TransitionStates)
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