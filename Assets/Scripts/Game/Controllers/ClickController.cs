using UnityEngine;

// Controlled attached to Game Object.
public class ClickController : MonoBehaviour
{
    private bool isClicking;
    public bool IsLongClick { get; set; }
    private float ClickingTime { get; set; }
    private const float LONG_CLICK_DURATION = 0.2f;
    private GridController gridController;
    public GameObject ClickedObject { get; set; }
    public GameTile ClickedGameTile { get; set; }
    private Camera mainCamera;
    public bool IsPressingButton { get; set; }

    public bool MouseOverUI { get; set; }

    private void Start()
    {
        // Long Click
        ClickingTime = 0;
        isClicking = false;
        IsLongClick = false;
        mainCamera = Camera.main;
        // Grid Controller
        GameObject gameGridObject = gameObject.transform.Find(Settings.GameGrid).gameObject;
        gridController = gameGridObject.GetComponent<GridController>();
    }

    private void Update()
    {
        // Controls the state of the first and long click
        ClickControl();

        //Object Clicked Control, sets the last ClickedObject
        ObjectClickedControl();
    }

    private void ClickControl()
    {

        // gridController.GetPathFindingGridFromWorldPosition(Util.GetMouseInWorldPosition()), accurate
        
        //Debug.Log(gridController.GetWorldFromPathFindingGridPositionWithOffSet(gridController.GetPathFindingGridFromWorldPosition(Util.GetMouseInWorldPosition())));

        // first click 
        if (Input.GetMouseButtonDown(0))
        {
            ClickingTime = 0;
            isClicking = true;
        }

        // During Click
        if (isClicking)
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
            isClicking = false;
            IsLongClick = false;
        }

        // Resets isLongClick
        if (ClickingTime > LONG_CLICK_DURATION)
        {
            IsLongClick = true;
        }
    }

    // The object must have a collider attached, used for when clicking individual NPCs or detecting long click for the player
    private void ObjectClickedControl()
    {
        if (!Input.GetMouseButtonDown(0) || MouseOverUI)
        {
            return;
        }

        Vector3Int clickPosition = gridController.GetPathFindingGridFromWorldPosition(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        GameTile tile = gridController.GetGameTileFromClickInPathFindingGrid(clickPosition);
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (tile != null)
        {
            ClickedGameTile = tile;
        }

        if (hit.collider)
        {
            ClickedObject = GameObject.Find(hit.collider.name);
        }
    }
}