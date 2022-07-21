using System;
using UnityEngine;
using System.Collections.Generic;

// Class attached to all static in Game items
public class GameItemController : MonoBehaviour
{
    public float X { get; set; }
    public float Y { get; set; }
    public ObjectType Type { get; set; }
    private int width;
    private int height;
    private Vector3 position;
    private GameGridController gameGrid;
    private GameItemController current;
    private ObjectType type = ObjectType.OBSTACLE;
    private GameObject gameGridObject;

    void Start()
    {
        current = GetComponent<GameItemController>();
        Type = ObjectType.OBSTACLE;
        // Getting game grid
        gameGridObject = GameObject.FindGameObjectWithTag(Settings.PREFAB_GAME_GRID);

        if (gameGridObject != null)
        {
            gameGrid = gameGridObject.GetComponent<GameGridController>();

            UpdatePositionInGrid();
            SetObjectSize();

            // The SetObjectSize has to be setted before calling the grid
            gameGrid.UpdateObjectPosition(current, width, height);
        }
        else
        {
            Debug.LogWarning("GameItemController.cs/gameGridObject null");
        }
    }

    private void SetObjectSize()
    {
        // This block the position of the object in the grid
        Renderer renderer = GetComponent<Renderer>();
        Vector3 bounds = renderer.bounds.size;
        width = Mathf.FloorToInt(bounds.x * (1 * 1 / Settings.GRID_CELL_SIZE));
        height = Mathf.FloorToInt(bounds.y * (1 * 1 / Settings.GRID_CELL_SIZE));
    }

    private void UpdatePositionInGrid()
    {
        Vector2Int pos = Util.GetXYInGameMap(transform.position);
        X = pos.x;
        Y = pos.y;
        position = new Vector3(X, Y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }

    public void SetTestGameGridController(GameGridController controller)
    {
        this.gameGrid = controller;
    }

    public float[] GetPositionAsArray()
    {
        return new float[] { position.x, position.y };
    }
}