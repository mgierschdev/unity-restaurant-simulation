public class StateNodeTransition
{
    // Same size as NpcStateTransitions
    public int[] StateTransitions { get; set; }

    public StateNodeTransition(int[] nodeTransition)
    {
        StateTransitions = nodeTransition;
    }
}