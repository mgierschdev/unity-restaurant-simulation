using UnityEngine;

// This class handles the change of camera with the touch
// Attached to: MainCamera Object
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 touchStart;
    [SerializeField]
    private Vector3 direction;
    GameObject gameBackgorund;
    SpriteRenderer spriteBackground;

    private void Awake()
    {
        touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
         
        gameBackgorund = GameObject.Find(Settings.CONST_GAME_BACKGROUND);
        spriteBackground = gameBackgorund.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Settings.PERSPECTIVE_HAND)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                direction.y = touchStart.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
                direction.x = touchStart.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                Camera.main.transform.position += new Vector3(Mathf.Clamp(0, 0, 0),direction.y, 0);

                // then we clamp the value
                //float clampY = Mathf.Clamp(transform.position.y, -(Camera.main.orthographicSize * 2), 0);
                float clampY = Mathf.Clamp(transform.position.y, -14, 0);
                transform.position = new Vector3(0, clampY, transform.position.z);

            }
        }
    }
}