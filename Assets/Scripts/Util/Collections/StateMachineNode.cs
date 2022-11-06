using System.Collections.Generic;

public class StateMachineNode
{
    public List<StateMachineNode> TransitionStates { get; set; }
    public StateNodeTransition stateNodeTransition { get; set; }

    public NpcState State { get; set; }

    public StateMachineNode(NpcState state)
    {
        TransitionStates = new List<StateMachineNode>();
        State = state;
    }
}