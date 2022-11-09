using System;
using System.Collections.Generic;

// Npc state machine represented as a directed graph
// T = NpcState, S = NpcStateTransitions. Finite states = T, Transitions = S
public class StateMachine<T, S> where T : Enum where S : Enum
{
    public string ID;
    public StateNodeTransition[,] AdjacencyMatrix { get; set; }
    public Dictionary<T, StateMachineNode<T>> Map { get; set; }
    public StateMachineNode<T> Current { get; set; }
    public bool[] TransitionStates { get; set; }
    private T startState;

    public StateMachine(StateNodeTransition[,] adjacencyMatrix, T startState, string ID)
    {
        this.ID = ID;
        AdjacencyMatrix = adjacencyMatrix;
        Map = new Dictionary<T, StateMachineNode<T>>();
        TransitionStates = new bool[Enum.GetNames(typeof(S)).Length];
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
        TransitionStates[(int)(int)Enum.Parse(typeof(S), transition.ToString())] = true;
    }

    public void UnSetTransition(S transition)
    {
        TransitionStates[(int)Enum.Parse(typeof(S), transition.ToString())] = false;
    }

    public bool GetTransitionState(S transition)
    {
        return TransitionStates[(int)Enum.Parse(typeof(S), transition.ToString())];
    }

    public void UnSetAll()
    {
        TransitionStates = new bool[Enum.GetNames(typeof(S)).Length];
    }

    public void CheckTransition()
    {
        //TODO: we could encode this into a single integer, instead of an array
        //Except from the time/ attributes
        foreach (StateMachineNode<T> node in Current.TransitionStates)
        {
            StateNodeTransition transition = AdjacencyMatrix[(int)Enum.Parse(typeof(T), Current.State.ToString()), (int)Enum.Parse(typeof(T), node.State.ToString())];
            bool valid = true;

            for (int i = 0; i < transition.StateTransitions.Length; i++)
            {
                // Transitions from the adj matrix should match TransitionStates
                if (transition.StateTransitions[i] && TransitionStates[i] != transition.StateTransitions[i])
                {
                    //TODO: erase debug
                    if (ID.Contains(Settings.EMPLOYEE_PREFIX))
                    {
                        GameLog.Log("Cannot move from " + Current.State + " ---> " + node.State + " reason: " + Enum.GetName(typeof(NpcStateTransitions), i));
                    }

                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                GameLog.Log("Moving to: " + node.State);
                Current = node;
                break;
            }
        }
    }

    public override string ToString()
    {
        string str = "";
        for (int i = 0; i < TransitionStates.GetLength(0); i++)
        {
            str += Enum.GetName(typeof(S), i) + ":" + TransitionStates[i] + " ";
        }
        return str;
    }

    public StateMachineNode<T> GetStartNode()
    {
        return Map[startState];
    }

    public string GetDebugTransitions()
    {
        string str = "";

        for (int i = 0; i < TransitionStates.Length; i++)
        {
            str += Enum.GetName(typeof(S), i) + " " + TransitionStates[i] + " \n";
        }
        return str;
    }
}