using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopGameMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnClick_Settings()
    {
        MenuManager.OpenMenu(Settings.Menu.CONFIG_MENU, gameObject);
    }
}