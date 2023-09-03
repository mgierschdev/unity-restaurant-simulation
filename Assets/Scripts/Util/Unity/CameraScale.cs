using UnityEngine;

namespace Util.Unity
{
    public class CameraScale
    {
        private float _currentWindowAspectRatio, _targetSpectRatio;
        private Camera _mainCamera;

        private void ScaleCamera()
        {
            _currentWindowAspectRatio = Screen.width / (float)Screen.height;
            _targetSpectRatio = Settings.ConstDefaultCameraWidth / (float)Settings.ConstDefaultCameraHeight;
            float newScaleHeight = _currentWindowAspectRatio / _targetSpectRatio;

            if (newScaleHeight > 1)
            {
                _mainCamera.orthographicSize = Settings.ConstDefaultCameraOrthographicsize - (newScaleHeight - 1) * Settings.ConstDefaultCameraOrthographicsize;
            }
        }
    }
}