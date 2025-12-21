using System;
using System.Collections.Generic;

namespace Util.Collections
{
    /**
     * Problem: Represent a node in a generic state machine graph.
     * Goal: Track a state and its allowed transitions.
     * Approach: Store a state value and a list of transition nodes.
     * Time: O(n) to iterate transitions.
     * Space: O(n) for transition list.
     */
    public class StateMachineNode<T> where T : Enum
    {
        public List<StateMachineNode<T>> TransitionStates { get; set; }
        public T State { get; set; }

        public StateMachineNode(T state)
        {
            State = state;
            TransitionStates = new List<StateMachineNode<T>>();
        }

        public string GetNextStates()
        {
            string states = "";
            foreach (StateMachineNode<T> n in TransitionStates)
            {
                states += n.State.ToString() + "\n";

            }
            return states;
        }
    }
}
