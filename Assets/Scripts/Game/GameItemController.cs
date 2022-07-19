using System;
using UnityEngine;
using System.Collections.Generic;

// Class attached to all static in Game items
public class GameItemController : MonoBehaviour, IGameObject
{
    private float x;
    private float y;
    private float speed = 0;
    private Vector3 position;
    private GameGridController gameGrid;
    private GameItemController current;
    private ObjectType type = ObjectType.OBSTACLE;
    private GameObject gameGridObject;

    void Start()
    {
        current = GetComponent<GameItemController>();
        // Getting game grid
        gameGridObject = GameObject.FindGameObjectWithTag(Settings.PREFAB_GAME_GRID);

        if (gameGridObject != null)
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

    public void AddMovement(Vector3 direction)
    {
        return;
    }

    public void AddPath(List<Node> n)
    {
        return;
    }

    public void SetTestGameGridController(GameGridController controller)
    {
        this.gameGrid = controller;
    }

    public float GetX()
    {
        return x;
    }

    public float GetY()
    {
        return y;
    }

    public ObjectType GetType()
    {
        return ObjectType.OBSTACLE;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public float[] GetPositionAsArray()
    {
        return new float[] { position.x, position.y };
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}