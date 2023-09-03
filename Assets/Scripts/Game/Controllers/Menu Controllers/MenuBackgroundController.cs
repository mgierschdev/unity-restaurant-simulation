using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Game.Controllers.Menu_Controllers
{
    public class MenuBackgroundController : MonoBehaviour
    {
        private MenuHandlerController _menuHandlerController;
        private Button _backgroundMenuImageButton;
        private Image _image;
        private bool _isActive;

        void Start()
        {
            //Background Button
            GameObject menuBackground = transform.Find(Settings.MenuBackground).gameObject;
            Util.Util.IsNull(menuBackground, "MenuBackgroundController.cs/menuBackground null");

            _backgroundMenuImageButton = menuBackground.GetComponent<Button>();
            if (_backgroundMenuImageButton == null)
            {
                GameLog.LogError("MenuBackgroundController.cs/backgroundMenuImageButton null");
            }

            _image = menuBackground.GetComponent<Image>();
            if (_image == null)
            {
                GameLog.LogError("MenuBackgroundController.cs/image null");
            }

            _menuHandlerController =
                GameObject.Find(Settings.ConstCanvasParentMenu).GetComponent<MenuHandlerController>();
            _backgroundMenuImageButton.onClick.AddListener(ButtonClicked);
            Disable();
            _isActive = false;
        }

        public void ButtonClicked()
        {
            _menuHandlerController.CloseMenu();
        }

        public void Disable()
        {
            _backgroundMenuImageButton.interactable = false;
            _image.raycastTarget = false;
            _isActive = false;
        }

        public void Enable()
        {
            _backgroundMenuImageButton.interactable = true;
            _image.raycastTarget = true;
            _isActive = true;
        }

        public bool IsActive()
        {
            return _isActive;
        }
    }
}