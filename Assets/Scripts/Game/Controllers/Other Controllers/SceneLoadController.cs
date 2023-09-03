using System;
using Game.Controllers.Menu_Controllers;
using Game.Players;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

// This controller is attached to the -LoadScene-  not to the main game scene -World-
// Check the folder under Assets/Scenes
namespace Game.Controllers.Other_Controllers
{
    public class SceneLoadController : MonoBehaviour
    {
        private Slider _slider;
        private AsyncOperation _operation;
        private float _currentProgress;
        private float _currentTimeAtScene, _timeRetryingConnection;
        private MessageController _messageController;

        public void Start()
        {
            try
            {
                // We get the message controller for the init message
                GameObject initMessage = transform.Find(Settings.CanvasMessageObject).gameObject;
                _messageController = initMessage.GetComponent<MessageController>();
                _messageController.Disable();

                Button retryButton = _messageController.GetRetryButton();
                retryButton.onClick.AddListener(() => UnityAuth.InitUnityServices());

                // // We get the slider 
                GameObject sliderGameObject = GameObject.FindGameObjectWithTag(Settings.SliderTag);
                _slider = sliderGameObject.GetComponent<Slider>();
                SetInitValues();
                Util.Util.IsNull(sliderGameObject, "SceneLoadController/Start Slider is null");
                UnityAuth.InitUnityServices();
            }
            catch (Exception e)
            {
                GameLog.LogError(e.ToString());
            }
        }

        private void FixedUpdate()
        {
            if (!Util.Util.IsInternetReachable() && !Settings.DisableNetwork)
            {
                _messageController.Enable();
                _messageController.SetTextMessage(TextUI.ConnectionProblem);
                SetInitValues();
                return;
            }
            else
            {
                LoadGame();
            }
        }

        private void LoadGame()
        {
            try
            {
                _currentTimeAtScene += Time.fixedDeltaTime;
                _slider.value = _currentTimeAtScene / Settings.ScreenLoadTime;

                GameLog.Log("Loading-game: " +
                            (_operation != null ? Mathf.Approximately(_operation.progress, 0.9f) : "null") + " " +
                            (_currentTimeAtScene >= Settings.ScreenLoadTime) + " " +
                            PlayerData.IsUserSetted() + " ");

                // The user is loaded, the Service is init, the user is auth complete and 2 seconds passed
                if (_operation != null
                    && Mathf.Approximately(_operation.progress, 0.9f)
                    && _currentTimeAtScene >= Settings.ScreenLoadTime
                    && PlayerData.IsUserSetted())
                {
                    GameLog.Log("User name " + PlayerData.ToStringDebug());
                    _operation.allowSceneActivation = true;
                }
                else if (_operation == null)
                {
                    _operation = SceneManager.LoadSceneAsync(Settings.GameScene, LoadSceneMode.Single);
                    // if not will load scene before filling the load animation
                    _operation.allowSceneActivation = false;
                }
            }
            catch (Exception e)
            {
                GameLog.LogWarning(e.ToString());
            }
        }

        private void SetInitValues()
        {
            _slider.maxValue = 1;
            _slider.value = 0;
            _currentTimeAtScene = 0;
        }
    }
}