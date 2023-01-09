using TMPro;
using UnityEngine;

public class MessageController : MonoBehaviour
{
    private GameObject messageObj, imageObj;
    private TextMeshProUGUI textMessage;

    void Awake()
    {
        messageObj = transform.Find(Settings.MessageTextObject).gameObject;
        imageObj = transform.Find(Settings.MessageImageObject).gameObject;
        textMessage = messageObj.GetComponent<TextMeshProUGUI>();
    }

    public void SetTextMessage(string text)
    {
        textMessage.text = text;
    }

    public void Enable()
    {
        transform.gameObject.SetActive(true);
    }

    public void Disable()
    {
        transform.gameObject.SetActive(false);
    }

    public bool GetIsActive()
    {
        return transform.gameObject.activeSelf;
    }
}
