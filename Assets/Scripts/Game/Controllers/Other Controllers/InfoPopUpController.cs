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
        spriteResolverTopDispenser.SetCategoryAndLabel(Settings.TopObjectInfoSprite, "Dispenser-1");//TODO: TMP
        DisableAnimation();
        topDispenserInfoPopUpImage.SetActive(false);
    }

    public void SetSprite(string sprite)
    {
        spriteResolverTopDispenser.SetCategoryAndLabel(Settings.TopObjectInfoSprite, sprite);
    }

    public void Enable()
    {
        Debug.Log("Enabling animation while waiting to be attended - ");
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

        Debug.Log("Enabling animation while waiting to be attended");
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

    public bool IsEnable()
    {
        return gameObject.activeSelf;
    }

    private void EnableAnimation()
    {
        animator.ResetTrigger(InfoAnimatorState.Idle);
        animator.SetTrigger(InfoAnimatorState.Moving);
        animator.Play(InfoAnimatorState.Idle, 0);
    }

    private void DisableAnimation()
    {
        animator.ResetTrigger(InfoAnimatorState.Moving);
        animator.SetTrigger(InfoAnimatorState.Idle);
        animator.StopPlayback();
    }
}