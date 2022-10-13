using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This controller is attached to the -LoadScene-  not to the main game scene -World-
// Check the folder under Assets/Scenes
public class SceneLoadController : MonoBehaviour
{
    private TextMeshProUGUI sliderProgress;
    private Slider slider;
    private FirebaseLoad firebase;
    private Firestore firestore;
    private FirebaseAuth auth;
    //Scene load
    private AsyncOperation operation;
    private float currentProgress;
    private Task<DocumentSnapshot> userData;

    //Loads Auth and user data
    public async void Start()
    {
        //We get the slider 
        GameObject sliderGameObject = GameObject.FindGameObjectWithTag(Settings.SliderTag);
        slider = sliderGameObject.GetComponent<Slider>();
        slider.maxValue = 1;
        slider.value = 0;
        Util.IsNull(sliderGameObject, "SceneLoadController/Start Slider is null");

        //We get the text inside the slider
        GameObject sliderProgressObject = sliderGameObject.transform.Find(Settings.SliderProgress).gameObject;
        Util.IsNull(sliderProgressObject, "SceneLoadController/Start sliderProgressObject is null");
        sliderProgress = sliderProgressObject.GetComponent<TextMeshProUGUI>();

        //Init firebase and auth
        if (!Settings.IsFirebaseEmulatorEnabled)
        {
            await firebase.InitAuth();
        }

        firestore = new Firestore();
        firebase = new FirebaseLoad();
        await firebase.InitFirebase();
        auth = firebase.GetFirebaseAuth();
        userData = firestore.GetUserData(Settings.IsFirebaseEmulatorEnabled ? Settings.TEST_USER : auth.CurrentUser.Email);

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

        currentProgress = Mathf.MoveTowards(operation.progress / 0.9f, 1, 0.20f * Time.deltaTime);
        slider.value = currentProgress;
        sliderProgress.text = (int)(currentProgress * 100) + " % ";

        if (Mathf.Approximately(currentProgress, 1) && userData.IsCompleted)
        {
            // Loads the queried data to the player game object
            PlayerData.LoadFirebaseDocument(userData.Result);
            operation.allowSceneActivation = true;
        }
    }
}