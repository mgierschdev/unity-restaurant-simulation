using System;
using UnityEngine;

// Class attached to all static in Game items
public class GameItemController : MonoBehaviour
{
    private float x;
    private float y;
    private Vector3 position;
    private GameGridController gameGrid;
    private GameItemController current;
    private ObjectType type = ObjectType.OBSTACLE;

    void Start()
    {
        current = GetComponent<GameItemController>();
        // Getting game grid
        GameObject gameGridObject = GameObject.Find(Settings.PREFAB_GAME_GRID);

        if (gameGrid != null)
        {
            gameGrid = gameGridObject.GetComponent<GameGridController>();
            UpdatePositionInGrid();
            gameGrid.UpdateObjectPosition(current);
        }
        else
        {
            Debug.LogWarning("GameItemController.cs/gameGridObject null");
        }
    }

    private void UpdatePositionInGrid()
    {
        Vector2Int pos = Util.GetXYInGameMap(transform.position);
        x = pos.x;
        y = pos.y;
        position = new Vector3(x, y, 1);
    }

    public void SetGameGridController(GameGridController controller)
    {
        this.gameGrid = controller;
    }

    public int GetX()
    {
        return (int)x;
    }

    public int GetY()
    {
        return (int)y;
    }

    public ObjectType GetType()
    {
        return ObjectType.OBSTACLE;
    }

    public Vector3 GetPosition()
    {
        return position;
    }
}