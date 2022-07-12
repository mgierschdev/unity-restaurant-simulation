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

    void Awake()
    {
        current = GetComponent<GameItemController>();
        // Getting game grid
        gameGrid = GameObject.Find(Settings.PREFAB_GAME_GRID).gameObject.GetComponent<GameGridController>();
        UpdatePositionInGrid();
        gameGrid.UpdateObjectPosition(current);
    }

    private void UpdatePositionInGrid()
    {
        Vector2Int pos = Util.GetXYInGameMap(transform.position);
        x = pos.x;
        y = pos.y;
        position = new Vector3(x, y, 1);
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