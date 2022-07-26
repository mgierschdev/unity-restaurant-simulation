using UnityEngine;

public class ItemMenu : MonoBehaviour
{
    private void Start()
    {
        MenuManager.CloseMenu(gameObject);
    }

    public void OnClickSettings()
    {
        MenuManager.OpenMenu(Menu.CONFIG_MENU, gameObject);
    }

    public void OnClickBackButton()
    {
        MenuManager.CloseMenu(gameObject);
        MenuManager.OpenMenu(Menu.TOP_GAME_MENU);
    }
}