using UnityEngine;

// This class handles the change of camera with the touch
// Attached to: MainCamera Object
public class CameraController : MonoBehaviour
{
    private Vector3 pointerDownStart;
    private Vector3 direction;
    // MouseScroll zoom 
    private float targetPosition;
    private const float ZOOM_SPEED = 35;
    private const float ZOOM_SPEED_PINCH = 8f;
    private const float MIN_ZOOM_SIZE = 1;
    private const float MAX_ZOOM_SIZE = 5;
    private const float MIN_TIME_TO_ENABLE_PERSPECTIVE_HAND = 0.1f;//Adding a small delay to perspective hand
    // Main Camera
    private Camera mainCamera;
    private Vector3 targetVectorPosition;
    private float targetOrthographicSize;
    // Menu Controller
    private MenuHandlerController menuHandlerController;
    // private ClickController clickController;
    // private float interpolation = Settings.CameraFollowInterpolation;

    private void Start()
    {
        mainCamera = Camera.main;
        targetPosition = mainCamera.orthographicSize;
        pointerDownStart = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        direction = pointerDownStart - mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // Menu controller
        GameObject menuHandler = GameObject.Find(Settings.ConstCanvasParentMenu).gameObject;
        menuHandlerController = menuHandler.GetComponent<MenuHandlerController>();
        // Click controller
        // GameObject cController = GameObject.FindGameObjectWithTag(Settings.ConstParentGameObject);
        // clickController = cController.GetComponent<ClickController>();
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
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, ZOOM_SPEED * Time.unscaledDeltaTime);
    }

    private void PerspectiveHand()
    {
        if (!Settings.CameraPerspectiveHand || BussGrid.GetDragginObject() || menuHandlerController.IsMenuOpen())
        {
            return;
        }

        // This most be setted, before comparing with the next Input.GetMouseButtonDown(0)
        // It works for touches to becase Input.simulateMouseWithTouches is enabled by default
        if (Input.GetMouseButtonDown(0))
        {
            pointerDownStart = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            direction.y = pointerDownStart.y - mainCamera.ScreenToWorldPoint(Input.mousePosition).y;
            direction.x = pointerDownStart.x - mainCamera.ScreenToWorldPoint(Input.mousePosition).x;
            mainCamera.transform.position += new Vector3(direction.x, direction.y, 0);
            Vector3 transformPosition = transform.position;

            // then we clamp the value
            float clampX = Mathf.Clamp(transformPosition.x, Settings.CameraPerspectiveHandClampX[0], Settings.CameraPerspectiveHandClampX[1]); // left and right
            float clampY = Mathf.Clamp(transformPosition.y, Settings.CameraPerspectiveHandClampY[0], Settings.CameraPerspectiveHandClampY[1]); // units down, and up
            transform.position = new Vector3(clampX, clampY, transformPosition.z);
        }

        if (Input.touchCount > 1) // Mobile pinch
        {
            Touch firstFinger = Input.GetTouch(0);
            Touch secondFinger = Input.GetTouch(1);

            // If any of the 2 fingers moved
            if (firstFinger.phase == TouchPhase.Moved || secondFinger.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector3.Distance(firstFinger.position, secondFinger.position);
                float previousDistance = Vector3.Distance(firstFinger.position - firstFinger.deltaPosition, secondFinger.position - secondFinger.deltaPosition);

                // if the distance increased meaning we are zomming in 
                if (currentDistance > previousDistance)
                {
                    targetPosition -= Time.fixedDeltaTime * ZOOM_SPEED_PINCH;
                }
                else
                {
                    targetPosition += Time.fixedDeltaTime * ZOOM_SPEED_PINCH;
                }
                targetPosition = Mathf.Clamp(targetPosition, MIN_ZOOM_SIZE, MAX_ZOOM_SIZE);
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetPosition, ZOOM_SPEED_PINCH * Time.fixedDeltaTime);
            }
        }

        if (Input.mouseScrollDelta != Vector2.zero)
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