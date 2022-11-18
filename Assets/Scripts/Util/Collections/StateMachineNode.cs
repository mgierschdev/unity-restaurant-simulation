using System;
using System.Collections.Generic;

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