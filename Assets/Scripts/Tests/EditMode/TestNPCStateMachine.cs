using System;
using System.Collections.Generic;
using Game.Players;
using NUnit.Framework;
using Util;
using Util.Collections;

namespace Tests.EditMode
{
    /**
     * Problem: Validate NPC state machine graph coverage.
     * Goal: Ensure client and employee state machines are connected as expected.
     * Approach: BFS through state graph and assert visited counts.
     * Time: O(n + e) per test.
     * Space: O(n) for queue and visited sets.
     */
    public class TestNpcStateMachine
    {
        [Test]
        public void TestNpcStateMachineClient()
        {
            StateMachine<NpcState, NpcStateTransitions> stateMachine =
                NpcStateMachineFactory.GetClientStateMachine("ID");
            stateMachine.PrintStateMachine();
            Assert.True(AssertStates(stateMachine.Map[NpcState.Idle], 9));
        }

        [Test]
        public void TestNpcStateMachineEmployee()
        {
            StateMachine<NpcState, NpcStateTransitions> stateMachine =
                NpcStateMachineFactory.GetEmployeeStateMachine("ID");
            stateMachine.PrintStateMachine();
            Assert.True(AssertStates(stateMachine.Map[NpcState.Idle], 17));
        }

        // connected states not necesarily all of them
        private static bool AssertStates(StateMachineNode<NpcState> start, int states)
        {
            var count = 0;
            var queue = new Queue<StateMachineNode<NpcState>>();
            var visited = new HashSet<StateMachineNode<NpcState>>();
            queue.Enqueue(start);

            while (queue.Count != 0)
            {
                var size = queue.Count;

                while (size-- > 0)
                {
                    var current = queue.Dequeue();
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

            GameLog.Log(count);

            return count == states && count <= Enum.GetNames(typeof(NpcState)).Length;
        }
    }
}
