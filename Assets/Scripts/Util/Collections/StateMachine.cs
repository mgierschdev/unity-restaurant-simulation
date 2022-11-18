using System;
using System.Collections.Generic;
using UnityEngine;

// Npc state machine represented as a directed graph
// T = NpcState, S = NpcStateTransitions. Finite states = T, Transitions = S
public class StateMachine<T, S> where T : Enum where S : Enum
{
    public string ID;
    public StateNodeTransition[,] AdjacencyMatrix { get; set; }
    public Dictionary<T, StateMachineNode<T>> Map { get; set; }
    public StateMachineNode<T> Current { get; set; }
    public Int32 TransitionStates { get; set; } // State machine current states
    private T startState;

    public StateMachine(StateNodeTransition[,] adjacencyMatrix, T startState, string ID)
    {
        this.ID = ID;
        AdjacencyMatrix = adjacencyMatrix;
        Map = new Dictionary<T, StateMachineNode<T>>();
        TransitionStates = 0;
        AddNodes();// Adds nodes to map
        BuildGraph();
        Current = Map[startState];
        this.startState = startState;
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

    public void printStateMachine()
    {
        Queue<StateMachineNode<T>> queue = new Queue<StateMachineNode<T>>();
        HashSet<T> visited = new HashSet<T>();

        queue.Enqueue(Map[startState]);
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

    public void SetTransition(S transition)
    {
        Debug.Log(" ");
        // we set the bit
        TransitionStates |= 1 << (int)Enum.Parse(typeof(S), transition.ToString());
        Debug.Log("Setting " + Enum.Parse(typeof(S), transition.ToString()) + " " + Convert.ToString(TransitionStates, 2) + " " + (int)Enum.Parse(typeof(S), transition.ToString()));
        Debug.Log(" ");
    }

    public void UnSetTransition(S transition)
    {
        // we clear the bit
        Int32 mask = ~(1 << (int)Enum.Parse(typeof(S), transition.ToString()));
        // Debug.Log("UnSetTransition bit " + (int)Enum.Parse(typeof(S), transition.ToString()));
        // Debug.Log("UnSetTransition Current states " + ToString());
        // Debug.Log("UnSetTransition Mask " + Convert.ToString(mask, 2));

        TransitionStates &= mask;

        // Debug.Log("UnSetTransition result Current state " + ToString());
    }

    public bool GetTransitionState(S transition)
    {
        // we get the bit
        return (TransitionStates & (1 << (int)Enum.Parse(typeof(S), transition.ToString()))) != 0;
    }

    public void UnSetAll()
    {
        TransitionStates = 0;
    }

    public void CheckTransition()
    {
        //TODO: we could encode this into a single integer, instead of an array
        //GameLog.Log(" current "+);
        string debug = " ";
        //AdjacencyMatrix

        Debug.Log("CheckTransition()");
        foreach (StateMachineNode<T> node in Current.TransitionStates)
        {
            StateNodeTransition transition = AdjacencyMatrix[(int)Enum.Parse(typeof(T), Current.State.ToString()), (int)Enum.Parse(typeof(T), node.State.ToString())];

            Int32 mask = transition.StateTransitionsEncoded & TransitionStates;

            Debug.Log("CheckTransition() " + mask + " " + Convert.ToString(transition.StateTransitionsEncoded, 2) + " " + Convert.ToString(TransitionStates, 2));

            if (mask == transition.StateTransitionsEncoded && TransitionStates != 0)
            {
                Debug.Log("CheckTransition(): Valid " + Enum.Parse(typeof(T), node.State.ToString()));
                Current = node;
                break;
            }
        }
    }

    public override string ToString()
    {
        return Convert.ToString(TransitionStates, 2);
    }

    public string DebugTransitions()
    {
        string debug = "";
        foreach (S state in Enum.GetValues(typeof(S)))
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
        return Map[startState];
    }
}