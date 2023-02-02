using UnityEngine;
using UnityEngine.U2D.Animation;

public class SkinSelectorController : MonoBehaviour
{
    private SpriteResolver head, body, armLeft, armRight, waist, shoeLeft, shoeRight, legRight, legLeft;

    public void Start()
    {
        GameObject characterObject = gameObject.transform.Find(Settings.CharacterObjectName).gameObject;
        head = characterObject.transform.Find(CharacterParts.Head).GetComponent<SpriteResolver>();
        body = characterObject.transform.Find(CharacterParts.Body).GetComponent<SpriteResolver>();
        armLeft = characterObject.transform.Find(CharacterParts.ArmLeft).GetComponent<SpriteResolver>();
        armRight = characterObject.transform.Find(CharacterParts.ArmRight).GetComponent<SpriteResolver>();
        waist = characterObject.transform.Find(CharacterParts.Waist).GetComponent<SpriteResolver>();
        shoeLeft = characterObject.transform.Find(CharacterParts.FootLeft).GetComponent<SpriteResolver>();
        shoeRight = characterObject.transform.Find(CharacterParts.FootRight).GetComponent<SpriteResolver>();
        legRight = characterObject.transform.Find(CharacterParts.LegRight).GetComponent<SpriteResolver>();
        legLeft = characterObject.transform.Find(CharacterParts.LegLeft).GetComponent<SpriteResolver>();
    }

    public void SetCharacter(CharacterType type)
    {
        Character character = new Character(type);
        head.SetCategoryAndLabel(Settings.CategoryHeads, character.Head);
        body.SetCategoryAndLabel(Settings.CategoryBodies, character.Body);
        armLeft.SetCategoryAndLabel(Settings.CategoryArms, character.ArmLeft);
        armRight.SetCategoryAndLabel(Settings.CategoryArms, character.ArmRight);
        waist.SetCategoryAndLabel(Settings.CategoryWaist, character.Waist);
        shoeLeft.SetCategoryAndLabel(Settings.CategoryShoes, character.ShoeLeft);
        shoeRight.SetCategoryAndLabel(Settings.CategoryHeads, character.ShoeRight);
        legRight.SetCategoryAndLabel(Settings.CategoryLegs, character.LegRight);
        legLeft.SetCategoryAndLabel(Settings.CategoryLegs, character.LegLeft);
    }
}
