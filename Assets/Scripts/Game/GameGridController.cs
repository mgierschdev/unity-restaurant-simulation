using System;
using UnityEngine;
using System.Collections.Generic;

// In game position is defined by the grid coordinates (0,0), (0,1). World position is defined by the Unity Coords plane.
public class GameGridController : MonoBehaviour
{
    private readonly int width = Settings.GRID_WIDTH;
    private readonly int height = Settings.GRID_HEIGHT;
    private readonly int cellSize = 1;
    private int[,] gridArray;

    // Game objects in UI either NPCs or Static objects
    private HashSet<NPCController> npcs;
    private HashSet<GameItemController> items;

    private TextMesh[,] debugArray;
    private Vector3 gridOriginPosition = new Vector3(Settings.GRID_START_X, Settings.GRID_START_Y, Settings.CONST_DEFAULT_BACKGROUND_ORDERING_LEVEL);
    private Vector3 originPosition = new Vector3(Settings.GRID_START_X, Settings.GRID_START_Y, 0);

    // Debug Parameters
    private readonly int debugLineDuration = Settings.DEBUG_DEBUG_LINE_DURATION; // in seconds
    private readonly int cellTexttSize = Settings.DEBUG_TEXT_SIZE;

    //Caching 

    public void Awake()
    {
        gridArray = new int[width, height];
        items = new HashSet<GameItemController>();
        npcs = new HashSet<NPCController>();
        Vector3 textCellOffset = new Vector3(cellSize, cellSize) * cellSize / 2;

        if(Settings.DEBUG_ENABLE){
            debugArray = new TextMesh[width, height];
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x, y + 1), Color.white, debugLineDuration);
                    Debug.DrawLine(GetCellPosition(x, y), GetCellPosition(x + 1, y), Color.white, debugLineDuration);
                    debugArray[x, y] = Util.CreateTextObject(x+","+y,gameObject, gridArray[x, y].ToString(), GetCellPosition(x, y) +
                        textCellOffset, cellTexttSize, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                }
                Debug.DrawLine(GetCellPosition(0, height), GetCellPosition(width, height), Color.white, debugLineDuration);
                Debug.DrawLine(GetCellPosition(width, 0), GetCellPosition(width, height), Color.white, debugLineDuration);
            }
        }
    }

    public void Update()
    {
        if(Settings.DEBUG_ENABLE){
            MouseOnClick();
        }
    }

    // Debug Methods
    private void MouseHeatMap(){
        Vector2Int mouseInGame = GetMousePositionInGame();
        if(IsInsideGridLimit(mouseInGame.x, mouseInGame.y)){
            SetValue(Util.GetMouseInWorldPosition(), GetCellValueInGamePosition(mouseInGame.x, mouseInGame.y) + (int)(100 * Time.deltaTime));
        }
    }
    
    private void MouseOnClick(){
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector2Int mousePositionVector = Util.GetXYInGameMap(mousePosition);
            SetValue(Util.GetMouseInWorldPosition(), GetCellValueInGamePosition(mousePositionVector.x, mousePositionVector.y) + 10);
            PrintGrids();
        }
    }

    private void PrintGrids(){
       // PrintArray(obstacles);
        PrintArray(gridArray);
    }
    
    private void SetValue(int x, int y, int value)
    {
        if(!IsInsideGridLimit(x, y)){
            return;
        }
        gridArray[x, y] = value;
        debugArray[x, y].text = gridArray[x, y].ToString();
    }
    
    private int GetCellValueInGamePosition(int x, int y){
        if(!IsInsideGridLimit(x, y)){
            Debug.LogError("The GetCellValueInGamePosition is outside boundaries");
            throw new Exception();
            return -1;
        }
        string text = debugArray[x, y].text;
        int value;
        int.TryParse(text, out value);
        return value;
    }
    
    private void PrintArray(int[,] a){
        for(int i = 0; i < a.GetLength(0); i++){
            for(int j = 0; j < a.GetLength(1); j++){
                Debug.Log(a[i, j] +" "+i+","+j);
            }
        }
    }
   
       private void setCellBlue(int x, int y){
        debugArray[x, y].text = gridArray[x, y].ToString();
        debugArray[x, y].color = Color.blue; // Busy
    }

   
    // Debug Methods

    private Vector3 GetCellPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + gridOriginPosition;
    }
    
    private void SetValue(Vector3 position, int value)
    {
        Vector2Int pos = Util.GetXYInGameMap(position);
        SetValue(pos.x, pos.y, value);
    }

    private bool IsInsideGridLimit(int x, int y){
        return (x >= 0 && x < gridArray.GetLength(0) && y >= 0 && y < gridArray.GetLength(1));
    }

    public Vector2Int GetMousePositionInGame(){
        Vector3 mousePosition = Util.GetMouseInWorldPosition();
        return Util.GetXYInGameMap(mousePosition);
    }

    public Vector3 GetCellPosition(int x, int y, int z)
    {
        Debug.Log(x+" "+y);
        if(!IsInsideGridLimit(x, y)){
            Debug.LogError("The GetCellPosition is outside boundaries");
            throw new Exception();
            return new Vector3();
        }
        return new Vector3(x, y, z) * cellSize + originPosition;
    }

    private void SetObstacle(int x, int y){
        if(!IsInsideGridLimit(x, y) && x > 1 && y > 1){
            Debug.LogError("The object should be placed inside the perimeter");
            throw new Exception();
            return;
        }

        Debug.Log(x+","+y);

        gridArray[x, y] = 1;
        gridArray[x, y - 1] = 1;
        gridArray[x - 1, y] = 1;
        gridArray[x - 1, y - 1] = 1;

        if(Settings.DEBUG_ENABLE){
            setCellBlue(x, y);
            setCellBlue(x, y - 1);
            setCellBlue(x - 1, y);
            setCellBlue(x - 1, y - 1);
        }
    }

    public void UpdateNPCPosition(NPCController obj){
        if(!npcs.Contains(obj)){
            npcs.Add(obj);
        }
        SetObstacle(obj.GetX(), obj.GetY());
    }

    public void UpdateItemPosition(GameItemController obj){
        if(!items.Contains(obj)){
            items.Add(obj);
        }

        SetObstacle(obj.GetX(), obj.GetY());
    }
    
    public void SetObstacleInGridPosition(int x, int y){
         SetObstacle(x, y);
    }

    // Unset position in Grid
}