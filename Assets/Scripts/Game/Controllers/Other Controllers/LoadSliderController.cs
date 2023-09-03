using System;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Game.Controllers.Other_Controllers
{
    public class LoadSliderController : MonoBehaviour
    {
        private Slider _slider;
        private float _currentEnergy, _energyBarTime, _seconds;
        private Image _sliderImage, _backgroundImage;
        private bool _finished;

        public void Awake()
        {
            _slider = GetComponent<Slider>();
            var fillAreaObject = transform.Find("FillArea/Fill").gameObject;
            var backgroundImageObject = transform.Find("Background").gameObject;
            _backgroundImage = backgroundImageObject.GetComponent<Image>();

            Util.Util.IsNull(fillAreaObject, "LoadSliderController/fillAreaObject is null");
            Util.Util.IsNull(backgroundImageObject, "LoadSliderController/backgroundImageObject is null");

            _sliderImage = fillAreaObject.GetComponent<Image>();
            _slider.value = 0;
            _seconds = 0;
            _finished = false;

            if (_slider == null)
            {
                GameLog.LogWarning("EnergyBarController/Slider null");
            }

            SetMaxEnergy(Settings.NpcDefaultEnergy);
            SetSliderSprite("Sprite-Circle"); //Default load sprite
        }

        public void Update()
        {
            // EnergyBar controller, only if it is active
            if (gameObject.activeSelf)
            {
                _seconds += Time.unscaledDeltaTime;

                if (_seconds <= _energyBarTime)
                {
                    _currentEnergy = _seconds * 100 / _energyBarTime;
                    SetEnergy((int)_currentEnergy);
                }
                else
                {
                    _finished = true;
                    _seconds = 0;
                    SetInactive();
                }
            }
        }

        public void SetDefaultFillTime(float time)
        {
            _energyBarTime = time;
        }

        private void SetEnergy(int energy)
        {
            _slider.value = energy;
        }

        private void SetMaxEnergy(int maxEnergy)
        {
            _slider.maxValue = maxEnergy;
            _slider.value = maxEnergy;
        }

        public void SetInactive()
        {
            gameObject.SetActive(false);
        }

        public void SetActive(float seconds)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                RestartState();
                _energyBarTime = seconds;
            }
        }

        public void SetActive()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                _energyBarTime = _energyBarTime == 0 ? 3 : _energyBarTime;
                RestartState();
            }
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        private float GetCurrentEnergy()
        {
            return _currentEnergy;
        }

        public bool IsFinished()
        {
            return _finished;
        }

        public void RestartState()
        {
            _finished = false;
            _currentEnergy = 0;
            _seconds = 0;
            SetEnergy(0);
        }

        public void SetSliderFillMethod(Image.FillMethod method)
        {
            _sliderImage.fillMethod = method;
        }

        public void SetSliderSprite(string spReference)
        {
            spReference = Settings.StoreSpritePath + spReference;
            Sprite sp = Resources.Load<Sprite>(spReference);

            if (!sp)
            {
                throw new Exception("Sprite not found SetSliderSprite() " + spReference);
            }

            _backgroundImage.sprite = sp;
            _sliderImage.sprite = sp;
        }
    }
}