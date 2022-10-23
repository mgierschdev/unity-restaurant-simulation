using System;
using System.Collections;
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
        firebase = new FirebaseLoad();
        await firebase.InitFirebase();

        try
        {
            Firestore.Init(Settings.IsFirebaseEmulatorEnabled);
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;

            // Init firebase
            if (!Settings.IsFirebaseEmulatorEnabled)
            {
                // Firestore.Init(Settings.IsFirebaseEmulatorEnabled);
                // FirebaseAuth auth = FirebaseAuth.DefaultInstance;
                // GameLog.LogAll("Current user " + auth.CurrentUser);
                // if (auth.CurrentUser != null)
                // {
                //     GameLog.LogAll("User loggedin");
                // }
                // else
                // {
                //     GameLog.LogAll("User is not logged in");
                // }

                // newUser = await auth.SignInAnonymouslyAsync(); // Critical exception here
                // PlayerData.CreateNewUser(newUser);
                // userData = await Firestore.GetUserData(newUser.UserId);

                // if (userData == null)
                // {
                //     PlayerData.CreateNewUser(newUser);
                // }
            }
            else
            {
                PlayerData.InitUser(auth.CurrentUser);
            }
        }
        catch (SystemException e)
        {
            GameLog.LogAll(e.ToString());
        }

        operation = SceneManager.LoadSceneAsync(Settings.GameScene);
    }

    private void Update()
    {
        if (operation == null)
        {
            return;
        }

        currentTimeAtScene += Time.fixedDeltaTime;
        currentProgress = Mathf.Lerp(currentTimeAtScene / MIN_TIME_LOADING, 0.10f, Time.fixedDeltaTime);
        slider.value = currentProgress;

        if (Mathf.Approximately(operation.progress, 0.9f)
        && currentTimeAtScene > MIN_TIME_LOADING
        && userData != null)
        {
            operation.allowSceneActivation = true;
        }
    }
}