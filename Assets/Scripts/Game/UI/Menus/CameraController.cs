using System;
using Game.Controllers.Menu_Controllers;
using Game.Grid;
using UnityEngine;

// This class handles the change of camera with the touch
// Attached to: MainCamera Object
namespace Game.UI.Menus
{
    public class CameraController : MonoBehaviour
    {
        private Vector3 _pointerDownStart, _direction;
        private readonly Vector3 _cameraGoToOffset = new Vector3(0, 0.5f, 0);
        private Vector3 _targetVectorPosition;
        private float _targetPosition, _targetOrthographicSize;
        private readonly float _cameraMovementSpeed = Settings.CameraMovementSpeed;
        private readonly float _zoomSpeed = Settings.ZoomSpeed;
        private readonly float _zoomSpeedPinch = Settings.ZoomSpeedPinch;
        private readonly float _minZoomSize = Settings.MinZoomSize;
        private readonly float _maxZoomSize = Settings.MaxZoomSize;
        private Camera _mainCamera;
        private MenuHandlerController _menuHandlerController;
        private bool _isPerspectiveHandTempDisabled;

        private void Start()
        {
            _mainCamera = Camera.main;

            if (_mainCamera == null)
            {
                throw new ArgumentNullException(nameof(_mainCamera));
            }

            _targetPosition = _mainCamera.orthographicSize;
            _pointerDownStart = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _direction = _pointerDownStart - _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // Menu controller
            GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
            _menuHandlerController = menuHandler.GetComponent<MenuHandlerController>();
            _targetVectorPosition = Vector3.zero;
            _targetOrthographicSize = 2.5f;
            _isPerspectiveHandTempDisabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            // Only if enabled in Settings or if no menu is open
            PerspectiveHand();
            UpdateGoTo();
        }

        private void UpdateGoTo()
        {
            if (_targetVectorPosition == Vector3.zero)
            {
                return;
            }

            if (Util.Util.IsAtDistanceWithObject(transform.position, _targetVectorPosition))
            {
                _targetVectorPosition = Vector3.zero;
                return;
            }

            _mainCamera.transform.position = Vector3.MoveTowards(transform.position, _targetVectorPosition,
                _cameraMovementSpeed * Time.fixedDeltaTime);
        }

        // Move the camera to the target Position
        public void GoTo(Vector3 position)
        {
            _targetVectorPosition = new Vector3(position.x, position.y, -1) + _cameraGoToOffset;
        }

        private void FollowTarget()
        {
            if (Util.Util.IsAtDistanceWithObject(transform.position, _targetVectorPosition) &&
                _mainCamera.orthographicSize >= _targetOrthographicSize)
            {
                _targetVectorPosition = Vector3.zero;
                return;
            }

            _mainCamera.orthographicSize = Mathf.Lerp(_mainCamera.orthographicSize, _targetOrthographicSize,
                _zoomSpeed * Time.unscaledDeltaTime);
        }

        private void PerspectiveHand()
        {
            if (!Settings.CameraPerspectiveHand || ObjectDraggingHandler.GetIsDraggingEnabled() ||
                _menuHandlerController.IsMenuOpen() || _isPerspectiveHandTempDisabled)
            {
                _pointerDownStart = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                return;
            }

            // This most be setted, before comparing with the next Input.GetMouseButtonDown(0)
            // It works for touches to becase Input.simulateMouseWithTouches is enabled by default
            if (Input.GetMouseButtonDown(0))
            {
                _pointerDownStart = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                _direction.y = _pointerDownStart.y - _mainCamera.ScreenToWorldPoint(Input.mousePosition).y;
                _direction.x = _pointerDownStart.x - _mainCamera.ScreenToWorldPoint(Input.mousePosition).x;
                _mainCamera.transform.position += new Vector3(_direction.x, _direction.y, 0);
                Vector3 transformPosition = transform.position;

                // then we clamp the value
                float clampX = Mathf.Clamp(transformPosition.x, Settings.CameraPerspectiveHandClampX[0],
                    Settings.CameraPerspectiveHandClampX[1]); // left and right
                float clampY = Mathf.Clamp(transformPosition.y, Settings.CameraPerspectiveHandClampY[0],
                    Settings.CameraPerspectiveHandClampY[1]); // units down, and up
                transform.position = new Vector3(clampX, clampY, transformPosition.z);
                // Debug.Log("ClampX " + transformPosition.x + " " + Settings.CameraPerspectiveHandClampX[0] + "," + Settings.CameraPerspectiveHandClampX[0] + " Clamped: " + clampX);
                // Debug.Log("ClampY " + transformPosition.y + " " + Settings.CameraPerspectiveHandClampY[0] + "," + Settings.CameraPerspectiveHandClampY[0] + " Clamped: " + clampY);
            }

            // TwoFingerZoom();

            //DEV: Camera zoom with mouse scroll wheel, only on desktop
            if (Input.mouseScrollDelta != Vector2.zero)
            {
                // Camera zoom
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                _targetPosition -= scroll * _zoomSpeed;
                _targetPosition = Mathf.Clamp(_targetPosition, _minZoomSize, _maxZoomSize);
                _mainCamera.orthographicSize = Mathf.Lerp(_mainCamera.orthographicSize, _targetPosition,
                    _zoomSpeed * Time.fixedDeltaTime);
            }
        }

        // 2 finger, Mobile zoom in/out
        public void TwoFingerZoom()
        {
            if (Input.touchCount > 1)
            {
                Touch firstFinger = Input.GetTouch(0);
                Touch secondFinger = Input.GetTouch(1);

                // If any of the 2 fingers moved
                if (firstFinger.phase == TouchPhase.Moved || secondFinger.phase == TouchPhase.Moved)
                {
                    float currentDistance = Vector3.Distance(firstFinger.position, secondFinger.position);
                    float previousDistance = Vector3.Distance(firstFinger.position - firstFinger.deltaPosition,
                        secondFinger.position - secondFinger.deltaPosition);

                    // if the distance increased meaning we are zomming in 
                    if (currentDistance > previousDistance)
                    {
                        _targetPosition -= Time.fixedDeltaTime * _zoomSpeedPinch;
                    }
                    else
                    {
                        _targetPosition += Time.fixedDeltaTime * _zoomSpeedPinch;
                    }

                    _targetPosition = Mathf.Clamp(_targetPosition, _minZoomSize, _maxZoomSize);
                    _mainCamera.orthographicSize = Mathf.Lerp(_mainCamera.orthographicSize, _targetPosition,
                        _zoomSpeedPinch * Time.fixedDeltaTime);
                }
            }
        }
    }
}