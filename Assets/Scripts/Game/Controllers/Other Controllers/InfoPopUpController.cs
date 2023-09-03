using Game.Players;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Util;

namespace Game.Controllers.Other_Controllers
{
    public class InfoPopUpController : MonoBehaviour
    {
        private GameObject _topStoreItemInfoPopUpImage;
        private SpriteResolver _spriteResolverTopStoreItem;
        private Animator _animator;

        // Start is called before the first frame update
        void Awake()
        {
            _topStoreItemInfoPopUpImage = transform.Find("Image").gameObject;
            Util.Util.IsNull(_topStoreItemInfoPopUpImage, "InfoPopUpController/topStoreItemInfoPopUpImage null");
            _spriteResolverTopStoreItem = _topStoreItemInfoPopUpImage.GetComponent<SpriteResolver>();
            _animator = transform.GetComponent<Animator>();
            _spriteResolverTopStoreItem.SetCategoryAndLabel(Settings.TopObjectInfoSprite,
                "Store-1-Item-1"); //Default Item
            DisableAnimation();
            _topStoreItemInfoPopUpImage.SetActive(false);
        }

        public void SetSprite(string sprite)
        {
            _spriteResolverTopStoreItem.SetCategoryAndLabel(Settings.TopObjectInfoSprite, sprite);
        }

        public void Enable()
        {
            EnableAnimation();
            gameObject.SetActive(true);
            _topStoreItemInfoPopUpImage.SetActive(true);
        }

        public void EnableWithoutAnimation()
        {
            if (gameObject.activeSelf)
            {
                return;
            }

            DisableAnimation();
            gameObject.SetActive(true);
            _topStoreItemInfoPopUpImage.SetActive(true);
        }

        public void Disable()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            DisableAnimation();
            gameObject.SetActive(false);
            _topStoreItemInfoPopUpImage.SetActive(false);
        }

        public void SetInfoPopUItem(ItemType item)
        {
            _spriteResolverTopStoreItem.SetCategoryAndLabel(Settings.TopObjectInfoSprite,
                GameObjectList.GetItemSprite(item));
        }

        private void EnableAnimation()
        {
            _animator.enabled = true;
            _animator.ResetTrigger(InfoAnimatorState.Idle);
            _animator.SetTrigger(InfoAnimatorState.Moving);
        }

        private void DisableAnimation()
        {
            _animator.enabled = false;
            _animator.ResetTrigger(InfoAnimatorState.Moving);
            _animator.SetTrigger(InfoAnimatorState.Idle);
        }
    }
}