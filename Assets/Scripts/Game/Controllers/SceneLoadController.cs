using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This controller is attached to the -LoadScene-  not to the main game scene -World-
// Check the folder under Assets/Scenes
public class SceneLoadController : MonoBehaviour
{
    private TextMeshProUGUI sliderProgress;
    private Slider slider;
    private FirebaseLoad firebase;
    private FirebaseAuth auth;
    //Scene load
    private AsyncOperation operation;
    private float currentProgress;
    private Task<DocumentSnapshot> userData;
    private float MIN_TIME_LOADING = 4f; // Min time while laoding the screen
    private float currentTimeAtScene; // Current time at the screen

    //Loads Auth and user data
    public async void Start()
    {
        //Init 
        currentTimeAtScene = 0;

        //We get the slider 
        GameObject sliderGameObject = GameObject.FindGameObjectWithTag(Settings.SliderTag);
        slider = sliderGameObject.GetComponent<Slider>();
        slider.maxValue = 1;
        slider.value = 0;
        Util.IsNull(sliderGameObject, "SceneLoadController/Start Slider is null");

        //We get the text inside the slider
        GameObject sliderProgressObject = GameObject.Find(Settings.SliderProgress).gameObject;
        Util.IsNull(sliderProgressObject, "SceneLoadController/Start sliderProgressObject is null");
        sliderProgress = sliderProgressObject.GetComponent<TextMeshProUGUI>();

        //Init firebase
        firebase = new FirebaseLoad();
        await firebase.InitFirebase();
        Firestore.Init();
        //await firebase.InitAuth(); ONLY after build
        auth = firebase.GetFirebaseAuth();
        userData = Firestore.GetUserData(Settings.IsFirebaseEmulatorEnabled ? Settings.TEST_USER : auth.CurrentUser.UserId);

        //Loading next scene
        //Additional parameters: LoadSceneMode.Additive will not close current scene, default ==LoadSceneMode.Single will close current scene 
        //after the new one finishes loading.
        operation = SceneManager.LoadSceneAsync(Settings.GameScene);
        operation.allowSceneActivation = false;
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
        sliderProgress.text = "LOADINGD" + Mathf.Ceil(currentProgress * 100).ToString() + "%";

        if (Mathf.Approximately(operation.progress, 0.9f) && userData != null && userData.IsCompleted && currentTimeAtScene > MIN_TIME_LOADING)
        {
            // Loads the queried data to the player game object
            PlayerData.LoadFirebaseDocument(userData.Result);
            operation.allowSceneActivation = true;
        }
    }
}