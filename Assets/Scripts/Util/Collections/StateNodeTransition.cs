using System;

namespace Util.Collections
{
    /**
     * Problem: Encode state transition flags into a bitmask.
     * Goal: Represent allowed transitions compactly for state machines.
     * Approach: Convert boolean arrays to an Int32 bitset.
     * Time: O(n) for encoding (n = transitions).
     * Space: O(n) for transition array.
     */
    public class StateNodeTransition
    {
        // Same size as NpcStateTransitions
        private bool[] StateTransitions { get; set; } //single 32 bit int

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

        public void PrintBinaryRepresentation()
        {
            GameLog.Log("Binary representation: " + BitUtil.GetBinaryString(StateTransitionsEncoded));
        }
    }
}
