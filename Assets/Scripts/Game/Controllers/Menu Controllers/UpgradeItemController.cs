using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemController : MonoBehaviour
{
    private Button button;
    private GameObject img;
    private Image background, imgComponent;
    private GameObject textCostGameObject, textCurrentUpgradeGameObject;
    private TextMeshProUGUI textCost, textCurrentUpgrade;
    private StoreGameObject storeGameObject;

    void Awake()
    {
        button = transform.GetComponent<Button>();
        img = transform.Find(Settings.PrefabInventoryItemImage).gameObject;
        GameObject gameObject = transform.Find(Settings.PrefabMenuInventoryItemImage).gameObject;
        background = img.GetComponent<Image>();
        textCostGameObject = transform.Find(Settings.PrefabUpgradeItemTextPrice).gameObject;
        textCurrentUpgradeGameObject = transform.Find(Settings.PrefabUpgradeLevelItemTextPrice).gameObject;
        textCost = textCostGameObject.GetComponent<TextMeshProUGUI>();
        textCurrentUpgrade = textCurrentUpgradeGameObject.GetComponent<TextMeshProUGUI>();
        imgComponent = gameObject.GetComponent<Image>();
    }

    // Sets the Item image on the tab Menu for the current item
    public void SetInventoryItem(string spReference, string botLeftLabelValue, StoreGameObject storeGameObject)
    {
        this.storeGameObject = storeGameObject;
        transform.name = storeGameObject.UpgradeType.ToString();
        Sprite sp = MenuObjectList.ObjectSprites[spReference];
        imgComponent.sprite = sp;
        SetPrice(botLeftLabelValue);
        SetCurrentLevel(PlayerData.GetUgrade(storeGameObject.UpgradeType).ToString());
    }

    public void SetCurrentLevel(string value)
    {
        textCurrentUpgrade.text = TextUI.CurrentLevel + ":" + value;
    }

    public void SetPrice(string value)
    {
        textCost.text = value;
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
