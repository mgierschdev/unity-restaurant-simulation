using System;
using System.Collections.Generic;

// Npc state machine represented as a directed graph
public class StateMachine
{
    public StateNodeTransition[,] AdjacencyMatrix { get; set; }
    public Dictionary<NpcState, StateMachineNode> Map { get; set; }
    public StateMachineNode Current { get; set; }

    public StateMachine(StateNodeTransition[,] adjacencyMatrix)
    {
        AdjacencyMatrix = adjacencyMatrix;
        Map = new Dictionary<NpcState, StateMachineNode>();
        AddNodes();
        BuildGraph();
        Current = Map[NpcState.IDLE];
    }

    private void BuildGraph()
    {
        foreach (NpcState state in Enum.GetValues(typeof(NpcState)))
        {
            StateMachineNode node = Map[state];

            foreach (NpcState neighbor in Enum.GetValues(typeof(NpcState)))
            {
                if (AdjacencyMatrix[(int)node.State, (int)neighbor] != null)
                {
                    node.TransitionStates.Add(Map[neighbor]);
                }
            }
        }
    }

    private void AddNodes()
    {
        foreach (NpcState state in Enum.GetValues(typeof(NpcState)))
        {
            Map.Add(state, new StateMachineNode(state));
        }
    }

    public void printStateMachine()
    {
        Queue<StateMachineNode> queue = new Queue<StateMachineNode>();
        HashSet<StateMachineNode> visited = new HashSet<StateMachineNode>();

        queue.Enqueue(Map[NpcState.IDLE]);
        int level = 0;

        while (queue.Count != 0)
        {
            int size = queue.Count;

            while (size-- > 0)
            {
                StateMachineNode current = queue.Dequeue();
                GameLog.Log("Node: " + current.State + " floor " + level);
                visited.Add(current);
                foreach (StateMachineNode node in current.TransitionStates)
                {
                    if (!visited.Contains(node))
                    {
                        queue.Enqueue(node);
                    }
                }
            }
            level++;
        }
    }

    public void CheckTransition(bool[] transitionAttributes)
    {
        //TODO: we could encode this into a single integer, instead of an array
        //Except from the time/ attributes
        foreach (StateMachineNode node in Current.TransitionStates)
        {
            StateNodeTransition transition = AdjacencyMatrix[(int)Current.State, (int)node.State];
            bool valid = true;

            for (int i = 0; i < transition.StateTransitions.Length; i++)
            {
                if (transition.StateTransitions[i] && transitionAttributes[i] != transition.StateTransitions[i])
                {
                   // GameLog.Log("Cannot move to: " + node.State +" attribute "+ i);
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
             //   GameLog.Log("Moving to: " + node.State);
                Current = node;
                break;
            }
        }
    }
}