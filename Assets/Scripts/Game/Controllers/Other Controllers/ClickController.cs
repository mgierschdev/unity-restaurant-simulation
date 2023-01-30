using System.Collections.Generic;
using UnityEngine;

// Controlled attached to Game scene and main Game Object.
public class ClickController : MonoBehaviour
{
    private bool isPressingButton, mouseOverUI;
    private float lastClickTime;
    private GameTile clickedGameTile;
    private Camera mainCamera;
    private GameObject clickedObject;
    private GameGridObject clickedGameGridObject;

    private void Start()
    {
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
            if (BussGrid.GetGameGridObjectsDictionary().ContainsKey(r.name))
            {
                GameGridObject selected = BussGrid.GetGameGridObjectsDictionary()[r.name];
                list.Add(selected, selected.GetSortingOrder());
            }
        }

        if (list.Count > 0)
        {
            clickedGameGridObject = list.Keys[0];

        }
        else
        {
            clickedGameGridObject = null;
        }

        if (tile != null)
        {
            clickedGameTile = tile;
        }
    }

    public GameGridObject GetGameGridClickedObject()
    {
        return clickedGameGridObject;
    }
}