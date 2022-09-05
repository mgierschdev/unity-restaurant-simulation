using UnityEngine;

// This class handles the change of camera with the touch
// Attached to: MainCamera Object
public class CameraController : MonoBehaviour
{
    private Vector3 touchStart;
    private Vector3 direction;
    public float interpolation = Settings.CAMERA_FOLLOW_INTERPOLATION;
    // MouseScroll zoom 
    private float targetPosition;
    private const float ZOOM_SPEED = 35;
    private const float MIN_ZOOM_SIZE = 1;
    private const float MAX_ZOOM_SIZE = 5;
    // Menu Controller
    private MenuHandlerController menuHandlerController;
    // Main Camera
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        targetPosition = mainCamera.orthographicSize;
        touchStart = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        direction = touchStart - mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // playerGameObject = GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG);
        GameObject parentCanvas = GameObject.Find(Settings.CONST_CANVAS_PARENT_MENU);
        menuHandlerController = parentCanvas.GetComponent<MenuHandlerController>();
        // Util.IsNull(playerGameObject, "CameraController/PlayerController is null");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Only if enabled in Settings or if no menu is open
        PerspectiveHand();

        // Follow Player
        // FollowPlayer();
    }

    // private void FollowPlayer()
    // {
    //     if (Settings.CAMERA_FOLLOW_PLAYER)
    //     {
    //         Vector3 playerPosition = new Vector3(playerGameObject.transform.position.x, playerGameObject.transform.position.y, transform.position.z);
    //         Vector3 smoothedPosition = Vector3.Lerp(transform.position, playerPosition, interpolation);
    //         transform.position = smoothedPosition;
    //     }
    // }

    private void PerspectiveHand()
    {
        if (!Settings.CAMERA_PERSPECTIVE_HAND || menuHandlerController.IsGamePaused())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            touchStart = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            direction.y = touchStart.y - mainCamera.ScreenToWorldPoint(Input.mousePosition).y;
            direction.x = touchStart.x - mainCamera.ScreenToWorldPoint(Input.mousePosition).x;
            mainCamera.transform.position += new Vector3(direction.x, direction.y, 0);
            Vector3 transformPosition = transform.position;

            // then we clamp the value
            float clampX = Mathf.Clamp(transformPosition.x, Settings.CAMERA_PERSPECTIVE_HAND_CLAMP_X[0],
                Settings.CAMERA_PERSPECTIVE_HAND_CLAMP_X[1]); // left and right
            float clampY = Mathf.Clamp(transformPosition.y, Settings.CAMERA_PERSPECTIVE_HAND_CLAMP_Y[0],
                Settings.CAMERA_PERSPECTIVE_HAND_CLAMP_Y[1]); // units down, and up
            transform.position = new Vector3(clampX, clampY, transformPosition.z);
        }
        else if (Input.mouseScrollDelta != Vector2.zero)
        {
            // Camera zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            targetPosition -= scroll * ZOOM_SPEED;
            targetPosition = Mathf.Clamp(targetPosition, MIN_ZOOM_SIZE, MAX_ZOOM_SIZE);
            mainCamera.orthographicSize = Mathf.MoveTowards(mainCamera.orthographicSize, targetPosition, ZOOM_SPEED * Time.deltaTime);
        }
    }
}