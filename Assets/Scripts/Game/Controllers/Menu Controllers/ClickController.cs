using System.Collections.Generic;
using UnityEngine;

// Controlled attached to Game scene and main Game Object.
public class ClickController : MonoBehaviour
{
    private bool isClicking, isLongClick, isPressingButton, mouseOverUI;
    private float clickingTime, LONG_CLICK_DURATION = 0.2f, lastClickTime;
    private Camera mainCamera;
    private GameObject clickedObject;
    private GameGridObject clickedGameGridObject;
    private GameTile clickedGameTile;

    private void Start()
    {
        // Long Click
        clickingTime = 0;
        isClicking = false;
        isLongClick = false;
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
            clickingTime = 0;
            isClicking = true;
        }

        // During Click
        if (isClicking)
        {
            // Continues counting even while the game is paused
            if (Time.deltaTime == 0)
            {
                clickingTime += Time.unscaledDeltaTime;
            }
            else
            {
                clickingTime += Time.fixedDeltaTime;
            }
        }

        // On realising the mouse
        if (Input.GetMouseButtonUp(0))
        {
            clickingTime = 0;
            isClicking = false;
            isLongClick = false;
        }

        // Resets isLongClick
        if (clickingTime > LONG_CLICK_DURATION)
        {
            isLongClick = true;
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
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPoint);
        SortedList<GameGridObject, int> list = new SortedList<GameGridObject, int>();

        foreach (Collider2D r in hits)
        {
            if (BussGrid.GetBusinessObjects().ContainsKey(r.name))
            {
                GameGridObject selected = BussGrid.GetBusinessObjects()[r.name];
                list.Add(selected, selected.GetSortingOrder());
            }
        }

        if (list.Count > 0)
        {
            clickedGameGridObject = list.Keys[0];
            clickedObject = GameObject.Find(list.Keys[0].Name);
        }

        if (tile != null)
        {
            clickedGameTile = tile;
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

    public GameGridObject GetGameGridClickedObject()
    {
        return clickedGameGridObject;
    }
}