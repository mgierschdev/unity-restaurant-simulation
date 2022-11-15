using UnityEngine;
using UnityEngine.U2D.Animation;

public class InfoPopUpController : MonoBehaviour
{
    GameObject topInfoObject, topDispenserInfoPopUpImage;
    SpriteResolver spriteResolverTopDispenser;
    // Start is called before the first frame update
    void Start()
    {
        topInfoObject = transform.Find("Slider/InfoPopUp").gameObject;
        topDispenserInfoPopUpImage = topInfoObject.transform.Find("Image").gameObject;
        Util.IsNull(topInfoObject, "GameGridObject/topInfoObject null");
        Util.IsNull(topDispenserInfoPopUpImage, "GameGridObject/topDispenserInfoPopUpImage null");
        spriteResolverTopDispenser = topDispenserInfoPopUpImage.GetComponent<UnityEngine.U2D.Animation.SpriteResolver>();
        topInfoObject.SetActive(false);
        topDispenserInfoPopUpImage.SetActive(false);
    }

    public void SetSprite(string sprite)
    {
        spriteResolverTopDispenser.SetCategoryAndLabel(Settings.TopObjectInfoSprite, sprite);
    }

    public void Enable()
    {
        topInfoObject.SetActive(true);
        topDispenserInfoPopUpImage.SetActive(true);
    }

    public void Disable()
    {
        topInfoObject.SetActive(false);
        topDispenserInfoPopUpImage.SetActive(false);
    }
}
