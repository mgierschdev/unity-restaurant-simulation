using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Controllers.Menu_Controllers
{
    /**
     * Problem: Control the tab button UI in the center menu.
     * Goal: Set button text and expose the button instance.
     * Approach: Cache TextMeshProUGUI and Button components.
     * Time: O(1) per call.
     * Space: O(1).
     */
    public class CenterTabMenuBottonController : MonoBehaviour
    {
        private TextMeshProUGUI _buttonText;
        private Button _button;

        private void Awake()
        {
            GameObject obj = transform.Find("Text").gameObject;
            _buttonText = obj.GetComponent<TextMeshProUGUI>();
            _button = transform.GetComponent<Button>();
        }

        public void SetText(string text)
        {
            _buttonText.text = text;
        }

        public Button GetButton()
        {
            return _button;
        }

        public Button LoadAndGetButton()
        {
            _button = transform.GetComponent<Button>();
            return _button;
        }
    }
}
