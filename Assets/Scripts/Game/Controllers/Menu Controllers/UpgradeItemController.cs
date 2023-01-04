using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemController : MonoBehaviour
{
    private Button button;
    private GameObject img;
    private Image background, imgComponent;
    private GameObject textCostGameObject, textCurrentLevelGameObject, titleTextGameObject;
    private TextMeshProUGUI textCost, textCurrentUpgrade, titleText;
    private StoreGameObject storeGameObject;

    void Awake()
    {
        button = transform.GetComponent<Button>();
        img = transform.Find(Settings.PrefabInventoryItemImage).gameObject;
        GameObject gameObject = transform.Find(Settings.PrefabMenuInventoryItemImage).gameObject;
        background = img.GetComponent<Image>();
        textCostGameObject = transform.Find(Settings.PrefabUpgradeItemTextPrice).gameObject;
        textCurrentLevelGameObject = transform.Find(Settings.PrefabUpgradeLevelItemTextPrice).gameObject;
        titleTextGameObject = transform.Find(Settings.PrefabInventoryItemTextTitle).gameObject;
        textCost = textCostGameObject.GetComponent<TextMeshProUGUI>();
        textCurrentUpgrade = textCurrentLevelGameObject.GetComponent<TextMeshProUGUI>();
        titleText = titleTextGameObject.GetComponent<TextMeshProUGUI>();
        imgComponent = gameObject.GetComponent<Image>();
    }

    // Sets the Item image on the tab Menu for the current item
    public void SetInventoryItem(StoreGameObject storeGameObject)
    {
        this.storeGameObject = storeGameObject;
        transform.name = storeGameObject.UpgradeType.ToString();
        Sprite sp = MenuObjectList.ObjectSprites[storeGameObject.MenuItemSprite];
        imgComponent.sprite = sp;
        SetPrice(storeGameObject.Cost.ToString());
        SetCurrentLevel(PlayerData.GetUgrade(storeGameObject.UpgradeType).ToString());
        SetTitle(storeGameObject.Name);
    }

    public void SetTitle(string value)
    {
        titleText.text = value;
    }

    public void SetCurrentLevel(string value)
    {
        textCurrentUpgrade.text = TextUI.CurrentLevel + ":" + value;
    }

    public void SetPrice(string value)
    {
        textCost.text = TextUI.Price + ":" + value;
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
