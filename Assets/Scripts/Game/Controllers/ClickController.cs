using UnityEngine;

public class ClickController : MonoBehaviour
{
    //MovingOnLongtouch(), Long click or touch vars
    public bool IsClicking { get; set; }
    public bool IsLongClick { get; set; }
    public float ClickingTime { get; set; }// To keep the coung if longclick
    private float longClickDuration = 0.2f;

    private void Start()
    {
        // Long Click
        ClickingTime = 0;
        IsClicking = false;
        IsLongClick = false;
    }

    private void Update()
    {
        // Controls the state of the first and long click
        ClickControl();
        Debug.Log("Clicking time "+ClickingTime + " "+IsLongClick);
    }

    private void ClickControl()
    {
        // first click 
        if (Input.GetMouseButtonDown(0))
        {
            ClickingTime = 0;
            IsClicking = true;
        }

        // On realising the mouse
        if(Input.GetMouseButtonUp(0)){
            ClickingTime = 0;
            IsClicking = false;
            IsLongClick = false;
        }

        // During Click
        if (IsClicking && Input.GetKey(KeyCode.Mouse0))
        {
            ClickingTime += Time.deltaTime;
        }

        // Resets isLongClick
        if (ClickingTime > longClickDuration)
        {
            IsLongClick = true;
        }
    }
}