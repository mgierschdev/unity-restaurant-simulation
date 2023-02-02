public class Character
{
    public string Head, Body, ArmLeft, ArmRight, Waist, ShoeLeft, ShoeRight, LegRight, LegLeft;

    public Character(CharacterType type)
    {
        if (type == CharacterType.employee)
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
        else if (type == CharacterType.client_1)
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
        else if (type == CharacterType.client_2)
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
        else if (type == CharacterType.client_3)
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
        else if (type == CharacterType.client_4)
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