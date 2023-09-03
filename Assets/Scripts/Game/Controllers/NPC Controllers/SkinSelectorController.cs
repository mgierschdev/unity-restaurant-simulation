using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Game.Controllers.NPC_Controllers
{
    public class SkinSelectorController : MonoBehaviour
    {
        private SpriteResolver _head, _body, _armLeft, _armRight, _waist, _shoeLeft, _shoeRight, _legRight, _legLeft;

        public void Start()
        {
            GameObject characterObject = gameObject.transform.Find(Settings.CharacterObjectName).gameObject;
            _head = characterObject.transform.Find(CharacterParts.Head).GetComponent<SpriteResolver>();
            _body = characterObject.transform.Find(CharacterParts.Body).GetComponent<SpriteResolver>();
            _armLeft = characterObject.transform.Find(CharacterParts.ArmLeft).GetComponent<SpriteResolver>();
            _armRight = characterObject.transform.Find(CharacterParts.ArmRight).GetComponent<SpriteResolver>();
            _waist = characterObject.transform.Find(CharacterParts.Waist).GetComponent<SpriteResolver>();
            _shoeLeft = characterObject.transform.Find(CharacterParts.FootLeft).GetComponent<SpriteResolver>();
            _shoeRight = characterObject.transform.Find(CharacterParts.FootRight).GetComponent<SpriteResolver>();
            _legRight = characterObject.transform.Find(CharacterParts.LegRight).GetComponent<SpriteResolver>();
            _legLeft = characterObject.transform.Find(CharacterParts.LegLeft).GetComponent<SpriteResolver>();
        }

        public void SetCharacter(CharacterType type)
        {
            Character character = new Character(type);
            _head.SetCategoryAndLabel(Settings.CategoryHeads, character.Head);
            _body.SetCategoryAndLabel(Settings.CategoryBodies, character.Body);
            _armLeft.SetCategoryAndLabel(Settings.CategoryArms, character.ArmLeft);
            _armRight.SetCategoryAndLabel(Settings.CategoryArms, character.ArmRight);
            _waist.SetCategoryAndLabel(Settings.CategoryWaist, character.Waist);
            _shoeLeft.SetCategoryAndLabel(Settings.CategoryShoes, character.ShoeLeft);
            _shoeRight.SetCategoryAndLabel(Settings.CategoryHeads, character.ShoeRight);
            _legRight.SetCategoryAndLabel(Settings.CategoryLegs, character.LegRight);
            _legLeft.SetCategoryAndLabel(Settings.CategoryLegs, character.LegLeft);
        }
    }
}