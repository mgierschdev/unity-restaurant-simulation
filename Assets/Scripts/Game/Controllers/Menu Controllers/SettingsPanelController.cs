using Game.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Controllers.Menu_Controllers
{
    public class SettingsPanelController : MonoBehaviour
    {
        private Button _saveButton;
        private TextMeshProUGUI _statsText;

        private void Start()
        {
            GameObject saveButtonObject = transform.Find(Settings.SettingsMenuSaveButton).gameObject;
            _saveButton = saveButtonObject.GetComponent<Button>();
            _saveButton.onClick.AddListener(() => SaveGame());
            GameObject obj = transform.Find(Settings.SettingsMenuStatsText).gameObject;
            _statsText = obj.GetComponent<TextMeshProUGUI>();
            SetStatsText();
        }

        public void SaveGame()
        {
            PlayerData.SaveGame();
        }

        public void SetStatsText()
        {
            _statsText.text = PlayerData.GetStats();
        }
    }
}