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
    private int upgradeValue;

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
        upgradeValue = PlayerData.GetUgrade(storeGameObject.UpgradeType);
        SetCurrentLevel();
        SetTitle(storeGameObject.Name);
    }

    public void SetTitle(string value)
    {
        titleText.text = value;
    }

    public void SetCurrentLevel()
    {
        textCurrentUpgrade.text = TextUI.CurrentLevel + ":" + (storeGameObject.MaxLevel >= upgradeValue ? TextUI.Max : upgradeValue.ToString());
    }

    public void SetPrice(string value)
    {
        textCost.text = TextUI.Price + ":" + value;
    }

    public void IncreaseUpgrade()
    {
        if (storeGameObject.Cost <= PlayerData.GetMoneyDouble())
        {
            upgradeValue++;
            SetCurrentLevel();
        }
        else
        {
            SetUnavailable();
        }
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
        imgComponent.color = Util.DisableColor;
    }

    public void SetAvailable()
    {
        imgComponent.color = Color.white;
    }
}
