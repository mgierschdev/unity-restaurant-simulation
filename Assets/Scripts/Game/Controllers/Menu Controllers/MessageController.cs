using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Controllers.Menu_Controllers
{
    /**
     * Problem: Control UI message panel display and retry actions.
     * Goal: Show/hide messages and expose the retry button.
     * Approach: Cache UI references and toggle GameObject active state.
     * Time: O(1) per call.
     * Space: O(1).
     */
    public class MessageController : MonoBehaviour
    {
        private GameObject _messageObj, _imageObj, _retryButtonObj;
        private Button _retryButton;
        private TextMeshProUGUI _textMessage;

        void Awake()
        {
            _imageObj = transform.Find(Settings.MessageImageObject).gameObject;
            _messageObj = transform.Find(Settings.MessageTextObject).gameObject;
            _retryButtonObj = transform.Find(Settings.MessageRetryButton).gameObject;
            _retryButton = _retryButtonObj.GetComponent<Button>();
            _textMessage = _messageObj.GetComponent<TextMeshProUGUI>();
        }

        public void SetTextMessage(string text)
        {
            _textMessage.text = text;
        }

        public void Enable()
        {
            if (!transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(true);
            }
        }

        public void Disable()
        {
            if (transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(false);
            }
        }

        public bool GetIsActive()
        {
            return transform.gameObject.activeSelf;
        }

        public Button GetRetryButton()
        {
            return _retryButton;
        }
    }
}
