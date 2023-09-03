using System;
using System.Collections.Generic;

// Builds a StateMachine given 2 enums.
// Example: Npc state machine represented as a directed graph
// T = NpcState, S = NpcStateTransitions. Finite states = T, Transitions = S
namespace Util.Collections
{
    public class StateMachine<T, TS> where T : Enum where TS : Enum
    {
        public string ID;
        public Dictionary<T, StateMachineNode<T>> Map { get; private set; }
        public StateMachineNode<T> Current { get; private set; }
    
        private StateNodeTransition[,] AdjacencyMatrix { get; set; }
        private int TransitionStates { get; set; } // State machine current states
        private readonly T _startState;

        public StateMachine(StateNodeTransition[,] adjacencyMatrix, T startState, string id)
        {
            this.ID = id;
            AdjacencyMatrix = adjacencyMatrix;
            Map = new Dictionary<T, StateMachineNode<T>>();
            TransitionStates = 0;
            AddNodes();// Adds nodes to map
            BuildGraph();
            Current = Map[startState];
            this._startState = startState;
        }

        private void BuildGraph()
        {
            foreach (T state in Enum.GetValues(typeof(T)))
            {
                StateMachineNode<T> node = Map[state];

                foreach (T neighbor in Enum.GetValues(typeof(T)))
                {
                    if (AdjacencyMatrix[(int)Enum.Parse(typeof(T), node.State.ToString()), (int)Enum.Parse(typeof(T), neighbor.ToString())] != null)
                    {
                        node.TransitionStates.Add(Map[neighbor]);
                    }
                }
            }
        }

        private void AddNodes()
        {
            foreach (T state in Enum.GetValues(typeof(T)))
            {
                Map.Add(state, new StateMachineNode<T>(state));
            }
        }

        public void PrintStateMachine()
        {
            Queue<StateMachineNode<T>> queue = new Queue<StateMachineNode<T>>();
            HashSet<T> visited = new HashSet<T>();

            queue.Enqueue(Map[_startState]);
            int level = 0;

            while (queue.Count != 0)
            {
                int size = queue.Count;

                while (size-- > 0)
                {
                    StateMachineNode<T> current = queue.Dequeue();

                    if (visited.Contains(current.State))
                    {
                        continue;
                    }

                    GameLog.Log("Node: " + current.State + " floor " + level + " node id " + current.GetHashCode());
                    visited.Add(current.State);
                    foreach (StateMachineNode<T> node in current.TransitionStates)
                    {
                        if (!visited.Contains(node.State))
                        {
                            queue.Enqueue(node);
                        }
                    }
                }
                level++;
            }
        }

        public void SetTransition(TS transition)
        {
            TransitionStates = BitUtil.SetBit(TransitionStates, (int)Enum.Parse(typeof(TS), transition.ToString()));
        }

        public void UnSetTransition(TS transition)
        {
            TransitionStates = BitUtil.UnSetBit(TransitionStates, (int)Enum.Parse(typeof(TS), transition.ToString()));
        }

        public bool GetTransitionState(TS transition)
        {
            return BitUtil.GetBit(TransitionStates, (int)Enum.Parse(typeof(TS), transition.ToString()));
        }

        public void UnSetAll()
        {
            TransitionStates = 0;
        }

        public void CheckTransition()
        {
            foreach (StateMachineNode<T> node in Current.TransitionStates)
            {
                StateNodeTransition transition = AdjacencyMatrix[(int)Enum.Parse(typeof(T), Current.State.ToString()), (int)Enum.Parse(typeof(T), node.State.ToString())];
                Int32 mask = transition.StateTransitionsEncoded & TransitionStates;

                if (mask == transition.StateTransitionsEncoded && TransitionStates != 0)
                {
                    Current = node;
                    break;
                }
            }
        }

        public override string ToString()
        {
            return BitUtil.GetBinaryString(TransitionStates);
        }

        public string DebugTransitions()
        {
            string debug = "";
            foreach (TS state in Enum.GetValues(typeof(TS)))
            {
                if (GetTransitionState(state))
                {
                    debug += state.ToString() + "= 1";
                }
                else
                {
                    debug += state.ToString() + "= 0";
                }
                debug += "\n";
            }
            return debug;
        }

        public StateMachineNode<T> GetStartNode()
        {
            return Map[_startState];
        }
    }
}