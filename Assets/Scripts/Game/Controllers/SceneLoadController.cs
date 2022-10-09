using System.Collections;
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

    void Start()
    {
        //We get the slider 
        GameObject sliderGameObject = GameObject.FindGameObjectWithTag(Settings.SliderTag);
        slider = sliderGameObject.GetComponent<Slider>();
        Util.IsNull(sliderGameObject, "SceneLoadController/Start Slider is null");

        //We get the text inside the slider
        GameObject sliderProgressObject = sliderGameObject.transform.Find(Settings.SliderProgress).gameObject;
        Util.IsNull(sliderProgressObject, "SceneLoadController/Start sliderProgressObject is null");
        sliderProgress = sliderProgressObject.GetComponent<TextMeshProUGUI>();

        //Default values
        slider.maxValue = 1;
        slider.value = 0;

        //We load the Scene managment 
        LoadSceneAsync(Settings.GameScene);
    }

    private void LoadSceneAsync(string scene)
    {
        StartCoroutine(LoadAsync(scene));
    }

    private IEnumerator LoadAsync(string scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene); // LoadSceneMode.Additive, default Single

        while (!operation.isDone)
        {
            slider.value = operation.progress;
            sliderProgress.text = (operation.progress * 100) + " % ";
            yield return null;
        }
    }
}
