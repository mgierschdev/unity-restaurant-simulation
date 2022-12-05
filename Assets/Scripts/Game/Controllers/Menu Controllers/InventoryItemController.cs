using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    private Button buyButton;
    private GameObject img;
    private Image background, imgComponent;
    private GameObject text;
    private TextMeshProUGUI textMesh;
    private StoreItemType storeItemType;

    void Awake()
    {
        buyButton = transform.GetComponent<Button>();
        img = transform.Find(Settings.PrefabInventoryItemImage).gameObject;
        GameObject gameObject = transform.Find(Settings.PrefabMenuInventoryItemImage).gameObject;
        background = img.GetComponent<Image>();
        text = transform.Find(Settings.PrefabInventoryItemTextPrice).gameObject;
        textMesh = text.GetComponent<TextMeshProUGUI>();
        imgComponent = gameObject.GetComponent<Image>();
    }
    // Sets the Item image on the tab Menu for the current item
    public void SetInventoryItem(string spReference, string botLeftLabelValue, StoreItemType storeItemType)
    {
        this.storeItemType = storeItemType;
        Sprite sp = Resources.Load<Sprite>(spReference);
        if (!sp)
        {
            GameLog.LogWarning("Sprite not found SetInventoryItem() " + spReference);
            spReference = Settings.StoreSpritePath + Settings.DefaultSquareSprite;
            sp = Resources.Load<Sprite>(spReference);
        }
        imgComponent.sprite = sp;
        textMesh.text = botLeftLabelValue;
    }

    public Button GetButton()
    {
        return buyButton;
    }

    public void SetBackground(Color color)
    {
        background.color = color;
    }
}
