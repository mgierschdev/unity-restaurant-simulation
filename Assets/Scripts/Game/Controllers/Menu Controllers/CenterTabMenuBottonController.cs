using TMPro;
using UnityEngine;

public class CenterTabMenuBottonController : MonoBehaviour
{
    private TextMeshProUGUI buttonText;
    void Start()
    {
        GameObject obj = transform.Find("Text").gameObject;
        buttonText = obj.GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        buttonText.text = text;
    }
}