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
        topDispenserInfoPopUpImage.SetActive(false);
        animator = transform.GetComponent<Animator>();
        DisableAnimation();
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

    public void Disable()
    {
        DisableAnimation();
        gameObject.SetActive(false);
        topDispenserInfoPopUpImage.SetActive(false);
    }

    public bool IsEnable()
    {
        return gameObject.activeSelf;
    }

    private void EnableAnimation()
    {
        animator.ResetTrigger(InfoAnimatorState.Idle);
        animator.SetTrigger(InfoAnimatorState.Moving);
    }

    private void DisableAnimation()
    {
        animator.ResetTrigger(InfoAnimatorState.Moving);
        animator.SetTrigger(InfoAnimatorState.Idle);
    }
}