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
    private float currentProgress, MIN_TIME_LOADING = Settings.ScreenLoadTime, currentTimeAtScene;
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
            PlayerData.InitUser();
            UnityAuth.InitUnityServices();
        }
        catch (SystemException e)
        {
            GameLog.LogWarning(e.ToString());
        }
    }

    private void FixedUpdate()
    {
        Debug.Log("Network connection: " + Util.IsInternetReachable());

        if (!Util.IsInternetReachable())
        {
            // TODO: show popup
            messageController.Enable();
            return;
        }

        currentTimeAtScene += Time.fixedDeltaTime;
        slider.value = currentTimeAtScene / MIN_TIME_LOADING;

        if (operation != null
        && Mathf.Approximately(operation.progress, 0.9f)
        && currentTimeAtScene >= MIN_TIME_LOADING
        && PlayerData.GetDataGameUser() != null
        && UnityAuth.IsUnityServiceInitialized()
        && UnityAuth.GetIsUserLogged())//TODO: re-try connection after an interval of seconds
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