using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CenterTabMenuBottonController : MonoBehaviour
{
    private TextMeshProUGUI buttonText;
    private Button button;

    private void Awake()
    {
        GameObject obj = transform.Find("Text").gameObject;
        buttonText = obj.GetComponent<TextMeshProUGUI>();
        button = obj.GetComponent<Button>();
    }

    public void SetText(string text)
    {
        buttonText.text = text;
    }

    public Button GetButton()
    {
        return button;
    }
}