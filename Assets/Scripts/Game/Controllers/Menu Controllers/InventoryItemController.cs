using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    private Button button;
    private GameObject img;
    private Image background, imgComponent;
    private GameObject text;
    private TextMeshProUGUI textMesh;

    void Awake()
    {
        button = GetComponent<Button>();
        img = transform.Find(Settings.PrefabInventoryItemImage).gameObject;
        GameObject gameObject = transform.Find(Settings.PrefabMenuInventoryItemImage).gameObject;
        background = img.GetComponent<Image>();
        text = transform.Find(Settings.PrefabInventoryItemTextPrice).gameObject;
        textMesh = text.GetComponent<TextMeshProUGUI>();
        imgComponent = gameObject.GetComponent<Image>();
    }

    public void SetInventoryItem(string spReference, string botLeftLabelValue)
    {
        Sprite sp = Resources.Load<Sprite>(spReference);
        imgComponent.sprite = sp;
        Debug.Log("Setting image component " + sp + " " + spReference + " bot label " + botLeftLabelValue);
        textMesh.text = botLeftLabelValue;
    }

    public Button GetButton()
    {
        return button;
    }

    public void SetBackground(Color color)
    {
        background.color = color;
    }
}
