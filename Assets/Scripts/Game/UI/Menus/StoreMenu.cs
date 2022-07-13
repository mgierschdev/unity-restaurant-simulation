using UnityEngine;

public class StoreMenu : MonoBehaviour
{
    private void Awake()
    {
        MenuManager.CloseMenu(gameObject);
    }

    public void OnClick_Settings()
    {
        MenuManager.OpenMenu(Menu.CONFIG_MENU, gameObject);
    }

    public void OnClickBackButton()
    {
        MenuManager.CloseMenu(gameObject);
        MenuManager.OpenMenu(Menu.TOP_GAME_MENU);
    }
}