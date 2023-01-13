using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This controller is attached to the -LoadScene-  not to the main game scene -World-
// Check the folder under Assets/Scenes
public class SceneLoadController : MonoBehaviour
{
    private Slider slider;
    private AsyncOperation operation;
    private float currentProgress, MIN_TIME_LOADING = Settings.ScreenLoadTime, currentTimeAtScene, timeRetryingConnection;
    private MessageController messageController;

    public void Awake()
    {
        // We get the message controller for the init message
        GameObject initMessage = transform.Find(Settings.CanvasMessageObject).gameObject;
        messageController = initMessage.GetComponent<MessageController>();

        Button retryButton = messageController.GetRetryButton();
        retryButton.onClick.AddListener(InitUnity);
        //messageController.Disable();

        // We get the slider 
        GameObject sliderGameObject = GameObject.FindGameObjectWithTag(Settings.SliderTag);
        slider = sliderGameObject.GetComponent<Slider>();
        SetInitValues();
        Util.IsNull(sliderGameObject, "SceneLoadController/Start Slider is null");

        // Init unity
        InitUnity();

        messageController.Enable();

        //This will avoid destroying the current scene including the event system while a new scene is being loaded
        //DontDestroyOnLoad(this.gameObject);
    }

    private void InitUnity()
    {
        //we init Unity game services 
        try
        {
            if (!Util.IsInternetReachable())
            {
                messageController.Enable();
                messageController.SetTextMessage(TextUI.ConnectionProblem);
            }
            else
            {
                UnityAuth.InitUnityServices();
            }
        }
        catch (SystemException e)
        {
            GameLog.LogWarning(e.ToString());
        }
    }

    private void LoadGame()
    {
        try
        {
            currentTimeAtScene += Time.fixedDeltaTime;
            slider.value = currentTimeAtScene / MIN_TIME_LOADING;

            GameLog.Log("Loading: " + (operation != null ? operation.progress : "null") + " " + (currentTimeAtScene >= MIN_TIME_LOADING) + " " + PlayerData.GetDataGameUser() + " " + UnityAuth.GetIsUserLogged());

            // The user is loaded, the Service is init, the user is auth complete and 2 seconds passed
            if (operation != null
            && Mathf.Approximately(operation.progress, 0.9f)
            && currentTimeAtScene >= MIN_TIME_LOADING
            && PlayerData.GetDataGameUser() != null
            && UnityAuth.GetIsUserLogged())
            {
                operation.allowSceneActivation = true;
            }
            else if (operation == null)
            {
                operation = SceneManager.LoadSceneAsync(Settings.GameScene);
                // if not will load scene before filling the load animation
                operation.allowSceneActivation = false;
            }
        }
        catch (Exception e)
        {
            GameLog.LogWarning(e.ToString());
        }
    }

    private void FixedUpdate()
    {
        if (!Util.IsInternetReachable() || UnityAuth.Retrying || UnityAuth.AuthFailed)
        {
            SetInitValues();
            return;
        }
        else
        {
            LoadGame();
        }
    }

    private void SetInitValues()
    {
        slider.maxValue = 1;
        slider.value = 0;
        currentTimeAtScene = 0;
    }
}