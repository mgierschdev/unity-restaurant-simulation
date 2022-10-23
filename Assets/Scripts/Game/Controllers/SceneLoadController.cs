using System;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This controller is attached to the -LoadScene-  not to the main game scene -World-
// Check the folder under Assets/Scenes
public class SceneLoadController : MonoBehaviour
{
    //private TextMeshProUGUI sliderProgress;
    private Slider slider;
    private FirebaseLoad firebase;
    private FirebaseAuth auth;
    // Scene load
    private AsyncOperation operation;
    private float currentProgress;
    private DocumentSnapshot userData;
    private float MIN_TIME_LOADING = 4f; // Min time while laoding the screen
    private float currentTimeAtScene; // Current time at the screen
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
        }
        catch (SystemException e)
        {
            GameLog.LogAll(e.ToString());
        }
    }

    private void Update()
    {
        if (operation == null)
        {
            return;
        }

        currentTimeAtScene += Time.fixedDeltaTime;
        currentProgress = Mathf.Lerp(currentTimeAtScene / MIN_TIME_LOADING, 0.20f, Time.fixedDeltaTime);
        slider.value = currentProgress;

        if (Mathf.Approximately(operation.progress, 0.9f)
        && currentTimeAtScene > MIN_TIME_LOADING
        && PlayerData.GetFirebaseGameUser() != null)
        {
            operation.allowSceneActivation = true;
        }
    }
}