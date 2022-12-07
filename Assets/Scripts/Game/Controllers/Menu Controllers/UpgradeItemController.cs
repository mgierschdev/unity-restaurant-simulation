using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemController : MonoBehaviour
{
    private Button button;
    private GameObject img;
    private Image background, imgComponent;
    private GameObject text;
    private TextMeshProUGUI textMesh;
    private StoreGameObject storeGameObject;

    void Awake()
    {
        button = transform.GetComponent<Button>();
        img = transform.Find(Settings.PrefabInventoryItemImage).gameObject;
        GameObject gameObject = transform.Find(Settings.PrefabMenuInventoryItemImage).gameObject;
        background = img.GetComponent<Image>();
        text = transform.Find(Settings.PrefabInventoryItemTextPrice).gameObject;
        textMesh = text.GetComponent<TextMeshProUGUI>();
        imgComponent = gameObject.GetComponent<Image>();
    }
    // Sets the Item image on the tab Menu for the current item
    public void SetInventoryItem(string spReference, string botLeftLabelValue, StoreGameObject storeGameObject)
    {
        this.storeGameObject = storeGameObject;
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

    public StoreGameObject GetStoreGameObject()
    {
        return storeGameObject;
    }

    public Button GetButton()
    {
        return button;
    }

    public void SetBackground(Color color)
    {
        background.color = color;
    }

    public void SetUnavailable()
    {
        imgComponent.color = Color.black;
    }

    public void SetAvailable()
    {
        imgComponent.color = Color.white;
    }
}
