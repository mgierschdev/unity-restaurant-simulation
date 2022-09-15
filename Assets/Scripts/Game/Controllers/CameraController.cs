using UnityEngine;

// This class handles the change of camera with the touch
// Attached to: MainCamera Object
public class CameraController : MonoBehaviour
{
    private Vector3 touchStart;
    private Vector3 direction;
    public float interpolation = Settings.CameraFollowInterpolation;
    // MouseScroll zoom 
    private float targetPosition;
    private const float ZOOM_SPEED = 35;
    private const float MIN_ZOOM_SIZE = 1;
    private const float MAX_ZOOM_SIZE = 5;
    // Menu Controller
    private MenuHandlerController menuHandlerController;
    // Main Camera
    private Camera mainCamera;
    private GridController gridController;
    private Vector3 targetVectorPosition;
    private float targetOrthographicSize;
    private MenuHandlerController menuController;

    private void Start()
    {
        mainCamera = Camera.main;
        targetPosition = mainCamera.orthographicSize;
        touchStart = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        direction = touchStart - mainCamera.ScreenToWorldPoint(Input.mousePosition);
        GameObject parentCanvas = GameObject.Find(Settings.ConstCanvasParentMenu);
        menuHandlerController = parentCanvas.GetComponent<MenuHandlerController>();
        GameObject gameGridObject = GameObject.Find(Settings.GameGrid).gameObject;
        gridController = gameGridObject.GetComponent<GridController>();
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        menuController = menuHandler.GetComponent<MenuHandlerController>();
        targetVectorPosition = Vector3.zero;
        targetOrthographicSize = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        // Only if enabled in Settings or if no menu is open
        PerspectiveHand();
    }

    // Move the camera to the target Position
    public void GoTo(Vector3 position)
    {
        targetVectorPosition = position;
    }

    private void FollowTarget()
    {
        if (Util.IsAtDistanceWithObject(transform.position, targetVectorPosition) && mainCamera.orthographicSize >= targetOrthographicSize)
        {
            targetVectorPosition = Vector3.zero;
            return;
        }
        // Vector3 tPosition = new Vector3(targetVectorPosition.x, targetVectorPosition.y, transform.position.z);
        // Vector3 smoothedPosition = Vector3.Lerp(transform.position, tPosition, interpolation);
        // transform.position = smoothedPosition;
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, ZOOM_SPEED * Time.unscaledDeltaTime);
    }

    private void PerspectiveHand()
    {
        if (!Settings.CameraPerspectiveHand || gridController.DraggingObject || menuController.IsMenuOpen())
        {
            return;
        }

        // This most be setted before comparing with the next Input.GetMouseButtonDown(0)
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
            float clampX = Mathf.Clamp(transformPosition.x, Settings.CameraPerspectiveHandClampX[0],
                Settings.CameraPerspectiveHandClampX[1]); // left and right
            float clampY = Mathf.Clamp(transformPosition.y, Settings.CameraPerspectiveHandClampY[0],
                Settings.CameraPerspectiveHandClampY[1]); // units down, and up
            transform.position = new Vector3(clampX, clampY, transformPosition.z);
        }
        else if (Input.mouseScrollDelta != Vector2.zero)
        {
            // Camera zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            targetPosition -= scroll * ZOOM_SPEED;
            targetPosition = Mathf.Clamp(targetPosition, MIN_ZOOM_SIZE, MAX_ZOOM_SIZE);
            // fixedDeltaTime works with the time scale = 0
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetPosition, ZOOM_SPEED * Time.fixedDeltaTime);
        }
    }
}