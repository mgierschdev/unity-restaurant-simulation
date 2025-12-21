using Game.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Game.Controllers.Menu_Controllers
{
    /**
     * Problem: Render upgrade items in the menu UI.
     * Goal: Display upgrade cost, level, and availability.
     * Approach: Bind UI components and update with PlayerData values.
     * Time: O(1) per update.
     * Space: O(1).
     */
    public class UpgradeItemController : MonoBehaviour
    {
        private Button _button;
        private GameObject _img;
        private Image _background, _imgComponent;
        private GameObject _textCostGameObject, _textCurrentLevelGameObject, _titleTextGameObject;
        private TextMeshProUGUI _textCost, _textCurrentUpgrade, _titleText;
        private StoreGameObject _storeGameObject;
        private int _upgradeValue;

        void Awake()
        {
            _button = transform.GetComponent<Button>();
            _img = transform.Find(Settings.PrefabInventoryItemImage).gameObject;
            var gameObjectImagePrefab = transform.Find(Settings.PrefabMenuInventoryItemImage).gameObject;
            _background = _img.GetComponent<Image>();
            _textCostGameObject = transform.Find(Settings.PrefabUpgradeItemTextPrice).gameObject;
            _textCurrentLevelGameObject = transform.Find(Settings.PrefabUpgradeLevelItemTextPrice).gameObject;
            _titleTextGameObject = transform.Find(Settings.PrefabInventoryItemTextTitle).gameObject;
            _textCost = _textCostGameObject.GetComponent<TextMeshProUGUI>();
            _textCurrentUpgrade = _textCurrentLevelGameObject.GetComponent<TextMeshProUGUI>();
            _titleText = _titleTextGameObject.GetComponent<TextMeshProUGUI>();
            _imgComponent = gameObjectImagePrefab.GetComponent<Image>();
        }

        // Sets the Item image on the tab Menu for the current item
        public void SetInventoryItem(StoreGameObject storeGameObject)
        {
            this._storeGameObject = storeGameObject;
            transform.name = storeGameObject.UpgradeType.ToString();
            Sprite sp = GameObjectList.ObjectSprites[storeGameObject.MenuItemSprite];
            _imgComponent.sprite = sp;
            SetPrice(storeGameObject.Cost.ToString());
            _upgradeValue = PlayerData.GetUgrade(storeGameObject.UpgradeType);
            SetCurrentLevel();
            SetTitle(storeGameObject.Name);
        }

        public void SetTitle(string value)
        {
            _titleText.text = value;
        }

        public void SetCurrentLevel()
        {
            _textCurrentUpgrade.text = TextUI.CurrentLevel + ":" +
                                       (_storeGameObject.MaxLevel <= _upgradeValue
                                           ? TextUI.Max
                                           : _upgradeValue.ToString());
        }

        public void SetPrice(string value)
        {
            _textCost.text = TextUI.Price + ":" + value;
        }

        public void IncreaseUpgrade()
        {
            if (_storeGameObject.Cost <= PlayerData.GetMoneyDouble())
            {
                _upgradeValue++;
                SetCurrentLevel();
            }
            else
            {
                SetUnavailable();
            }
        }

        public StoreGameObject GetStoreGameObject()
        {
            return _storeGameObject;
        }

        public Button GetButton()
        {
            return _button;
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
