public class StateNodeTransition
{
    // Same size as NpcStateTransitions
    public bool[] StateTransitions { get; set; }
    public float TimeToTransition { get; set; }

    public StateNodeTransition(bool[] nodeTransition, float TimeToTransition)
    {
        StateTransitions = nodeTransition;
        this.TimeToTransition = TimeToTransition;
    }
}