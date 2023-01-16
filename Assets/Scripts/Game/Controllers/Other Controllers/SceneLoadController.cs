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

        //
        Debug.Log("Init unity");

        // We get the message controller for the init message
        GameObject initMessage = transform.Find(Settings.CanvasMessageObject).gameObject;
        messageController = initMessage.GetComponent<MessageController>();
        messageController.Disable();

        Button retryButton = messageController.GetRetryButton();
        retryButton.onClick.AddListener(() => UnityAuth.InitUnityServices());

        // We get the slider 
        GameObject sliderGameObject = GameObject.FindGameObjectWithTag(Settings.SliderTag);
        slider = sliderGameObject.GetComponent<Slider>();
        SetInitValues();
        Util.IsNull(sliderGameObject, "SceneLoadController/Start Slider is null");

        // Init unity
        UnityAuth.InitUnityServices();
    }

    private void FixedUpdate()
    {
        if (!Util.IsInternetReachable() && !Settings.DisableNetwork)
        {
            messageController.Enable();
            messageController.SetTextMessage(TextUI.ConnectionProblem);
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
            currentTimeAtScene += Time.fixedDeltaTime;
            slider.value = currentTimeAtScene / MIN_TIME_LOADING;

            Debug.Log("Loading-game: " + (operation != null ? Mathf.Approximately(operation.progress, 0.9f) : "null") + " " +
            (currentTimeAtScene >= MIN_TIME_LOADING) + " " +
            (PlayerData.GetDataGameUser() != null) + " ");

            // The user is loaded, the Service is init, the user is auth complete and 2 seconds passed
            if (operation != null
            && Mathf.Approximately(operation.progress, 0.9f)
            && currentTimeAtScene >= MIN_TIME_LOADING
            && PlayerData.GetDataGameUser() != null)
            {
                GameLog.Log("Enabling scene");
                Debug.Log("User name " + PlayerData.ToString());

                operation.allowSceneActivation = true;
            }
            else if (operation == null)
            {
                operation = SceneManager.LoadSceneAsync(Settings.GameScene, LoadSceneMode.Single);
                // if not will load scene before filling the load animation
                operation.allowSceneActivation = false;
            }
        }
        catch (Exception e)
        {
            GameLog.LogWarning(e.ToString());
        }
    }

    private void SetInitValues()
    {
        slider.maxValue = 1;
        slider.value = 0;
        currentTimeAtScene = 0;
    }
}