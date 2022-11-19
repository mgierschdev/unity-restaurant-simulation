using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This controller is attached to the -LoadScene-  not to the main game scene -World-
// Check the folder under Assets/Scenes
public class SceneLoadController : MonoBehaviour
{
    private Slider slider;
    private FirebaseLoad firebase;
    private FirebaseAuth auth;
    // Scene load
    private AsyncOperation operation;
    private float currentProgress, MIN_TIME_LOADING = Settings.ScreenLoadTime, currentTimeAtScene;
    private DocumentSnapshot userData;
    private FirebaseUser newUser;

    // Loads Auth and user data
    public async void Awake()
    {
        // We get the slider 
        GameObject sliderGameObject = GameObject.FindGameObjectWithTag(Settings.SliderTag);
        slider = sliderGameObject.GetComponent<Slider>();
        slider.maxValue = 1;
        slider.value = 0;
        currentTimeAtScene = 0;
        Util.IsNull(sliderGameObject, "SceneLoadController/Start Slider is null");

        // Firebase Auth
        try
        {
            firebase = new FirebaseLoad();
            await firebase.InitFirebase();
            Firestore.Init(Settings.IsFirebaseEmulatorEnabled);
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;

            // If emulator is not enabled we init the anon auth, only if there is network connection
            if (!Settings.IsFirebaseEmulatorEnabled && Util.IsInternetReachable())
            {
                // Critical exception in SignInAnonymouslyAsync if offline
                await auth.SignInAnonymouslyAsync();
            }

            PlayerData.InitUser(auth);
            operation = SceneManager.LoadSceneAsync(Settings.GameScene);
            // if not will load scene before filling the load animation
            operation.allowSceneActivation = false;
        }
        catch (SystemException e)
        {
            GameLog.LogWarning(e.ToString());
        }
    }

    private void FixedUpdate()
    {
        currentTimeAtScene += Time.fixedDeltaTime;
        slider.value = currentTimeAtScene / MIN_TIME_LOADING;

        if (operation != null
        && Mathf.Approximately(operation.progress, 0.9f)
        && currentTimeAtScene >= MIN_TIME_LOADING
        && PlayerData.GetFirebaseGameUser() != null)
        {
            operation.allowSceneActivation = true;
        }
    }
}