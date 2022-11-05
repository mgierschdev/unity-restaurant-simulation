using System.Collections.Generic;

public class StateMachine
{
    public StateMachineNode start;

    public StateMachine(StateMachineNode start)
    {
        this.start = start;
    }

    public void printStateMachine()
    {

        Queue<StateMachineNode> queue = new Queue<StateMachineNode>();
        HashSet<StateMachineNode> visited = new HashSet<StateMachineNode>();

        queue.Enqueue(start);
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
}