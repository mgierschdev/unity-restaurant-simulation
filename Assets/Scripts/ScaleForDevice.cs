using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls the screen resolutions
// so the game or the app will be available in all Android screens
public class ScaleForDevice : MonoBehaviour
{

    public float resolutionFactor;
    float _height = Screen.height;
    float _weight = Screen.width;
    float scaleFactorW;
    float ScaleFactoeH;

    void Awake()
    {

        float _height = Screen.height;
        float _weight = Screen.width;
        float scaleFactorW = _weight / 1024;
        float scaleFactorH = _height / 768;
        resolutionFactor = scaleFactorW / scaleFactorH;

        #if UNITY_ANDROID || UNITY_EDITOR

                if (scaleFactorW > scaleFactorH)
                {
                    this.transform.localScale = new Vector3(scaleFactorW / scaleFactorH, 1, scaleFactorW / scaleFactorH);
                }
        #endif
    }

}
