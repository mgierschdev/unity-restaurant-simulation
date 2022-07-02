using UnityEngine;

// This class handles the change of camera with the touch
// Attached to: MainCamera Object
public class PerspectiveHand : MonoBehaviour
{
    [SerializeField]
    private Vector3 touchStart;
    [SerializeField]
    private Vector3 direction;

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
                // Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                direction.y = touchStart.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
                Camera.main.transform.position += direction;
            }
        }
    }
}