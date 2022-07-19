using UnityEngine;

// This class handles the change of camera with the touch
// Attached to: MainCamera Object
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 touchStart;
    [SerializeField]
    private Vector3 direction;
    private PlayerController playerController;
    private GameObject playerGameObject;

    // Camera follow player, for smothing camera movement 
    public float speed = 10f;

    private void Start()
    {
        touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerGameObject = GameObject.FindGameObjectWithTag(Settings.PREFAB_PLAYER);

        if (playerGameObject != null)
        {
            playerController = playerGameObject.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogWarning("CameraController/PlayerController is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Only if enabled in Settings
        PerspectiveHand();

        // Follow Player
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (Settings.CAMERA_FOLLOW_PLAYER)
        {
            Vector3 playerPosition = new Vector3(playerGameObject.transform.position.x, playerGameObject.transform.position.y, transform.position.z);
            Debug.Log(playerPosition);
            transform.position = playerPosition;
        }
    }

    private void PerspectiveHand()
    {
        if (Settings.CAMERA_PERSPECTIVE_HAND)
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
                float clampY = Mathf.Clamp(transform.position.y, -3, 3); // units down, and 0 up
                float clampX = Mathf.Clamp(transform.position.x, -3, 3); // left and right
                transform.position = new Vector3(clampX, clampY, transform.position.z);

            }
        }
    }
}