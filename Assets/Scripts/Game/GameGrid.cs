using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{

    private int width = 6;
    private int height = 10;
    private int cellSize = 1;
    private int[,] gridArray;
    private int debugLineDuration = 1000; // in seconds
    private int offSet = 5; // To center the grid in the middle
    private int cellTexttSize = 40;
    private TextMesh[,] debugArray;
    private Vector3 originPosition = new Vector3(-2.5f, -5, 0);


    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetValue(Util.GetMouseToWorldPosition(), 50);
        }
    }

    public void Awake()
    {
        if (!Settings.GRID_ENABLED)
        {
            gameObject.SetActive(false);
        }

        gridArray = new int[width, height];
        debugArray = new TextMesh[width, height];
        Vector3 textCellOffset = new Vector3(cellSize, cellSize) * cellSize / 2;



        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x, y + 1), Color.white, debugLineDuration);
                Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x + 1, y), Color.white, debugLineDuration);
                debugArray[x, y] = Util.CreateTextObject(gameObject, gridArray[x, y].ToString(), GetCellPosition(x, y) + textCellOffset, cellTexttSize, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
            }
            Debug.DrawLine(GetCellPosition(0, height), GetCellPosition(width, height), Color.white, debugLineDuration);
            Debug.DrawLine(GetCellPosition(width, 0), GetCellPosition(width, height), Color.white, debugLineDuration);
        }
    }

    private Vector3 GetCellPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    private Vector2Int GetXY(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt((position.x - originPosition.x) / cellSize), Mathf.FloorToInt((position.y - originPosition.y) / cellSize));
    }

    private void SetValue(int x, int y, int value)
    {
        if(x < 0 || y < 0 || x >= width || y >= height)
        {
            return;
        }
        gridArray[x, y] = value;
        debugArray[x, y].text = gridArray[x, y].ToString();
    }

    private void SetValue(Vector3 position, int value)
    {
        Vector2Int pos = GetXY(position);
        SetValue(pos.x, pos.y, value);
    }
}