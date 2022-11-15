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
    // Sets the Item image on the tab Menu for the current item
    public void SetInventoryItem(string spReference, string botLeftLabelValue)
    {
        Debug.Log("SpRefenrece: " + spReference);
        
        Sprite sp = Resources.Load<Sprite>(spReference);
        imgComponent.sprite = sp;
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
