using UnityEngine;

public class ClickController : MonoBehaviour
{
    //MovingOnLongtouch(), Long click or touch vars
    public bool IsClicking { get; set; }
    public bool IsLongClick { get; set; }
    public float ClickingTime { get; set; }// To keep the coung if longclick
    private float longClickDuration = 0.2f;

    private IsometricGridController gridController;

    public GameObject ClickedObject { get; set; }

    private void Start()
    {
        // Long Click
        ClickingTime = 0;
        IsClicking = false;
        IsLongClick = false;

        // Grid Controller
        GameObject gameGridObject = gameObject.transform.Find(Settings.GAME_GRID).gameObject;
        gridController = gameGridObject.GetComponent<IsometricGridController>();
    }

    private void Update()
    {
        // Controls the state of the first and long click
        ClickControl();

        //Object Cliked Control, sets the last ClickedObject
        ObjectClickedControl();
    }

    private void ClickControl()
    {
        // first click 
        if (Input.GetMouseButtonDown(0))
        {
            ClickingTime = 0;
            IsClicking = true;
        }

        // During Click
        if (IsClicking)
        {
            // Continues counting even while the game is paused
            if (Time.deltaTime == 0)
            {
                ClickingTime += Time.unscaledDeltaTime;
            }
            else
            {
                ClickingTime += Time.deltaTime;
            }
        }

        // On realising the mouse
        if (Input.GetMouseButtonUp(0))
        {
            ClickingTime = 0;
            IsClicking = false;
            IsLongClick = false;
        }

        // Resets isLongClick
        if (ClickingTime > longClickDuration)
        {
            IsLongClick = true;
        }
    }

    // The object must have a collider attachedDo
    public void ObjectClickedControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int clickPosition = gridController.GetPathFindingGridFromWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            GameTile tile = gridController.GetGameTileFromClickInWorldPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            if (tile != null)
            {
                Debug.Log("Tile " + tile.Type);
            }
            
            Debug.Log(clickPosition);

            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider != null)
            {
                ClickedObject = GameObject.Find(hit.collider.name);
            }
        }
    }
}