using UnityEngine;

// This class handles the change of camera with the touch
// Attached to: MainCamera Object
public class PerspectiveHand : MonoBehaviour
{
    private Vector3 touchStart;
    public float zoomOutMin = 1;
    public float zoomOutMax = 8;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;
        }
    }

}
