using System;

public class StateNodeTransition
{
    // Same size as NpcStateTransitions
    public bool[] StateTransitions { get; set; } //single 32 bit int

    public Int32 StateTransitionsEncoded { get; set; }

    public StateNodeTransition(bool[] nodeTransition)
    {
        StateTransitions = nodeTransition;
        EncodeTransitions();
    }

    private void EncodeTransitions()
    {
        StateTransitionsEncoded = 0;

        for (int i = 0; i < StateTransitions.Length; i++)
        {
            if (StateTransitions[i])
            {
                StateTransitionsEncoded = BitUtil.SetBit(StateTransitionsEncoded, i);
            }
        }
    }

    public void printBinaryRepresentation()
    {
        GameLog.Log("Binary representation: " + BitUtil.GetBinaryString(StateTransitionsEncoded));
    }
}