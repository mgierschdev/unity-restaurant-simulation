using TMPro;
using UnityEngine;

public class CenterTabMenuBottonController : MonoBehaviour
{
    private TextMeshProUGUI buttonText;
    private void Awake()
    {
        GameObject obj = transform.Find("Text").gameObject;
        buttonText = obj.GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        buttonText.text = text;
    }
}