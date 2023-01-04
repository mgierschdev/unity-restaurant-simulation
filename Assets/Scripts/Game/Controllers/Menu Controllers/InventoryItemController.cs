using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    private Button buyButton;
    private GameObject img;
    private Image background, imgComponent;
    private GameObject priceTextGameObject, titleTextGameObject;
    private TextMeshProUGUI priceText, titleText;
    private StoreGameObject storeGameObject;

    void Awake()
    {
        buyButton = transform.GetComponent<Button>();
        img = transform.Find(Settings.PrefabInventoryItemImage).gameObject;
        GameObject gameObject = transform.Find(Settings.PrefabMenuInventoryItemImage).gameObject;
        background = img.GetComponent<Image>();
        priceTextGameObject = transform.Find(Settings.PrefabInventoryItemTextPrice).gameObject;
        titleTextGameObject = transform.Find(Settings.PrefabInventoryItemTextTitle).gameObject;
        priceText = priceTextGameObject.GetComponent<TextMeshProUGUI>();
        titleText = titleTextGameObject.GetComponent<TextMeshProUGUI>();
        imgComponent = gameObject.GetComponent<Image>();
    }
    // Sets the Item image on the tab Menu for the current item
    public void SetInventoryItem(StoreGameObject storeGameObject)
    {
        this.storeGameObject = storeGameObject;
        transform.name = storeGameObject.StoreItemType.ToString();
        Sprite sp = MenuObjectList.ObjectSprites[storeGameObject.MenuItemSprite];
        imgComponent.sprite = sp;
        SetTitle(storeGameObject.Name);
    }

    public void SetPrice(string value)
    {
        priceText.text = TextUI.Price + ":" + value;
    }

    public void SetAmmount(string ammount)
    {
        priceText.text = TextUI.Ammount + ":" + ammount;
    }

    public void SetTitle(string title)
    {
        titleText.text = title;
    }

    public StoreGameObject GetStoreGameObject()
    {
        return storeGameObject;
    }

    public Button GetButton()
    {
        return buyButton;
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
