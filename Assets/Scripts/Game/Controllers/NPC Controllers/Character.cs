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
        if (type == CharacterType.EMPLOYEE)
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
        else if (type == CharacterType.CLIENT_1)
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
        else if (type == CharacterType.CLIENT_2)
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
        else if (type == CharacterType.CLIENT_3)
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
        else if (type == CharacterType.CLIENT_4)
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