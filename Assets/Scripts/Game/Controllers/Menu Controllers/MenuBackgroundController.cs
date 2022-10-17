using UnityEngine;
using UnityEngine.UI;

public class MenuBackgroundController : MonoBehaviour
{
    private MenuHandlerController menuHandlerController;
    private Button backgroundMenuImageButton;
    private Image image;
    private bool isActive;

    void Start()
    {
        //Background Button
        GameObject menuBackground = transform.Find(Settings.MenuBackground).gameObject;
        Util.IsNull(menuBackground, "MenuBackgroundController.cs/menuBackground null");

        backgroundMenuImageButton = menuBackground.GetComponent<Button>();
        if (backgroundMenuImageButton == null)
        {
            GameLog.LogError("MenuBackgroundController.cs/backgroundMenuImageButton null");
        }

        image = menuBackground.GetComponent<Image>();
        if (image == null)
        {
            GameLog.LogError("MenuBackgroundController.cs/image null");
        }

        menuHandlerController = GameObject.Find(Settings.ConstCanvasParentMenu).GetComponent<MenuHandlerController>();
        backgroundMenuImageButton.onClick.AddListener(ButtonClicked);
        Disable();
        isActive = false;
    }

    public void ButtonClicked()
    {
        menuHandlerController.CloseMenu();
    }

    public void Disable()
    {
        backgroundMenuImageButton.interactable = false;
        image.raycastTarget = false;
        isActive = false;
    }

    public void Enable()
    {
        backgroundMenuImageButton.interactable = true;
        image.raycastTarget = true;
        isActive = true;
    }

    public bool IsActive()
    {
        return isActive;
    }
}