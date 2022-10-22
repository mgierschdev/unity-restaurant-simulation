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
    private Task<DocumentSnapshot> userData;
    private float MIN_TIME_LOADING = 4f; // Min time while laoding the screen
    private float currentTimeAtScene; // Current time at the screen

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
        GameLog.LogAll("UNITY DEBUG: Loading INIT");
        // We get the text inside the slider
        // GameObject sliderProgressObject = GameObject.Find(Settings.SliderProgress).gameObject;
        // Util.IsNull(sliderProgressObject, "SceneLoadController/Start sliderProgressObject is null");
        // sliderProgress = sliderProgressObject.GetComponent<TextMeshProUGUI>();

        // Init firebase
        firebase = new FirebaseLoad();
        GameLog.LogAll("Loading firebase");
        await firebase.InitFirebase();
        GameLog.LogAll("Loading Init firebase");
        Firestore.Init();
        // await firebase.InitAuth(); //ONLY in build physical
        //auth = firebase.GetFirebaseAuth();
        userData = Firestore.GetUserData(Settings.TEST_USER);//Settings.IsFirebaseEmulatorEnabled ? Settings.TEST_USER : auth.CurrentUser.UserId);
        GameLog.LogAll("Loading enabled " + userData.Status);

        // Loading next scene
        // Additional parameters: LoadSceneMode.Additive will not close current scene, default ==LoadSceneMode.Single will close current scene 
        // after the new one finishes loading.
        operation = SceneManager.LoadSceneAsync(Settings.GameScene);
        //Task task = new Task(LoadAsyncScene());
        operation.allowSceneActivation = false;
    }

    private void Update()
    {
        if (operation == null)
        {
            return;
        }

        GameLog.Log("UNITY DEBUG: Loading " + userData + " " );

        currentTimeAtScene += Time.fixedDeltaTime;
        currentProgress = Mathf.Lerp(currentTimeAtScene / MIN_TIME_LOADING, 0.10f, Time.fixedDeltaTime);
        slider.value = currentProgress;
        // sliderProgress.text = "LOADING " + Mathf.Ceil(currentProgress * 100).ToString() + "%";

        if (Mathf.Approximately(operation.progress, 0.9f) && userData != null && userData.IsCompleted && currentTimeAtScene > MIN_TIME_LOADING)
        {
            // Loads the queried data to the player game object
            PlayerData.LoadFirebaseDocument(userData.Result);
            operation.allowSceneActivation = true;
        }

        //Debug.Log(operation.progress + " " + userData.IsCompleted);
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