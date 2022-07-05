using UnityEngine;

// This class is in charge of the game settings
// Attached to: 
public class ConfigMenu : MonoBehaviour
{
    private void Awake()
    {
        MenuManager.CloseMenu(gameObject);
    }

    public void OnClick_Store()
    {
        MenuManager.OpenMenu(Settings.Menu.STORE_MENU, gameObject);
    }

    public void OnClickBackButton()
    {
        MenuManager.CloseMenu(gameObject);
        MenuManager.OpenMenu(Settings.Menu.TOP_GAME_MENU);
    }
}
