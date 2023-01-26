using UnityEngine;
using UnityEngine.U2D.Animation;

public class InfoPopUpController : MonoBehaviour
{
    private GameObject topStoreItemInfoPopUpImage;
    private SpriteResolver spriteResolverTopStoreItem;
    private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        topStoreItemInfoPopUpImage = transform.Find("Image").gameObject;
        Util.IsNull(topStoreItemInfoPopUpImage, "InfoPopUpController/topStoreItemInfoPopUpImage null");
        spriteResolverTopStoreItem = topStoreItemInfoPopUpImage.GetComponent<SpriteResolver>();
        animator = transform.GetComponent<Animator>();
        spriteResolverTopStoreItem.SetCategoryAndLabel(Settings.TopObjectInfoSprite, "Store-1-Item-1");//Default Item
        DisableAnimation();
        topStoreItemInfoPopUpImage.SetActive(false);
    }

    public void SetSprite(string sprite)
    {
        spriteResolverTopStoreItem.SetCategoryAndLabel(Settings.TopObjectInfoSprite, sprite);
    }

    public void Enable()
    {
        EnableAnimation();
        gameObject.SetActive(true);
        topStoreItemInfoPopUpImage.SetActive(true);
    }

    public void EnableWithoutAnimation()
    {
        if (gameObject.activeSelf)
        {
            return;
        }

        DisableAnimation();
        gameObject.SetActive(true);
        topStoreItemInfoPopUpImage.SetActive(true);
    }

    public void Disable()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        DisableAnimation();
        gameObject.SetActive(false);
        topStoreItemInfoPopUpImage.SetActive(false);
    }

    public void SetInfoPopUItem(ItemType item)
    {
        spriteResolverTopStoreItem.SetCategoryAndLabel(Settings.TopObjectInfoSprite, GameObjectList.GetItemSprite(item));    
    }

    private void EnableAnimation()
    {
        animator.enabled = true;
        animator.ResetTrigger(InfoAnimatorState.Idle);
        animator.SetTrigger(InfoAnimatorState.Moving);
    }

    private void DisableAnimation()
    {
        animator.enabled = false;
        animator.ResetTrigger(InfoAnimatorState.Moving);
        animator.SetTrigger(InfoAnimatorState.Idle);
    }
}