namespace Game.Controllers.NPC_Controllers
{
    /**
     * Problem: Define default character part labels.
     * Goal: Provide baseline sprite part names for character assembly.
     * Approach: Expose static string constants for parts.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public static class CharacterParts
    {
        public static readonly string Head = "Head-1";
        public static readonly string Body = "Body-1";
        public static readonly string ArmRight = "Arm-Right-1";
        public static readonly string ArmLeft = "Arm-Left-1";
        public static readonly string Waist = "Waist-1";
        public static readonly string FootLeft = "Foot-Left-1";
        public static readonly string FootRight = "Foot-Right-1";
        public static readonly string LegRight = "Leg-Right-1";
        public static readonly string LegLeft = "Leg-Left-1";
    }

    public enum CharacterType
    {
        Employee = 1,
        Client1 = 2,
        Client2 = 3,
        Client3 = 4,
        Client4 = 5
    }
}
