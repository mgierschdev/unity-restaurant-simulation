using UnityEngine;

public class CameraScale : MonoBehaviour
{
    private float currentWindowAspectRatio;
    private float targetSpectRatio;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        
        ScaleCamera();
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
        if (mainCamera)
        {
            ScaleCamera();
        }
        #endif
    }

    // Failing with Samsumg foldable devices and some devices by half unit at the end of the screen
    // (LG Nexus4, Ipad Pro 12.9/11 2units at the end), Ipad Mini, Ipad Air, 
    private void ScaleCamera()
    {
        currentWindowAspectRatio = (float)Screen.width / (float)Screen.height;
        targetSpectRatio = (float)Settings.CONST_DEFAULT_CAMERA_WIDTH / (float)Settings.CONST_DEFAULT_CAMERA_HEIGHT;
        // should be scaled to this ammount
        float newScaleHeight = currentWindowAspectRatio / targetSpectRatio;
        if (newScaleHeight > 1)
        {
            mainCamera.orthographicSize = Settings.CONST_DEFAULT_CAMERA_ORTHOGRAPHICSIZE - (newScaleHeight - 1) * Settings.CONST_DEFAULT_CAMERA_ORTHOGRAPHICSIZE ;
        }
    }
}