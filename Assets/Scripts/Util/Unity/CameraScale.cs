using UnityEngine;

namespace Util.Unity
{
    /**
     * Problem: Adjust camera orthographic size based on screen aspect.
     * Goal: Maintain consistent framing across resolutions.
     * Approach: Compute aspect ratio and update orthographic size.
     * Time: O(1) per call.
     * Space: O(1).
     */
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
