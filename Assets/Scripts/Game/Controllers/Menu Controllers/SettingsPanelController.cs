using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    private Button saveButton;
    private TextMeshProUGUI statsText;

    private void Start()
    {
        GameObject saveButtonObject = transform.Find(Settings.SettingsMenuSaveButton).gameObject;
        saveButton = saveButtonObject.GetComponent<Button>();
        saveButton.onClick.AddListener(() => SaveGame());
        GameObject obj = transform.Find(Settings.SettingsMenuStatsText).gameObject;
        statsText = obj.GetComponent<TextMeshProUGUI>();
    }

    public void SaveGame()
    {
        PlayerData.SaveGame();
        GameLog.Log("TODO: popup Game saved");
    }

    public void SetStatsText()
    {
        statsText.text = PlayerData.GetStats();
    }
}