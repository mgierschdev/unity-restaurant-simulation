using UnityEngine;
using UnityEngine.UI;

public class MenuBackgroundController : MonoBehaviour
{
    private MenuHandlerController menuHandlerController;
    // Start is called before the first frame update
    void Start()
    {
        //Background Button
        Button backgroundMenuImageButton = transform.Find(Settings.MenuBackground).GetComponent<Button>();
        menuHandlerController = GameObject.Find(Settings.ConstCanvasParentMenu).GetComponent<MenuHandlerController>();
        backgroundMenuImageButton.onClick.AddListener(ButtonClicked);
        
    }

    public void ButtonClicked()
    {
        menuHandlerController.CloseMenu();
    }
}
