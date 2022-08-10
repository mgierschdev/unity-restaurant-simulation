using UnityEngine;

// This class handles the change of camera with the touch
// Attached to: MainCamera Object
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 touchStart;
    private Vector3 direction;
    private GameObject playerGameObject;

    // Camera follow player, for smothing camera movement 
    public float interpolation = Settings.CAMERA_FOLLOW_INTERPOLATION;

    // MouseScroll zoom 
    private float targetPosition;
    private float zoomValue;
    private float zoomSpeed = 35;
    private float minZoomSize = 1;
    private float maxZoomSize = 5;

    // Menu Controller
    private MenuHandlerController menuHandlerController;

    private void Start()
    {
        targetPosition = Camera.main.orthographicSize;
        touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerGameObject = GameObject.FindGameObjectWithTag(Settings.PREFAB_PLAYER);
        GameObject parentCanvas = GameObject.Find(Settings.CONST_CANVAS_PARENT_MENU);
        menuHandlerController = parentCanvas.GetComponent<MenuHandlerController>();

        if (playerGameObject == null)
        {
            Debug.LogWarning("CameraController/PlayerController is null");
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Only if enabled in Settings or if no menu is open
        PerspectiveHand();

        // Follow Player
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (Settings.CAMERA_FOLLOW_PLAYER)
        {
            Vector3 playerPosition = new Vector3(playerGameObject.transform.position.x, playerGameObject.transform.position.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, playerPosition, interpolation);
            transform.position = smoothedPosition;
        }
    }

    private void PerspectiveHand()
    {
        if (Settings.CAMERA_PERSPECTIVE_HAND && !menuHandlerController.IsGamePaused())
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                direction.y = touchStart.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
                direction.x = touchStart.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                Camera.main.transform.position += new Vector3(direction.x, direction.y, 0);

                // then we clamp the value
                float clampX = Mathf.Clamp(transform.position.x, Settings.CAMERA_PERSPECTIVE_HAND_CLAMP_X[0], Settings.CAMERA_PERSPECTIVE_HAND_CLAMP_X[1]); // left and right
                float clampY = Mathf.Clamp(transform.position.y, Settings.CAMERA_PERSPECTIVE_HAND_CLAMP_Y[0], Settings.CAMERA_PERSPECTIVE_HAND_CLAMP_Y[1]); // units down, and up
                transform.position = new Vector3(clampX, clampY, transform.position.z);
            }
            else if (Input.mouseScrollDelta != Vector2.zero)
            {
                // Camera zoom
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                targetPosition -= scroll * zoomSpeed;
                targetPosition = Mathf.Clamp(targetPosition, minZoomSize, maxZoomSize);
                Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetPosition, zoomSpeed * Time.deltaTime);
            }
        }
    }
}