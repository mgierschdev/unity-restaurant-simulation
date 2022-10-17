using UnityEngine;

// Controlled attached to Game Object.
public class ClickController : MonoBehaviour
{
    private bool isClicking;
    public bool IsLongClick { get; set; }
    private float ClickingTime { get; set; }
    private const float LONG_CLICK_DURATION = 0.2f;
    private Camera mainCamera;
    private float lastClickTime;

    private bool isPressingButton;
    private bool mouseOverUI;
    private GameObject clickedObject;
    private GameTile clickedGameTile;

    private void Start()
    {
        // Long Click
        ClickingTime = 0;
        isClicking = false;
        IsLongClick = false;
        mainCamera = Camera.main;
        // Time passed between clicks 
        lastClickTime = 0;
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
            lastClickTime = Time.unscaledTime;
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
                ClickingTime += Time.fixedDeltaTime;
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
        if (!Input.GetMouseButtonDown(0) || mouseOverUI)
        {
            return;
        }

        Vector3Int clickPosition = BussGrid.GetPathFindingGridFromWorldPosition(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        GameTile tile = BussGrid.GetGameTileFromClickInPathFindingGrid(clickPosition);
        Vector2 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (tile != null)
        {
            clickedGameTile = tile;
        }

        if (hit.collider)
        {
            clickedObject = GameObject.Find(hit.collider.name);
        }
    }
    public double TimePassedSinceLastClick()
    {
        return 0 > (Time.unscaledTime - lastClickTime) ? 0 : Time.unscaledTime - lastClickTime;
    }

    public bool GetIsPressingButton()
    {
        return isPressingButton;
    }

    public GameObject GetClickedObject()
    {
        return clickedObject;
    }

    public GameTile GetClickedGameTile()
    {
        return clickedGameTile;
    }
    public void SetClickedGameTile(GameTile tile)
    {
        clickedGameTile = tile;
    }
    public void SetClickedObject(GameObject obj)
    {
        clickedObject = obj;
    }
}