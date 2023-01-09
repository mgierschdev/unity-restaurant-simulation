using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using IEnumerator = System.Collections.IEnumerator;

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
        messageController.Disable();

        // We get the slider 
        GameObject sliderGameObject = GameObject.FindGameObjectWithTag(Settings.SliderTag);
        slider = sliderGameObject.GetComponent<Slider>();
        slider.maxValue = 1;
        slider.value = 0;
        currentTimeAtScene = 0;
        Util.IsNull(sliderGameObject, "SceneLoadController/Start Slider is null");

        //we init Unity game services 
        try
        {
            UnityAuth.InitUnityServices();
        }
        catch (SystemException e)
        {
            GameLog.LogWarning(e.ToString());
        }
    }

    private void FixedUpdate()
    {
        if (!Util.IsInternetReachable())
        {
            messageController.Enable();
            messageController.SetTextMessage(TextUI.ConnectionProblem);
            return;
        }

        currentTimeAtScene += Time.fixedDeltaTime;
        slider.value = currentTimeAtScene / MIN_TIME_LOADING;

        GameLog.Log("Loading: " + (operation != null ? operation.progress : "null") + " " + (currentTimeAtScene >= MIN_TIME_LOADING) + " ");

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
}