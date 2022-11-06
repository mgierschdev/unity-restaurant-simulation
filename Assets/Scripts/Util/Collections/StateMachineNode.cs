using System.Collections.Generic;

public class StateMachineNode
{
    public List<StateMachineNode> TransitionStates { get; set; }
    public NpcState State { get; set; }
    public float maxStateTime { get; set; }

    public StateMachineNode(NpcState state)
    {
        State = state;
        TransitionStates = new List<StateMachineNode>();
    }
}