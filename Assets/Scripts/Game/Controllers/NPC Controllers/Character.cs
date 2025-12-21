namespace Game.Controllers.NPC_Controllers
{
    /**
     * Problem: Define sprite part identifiers for character variants.
     * Goal: Provide body part labels per character type.
     * Approach: Assign string identifiers based on CharacterType.
     * Time: O(1) per construction.
     * Space: O(1).
     */
    public class Character
    {
        public readonly string Head;
        public readonly string Body;
        public readonly string ArmLeft;
        public readonly string ArmRight;
        public readonly string Waist;
        public readonly string ShoeLeft;
        public readonly string ShoeRight;
        public readonly string LegRight;
        public readonly string LegLeft;

        public Character(CharacterType type)
        {
            if (type == CharacterType.Employee)
            {
                Head = "Head-1";
                Body = "Body-1";
                ArmLeft = "Arm-Left-1";
                ArmRight = "Arm-Right-1";
                Waist = "Waist-1";
                ShoeLeft = "Shoe-1";
                ShoeRight = "Shoe-1";
                LegRight = "Leg-Right-1";
                LegLeft = "Left-Left-1";
            }
            else if (type == CharacterType.Client1)
            {
                Head = "Head-1";
                Body = "Body-2";
                ArmLeft = "Arm-Left-2";
                ArmRight = "Arm-Right-2";
                Waist = "Waist-1";
                ShoeLeft = "Shoe-1";
                ShoeRight = "Shoe-1";
                LegRight = "Leg-Right-1";
                LegLeft = "Left-Left-1";
            }
            else if (type == CharacterType.Client2)
            {
                Head = "Head-2";
                Body = "Body-2";
                ArmLeft = "Arm-Left-2";
                ArmRight = "Arm-Right-2";
                Waist = "Waist-1";
                ShoeLeft = "Shoe-1";
                ShoeRight = "Shoe-1";
                LegRight = "Leg-Right-1";
                LegLeft = "Left-Left-1";
            }
            else if (type == CharacterType.Client3)
            {
                Head = "Head-3";
                Body = "Body-2";
                ArmLeft = "Arm-Left-2";
                ArmRight = "Arm-Right-2";
                Waist = "Waist-1";
                ShoeLeft = "Shoe-1";
                ShoeRight = "Shoe-1";
                LegRight = "Leg-Right-1";
                LegLeft = "Left-Left-1";
            }
            else if (type == CharacterType.Client4)
            {
                Head = "Head-4";
                Body = "Body-2";
                ArmLeft = "Arm-Left-2";
                ArmRight = "Arm-Right-2";
                Waist = "Waist-1";
                ShoeLeft = "Shoe-1";
                ShoeRight = "Shoe-1";
                LegRight = "Leg-Right-1";
                LegLeft = "Left-Left-1";
            }
        }
    }
}
