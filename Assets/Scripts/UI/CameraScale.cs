using UnityEngine;
using System.Collections;

public class CameraScale : MonoBehaviour
{
    private float currentWindowAspectRatio;
    private float targetSpectRatio;
    private Camera mainCamera;
    private float orthographicSize = Settings.CONST_DEFAULT_CAMERA_ORTHOGRAPHICSIZE;

    private void Awake()
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


    private void ScaleCamera()
    {
        currentWindowAspectRatio = (float)Screen.width / (float)Screen.height;
        targetSpectRatio = (float)Settings.CONST_DEFAULT_CAMERA_WIDTH / (float)Settings.CONST_DEFAULT_CAMERA_HEIGHT;
        // should be scaled to this ammount
        float newScaleHeight = currentWindowAspectRatio / targetSpectRatio;

        GameObject go = GameObject.Find(Settings.CONST_GAME_BACKGROUND);
        SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();
        Vector3 min = sprite.bounds.min;
        Vector3 max = sprite.bounds.max;

        Vector3 screenMin = mainCamera.WorldToScreenPoint(min);
        Vector3 screenMax = mainCamera.WorldToScreenPoint(max);

        Vector3 screenWorldPointMin = mainCamera.ScreenToWorldPoint(screenMin);
        Vector3 screenWorldPointMax = mainCamera.ScreenToWorldPoint(screenMin);

        Debug.Log(screenMin + " World points " + screenMax);
        Debug.Log(screenWorldPointMin + " ScreenPoints " + screenMax);

        Debug.Log(newScaleHeight + " " + currentWindowAspectRatio + " " + targetSpectRatio+" ");
        var rec = mainCamera.rect;

        if (newScaleHeight < 1)
        { // if it is smaller we let the user scroll the bigger screen in all directions 
            //Debug.Log("Scaling");
            //rec.width = 1;
            //rec.height = newScaleHeight;
            //rec.x = 0;
            //rec.y = (1 - newScaleHeight) / 2;
        }
        else if (newScaleHeight > 1)
        {
            //var scaleWith = 1 / newScaleHeight;
            //rec.width = newScaleHeight;
            //rec.height = 1;
            //rec.x = (1 - scaleWith) / 2;
            //rec.y = 0;


            mainCamera.orthographicSize = Settings.CONST_DEFAULT_CAMERA_ORTHOGRAPHICSIZE - (newScaleHeight - 1) * Settings.CONST_DEFAULT_CAMERA_ORTHOGRAPHICSIZE ;

        }
    }
}
