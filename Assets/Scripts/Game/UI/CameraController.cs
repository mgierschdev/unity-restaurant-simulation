using UnityEngine;

// This class handles the change of camera with the touch
// Attached to: MainCamera Object
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 touchStart;
    [SerializeField]
    private Vector3 direction;
    private GameObject gameBackground;
    private SpriteRenderer background;

    private void Awake()
    {
        touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gameBackground = GameObject.Find(Settings.CONST_GAME_BACKGROUND_DEFAULT).gameObject;
        background = gameBackground.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Settings.PERSPECTIVE_HAND)
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
                float clampY = Mathf.Clamp(transform.position.y, -1, 0); // units down, and 0 up
                float clampX = Mathf.Clamp(transform.position.x, -0.5f, 0.5f); // left and right
                transform.position = new Vector3(clampX, clampY, transform.position.z);

            }
        }
    }
}