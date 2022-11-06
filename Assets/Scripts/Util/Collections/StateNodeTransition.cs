public class StateNodeTransition
{
    // Same size as NpcStateTransitions
    public bool[] StateTransitions { get; set; }

    public StateNodeTransition(bool[] nodeTransition)
    {
        StateTransitions = nodeTransition;
    }
}