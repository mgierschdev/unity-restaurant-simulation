using UnityEngine;
using UnityEngine.U2D.Animation;

public class InfoPopUpController : MonoBehaviour
{
    private GameObject topDispenserInfoPopUpImage;
    private SpriteResolver spriteResolverTopDispenser;
    private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        topDispenserInfoPopUpImage = transform.Find("Image").gameObject;
        Util.IsNull(topDispenserInfoPopUpImage, "InfoPopUpController/topDispenserInfoPopUpImage null");
        spriteResolverTopDispenser = topDispenserInfoPopUpImage.GetComponent<SpriteResolver>();
        animator = transform.GetComponent<Animator>();
        spriteResolverTopDispenser.SetCategoryAndLabel(Settings.TopObjectInfoSprite, "Dispenser-1");//Default Item
        DisableAnimation();
        topDispenserInfoPopUpImage.SetActive(false);
    }

    public void SetSprite(string sprite)
    {
        spriteResolverTopDispenser.SetCategoryAndLabel(Settings.TopObjectInfoSprite, sprite);
    }

    public void Enable()
    {
        EnableAnimation();
        gameObject.SetActive(true);
        topDispenserInfoPopUpImage.SetActive(true);
    }

    public void EnableWithoutAnimation()
    {
        if (gameObject.activeSelf)
        {
            return;
        }

        DisableAnimation();
        gameObject.SetActive(true);
        topDispenserInfoPopUpImage.SetActive(true);
    }

    public void Disable()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        DisableAnimation();
        gameObject.SetActive(false);
        topDispenserInfoPopUpImage.SetActive(false);
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