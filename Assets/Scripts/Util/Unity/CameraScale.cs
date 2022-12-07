using UnityEngine;

public class CameraScale
{
    private float currentWindowAspectRatio, targetSpectRatio;
    private Camera mainCamera;

    private void ScaleCamera()
    {
        currentWindowAspectRatio = (float)Screen.width / (float)Screen.height;
        targetSpectRatio = (float)Settings.ConstDefaultCameraWidth / (float)Settings.ConstDefaultCameraHeight;
        float newScaleHeight = currentWindowAspectRatio / targetSpectRatio;

        if (newScaleHeight > 1)
        {
            mainCamera.orthographicSize = Settings.ConstDefaultCameraOrthographicsize - (newScaleHeight - 1) * Settings.ConstDefaultCameraOrthographicsize;
        }
    }
}