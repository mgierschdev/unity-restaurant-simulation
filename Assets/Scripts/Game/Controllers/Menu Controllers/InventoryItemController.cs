using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    private Button button;
    private GameObject img;
    private Image background;
    private GameObject text;
    private TextMeshProUGUI textMesh;
    private Image imgComponent;

    void Awake()
    {
        button = GetComponent<Button>();
        img = transform.Find(Settings.PrefabInventoryItemImage).gameObject;
        GameObject gameObject = transform.Find("Image/ItemImage").gameObject;
        background = img.GetComponent<Image>();
        text = transform.Find(Settings.PrefabInventoryItemTextPrice).gameObject;
        textMesh = text.GetComponent<TextMeshProUGUI>();
        imgComponent = img.GetComponent<Image>();
    }

    public void SetInventoryItem(string spReference, string cost)
    {
        Sprite sp = Resources.Load<Sprite>(spReference);
        imgComponent.sprite = sp;
        textMesh.text = cost;
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
