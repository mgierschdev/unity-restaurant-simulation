using System.Collections;
using System.Threading.Tasks;
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
        // Init 
        currentTimeAtScene = 0;

        // We get the slider 
        GameObject sliderGameObject = GameObject.FindGameObjectWithTag(Settings.SliderTag);
        slider = sliderGameObject.GetComponent<Slider>();
        slider.maxValue = 1;
        slider.value = 0;
        Util.IsNull(sliderGameObject, "SceneLoadController/Start Slider is null");

        firebase = new FirebaseLoad();
        await firebase.InitFirebase();

        // Init firebase
        if (!Settings.IsFirebaseEmulatorEnabled)
        {
            Firestore.Init(Settings.IsFirebaseEmulatorEnabled);
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            newUser = await auth.SignInAnonymouslyAsync();
            userData = await Firestore.GetUserData(newUser.UserId);
            if (userData == null)
            {
                PlayerData.SetNewUser(newUser);
                userData = await Firestore.GetUserData(PlayerData.EmailID);
            }
        }
        else
        {
            Firestore.Init(Settings.IsFirebaseEmulatorEnabled);
            PlayerData.EmailID = Settings.TEST_USER;
            userData = await Firestore.GetUserData(PlayerData.EmailID);
        }

        //.ContinueWith(task =>
        // {
        //     if (task.IsCanceled)
        //     {
        //         Debug.LogError("SignInAnonymouslyAsync was canceled.");
        //         return;
        //     }
        //     if (task.IsFaulted)
        //     {
        //         Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
        //         return;
        //     }
        //     newUser = task.Result;
        //     Debug.LogFormat("User signed in successfully: {0} ({1})",
        //         newUser.DisplayName, newUser.UserId);
        // });

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
            // Loads the queried data to the player game object
            PlayerData.LoadFirebaseDocument(userData);
            operation.allowSceneActivation = true;
        }
    }

    private IEnumerator LoadAsyncScene()
    {
        operation = SceneManager.LoadSceneAsync(Settings.GameScene);
        operation.allowSceneActivation = false;
        Debug.Log("starting scene coroutine " + Settings.GameScene);
        while (!operation.isDone)
        {
            // Debug.Log("Progress "+operation.progress);
            yield return null;
        }
    }
}