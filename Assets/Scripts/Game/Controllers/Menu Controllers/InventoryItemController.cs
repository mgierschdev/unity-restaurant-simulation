using Game.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Game.Controllers.Menu_Controllers
{
    public class InventoryItemController : MonoBehaviour
    {
        private Button _buyButton;
        private GameObject _img;
        private Image _background, _imgComponent;
        private GameObject _priceTextGameObject, _titleTextGameObject;
        private TextMeshProUGUI _priceText, _titleText;
        private StoreGameObject _storeGameObject;

        void Awake()
        {
            _buyButton = transform.GetComponent<Button>();
            _img = transform.Find(Settings.PrefabInventoryItemImage).gameObject;
            var gameObjectItemImage = transform.Find(Settings.PrefabMenuInventoryItemImage).gameObject;
            _background = _img.GetComponent<Image>();
            _priceTextGameObject = transform.Find(Settings.PrefabInventoryItemTextPrice).gameObject;
            _titleTextGameObject = transform.Find(Settings.PrefabInventoryItemTextTitle).gameObject;
            _priceText = _priceTextGameObject.GetComponent<TextMeshProUGUI>();
            _titleText = _titleTextGameObject.GetComponent<TextMeshProUGUI>();
            _imgComponent = gameObjectItemImage.GetComponent<Image>();
        }

        // Sets the Item image on the tab Menu for the current item
        public void SetInventoryItem(StoreGameObject storeGameObject)
        {
            this._storeGameObject = storeGameObject;
            transform.name = storeGameObject.StoreItemType.ToString();
            Sprite sp = GameObjectList.ObjectSprites[storeGameObject.MenuItemSprite];
            _imgComponent.sprite = sp;
            SetTitle(storeGameObject.Name);
        }

        public void SetPrice(string value)
        {
            _priceText.text = TextUI.Price + ":" + value;
        }

        public void SetAmmount(string ammount)
        {
            _priceText.text = TextUI.Amount + ":" + ammount;
        }

        public void SetTitle(string title)
        {
            _titleText.text = title;
        }

        public StoreGameObject GetStoreGameObject()
        {
            return _storeGameObject;
        }

        public Button GetButton()
        {
            return _buyButton;
        }

        public void SetBackground(Color color)
        {
            _background.color = color;
        }

        public void SetUnavailable()
        {
            _imgComponent.color = Util.Util.DisableColor;
        }

        public void SetAvailable()
        {
            _imgComponent.color = Color.white;
        }
    }
}