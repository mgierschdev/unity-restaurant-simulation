using System;

// Bit encoding utility
namespace Util.Collections
{
    /**
     * Problem: Provide bitwise helper utilities.
     * Goal: Set, unset, and read bit flags in integers.
     * Approach: Use bit masks and shifts.
     * Time: O(1) per operation.
     * Space: O(1).
     */
    public static class BitUtil
    {
        //Sets the selected bit to 1
        public static Int32 SetBit(Int32 num, int bit)
        { 
            num |= 1 << bit;
            return num;
        }

        //UnSet the selected bit to 0
        public static Int32 UnSetBit(Int32 num, int bit)
        {
            Int32 mask = ~(1 << bit);
            num &= mask;
            return num;
        }
    
        //Gets the value of the bit
        public static bool GetBit(Int32 num, int bit)
        {
            return (num & (1 << bit)) != 0;
        }

        public static string GetBinaryString(Int32 num)
        {
            return Convert.ToString(num, 2);
        }
    }
}
