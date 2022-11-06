using System;

public class StateNodeTransition
{
    // Same size as NpcStateTransitions
    public int[] stateTransitions; 
 
    public StateNodeTransition()
    {
        stateTransitions = new int[Enum.GetNames(typeof(NpcStateTransitions)).Length];
    }
}