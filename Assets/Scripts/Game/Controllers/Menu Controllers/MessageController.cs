using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    private GameObject messageObj, imageObj, retryButtonObj;
    private Button retryButton;
    private TextMeshProUGUI textMessage;

    void Awake()
    {
        messageObj = transform.Find(Settings.MessageTextObject).gameObject;
        imageObj = transform.Find(Settings.MessageImageObject).gameObject;
        retryButtonObj = transform.Find(Settings.MessageRetryButton).gameObject;
        retryButton = retryButtonObj.GetComponent<Button>();
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

    public Button GetRetryButton()
    {
        return retryButton;
    }
}
