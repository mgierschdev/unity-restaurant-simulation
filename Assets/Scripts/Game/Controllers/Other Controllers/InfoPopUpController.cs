using UnityEngine;
using UnityEngine.U2D.Animation;

public class InfoPopUpController : MonoBehaviour
{
    private GameObject topStoreItemInfoPopUpImage;
    private SpriteResolver spriteResolverTopDispenser;
    private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        topStoreItemInfoPopUpImage = transform.Find("Image").gameObject;
        Util.IsNull(topStoreItemInfoPopUpImage, "InfoPopUpController/topDispenserInfoPopUpImage null");
        spriteResolverTopDispenser = topStoreItemInfoPopUpImage.GetComponent<SpriteResolver>();
        animator = transform.GetComponent<Animator>();
        spriteResolverTopDispenser.SetCategoryAndLabel(Settings.TopObjectInfoSprite, "Dispenser-1");//Default Item
        DisableAnimation();
        topStoreItemInfoPopUpImage.SetActive(false);
    }

    public void SetSprite(string sprite)
    {
        spriteResolverTopDispenser.SetCategoryAndLabel(Settings.TopObjectInfoSprite, sprite);
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
        spriteResolverTopDispenser.SetCategoryAndLabel(Settings.TopObjectInfoSprite, MenuObjectList.GetItemSprite(item));    
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