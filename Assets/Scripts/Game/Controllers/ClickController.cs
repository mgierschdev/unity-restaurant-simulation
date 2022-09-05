using UnityEngine;

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

    // The object must have a collider attachedDo
    private void ObjectClickedControl()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        Vector3Int clickPosition =
            gridController.GetPathFindingGridFromWorldPosition(mainCamera.ScreenToWorldPoint(Input.mousePosition));
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