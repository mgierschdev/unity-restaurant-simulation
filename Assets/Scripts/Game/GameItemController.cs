using UnityEngine;

// Class attached to all static in Game items
public class GameItemController : MonoBehaviour
{
    private float x;
    private float y;
    private GameGridController gameGrid;
    private GameItemController current;

    void Awake() {
        current = GetComponent<GameItemController>();
        // Getting game grid
        gameGrid = GameObject.Find(Settings.CONST_GAME_GRID).gameObject.GetComponent<GameGridController>(); 
        UpdatePositionInGrid(); 
    }

    private void UpdatePositionInGrid(){
        Vector2Int pos = Util.GetXYInGameMap(transform.position);
        x = pos.x;
        y = pos.y;
    }
    
    public int GetX(){
        return (int) x;
    }

    public int GetY(){
        return (int) y;
    }
}