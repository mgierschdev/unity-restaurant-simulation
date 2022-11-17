using UnityEngine;
using UnityEngine.U2D.Animation;

public class InfoPopUpController : MonoBehaviour
{
    GameObject topDispenserInfoPopUpImage;
    SpriteResolver spriteResolverTopDispenser;
    // Start is called before the first frame update
    void Awake()
    {
        topDispenserInfoPopUpImage = transform.Find("Image").gameObject;
        Util.IsNull(topDispenserInfoPopUpImage, "InfoPopUpController/topDispenserInfoPopUpImage null");
        spriteResolverTopDispenser = topDispenserInfoPopUpImage.GetComponent<SpriteResolver>();
        topDispenserInfoPopUpImage.SetActive(false);
    }

    public void SetSprite(string sprite)
    {
        spriteResolverTopDispenser.SetCategoryAndLabel(Settings.TopObjectInfoSprite, sprite);
    }

    public void Enable()
    {
        Debug.Log("Enabling InfoPopUpCOntroller");
        gameObject.SetActive(true);
        topDispenserInfoPopUpImage.SetActive(true);
    }

    public void Disable()
    {
        Debug.Log("Disabling InfoPopUpController");

        gameObject.SetActive(false);
        topDispenserInfoPopUpImage.SetActive(false);
    }

    public bool IsEnable()
    {
        return gameObject.activeSelf;
    }
}