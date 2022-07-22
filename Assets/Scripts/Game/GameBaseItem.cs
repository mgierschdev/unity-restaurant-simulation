using UnityEngine;

public abstract class GameBaseItem : MonoBehaviour
{
    public float X { get; set; }
    public float Y { get; set; }
    public ObjectType Type { get; set; }
    protected int width;
    protected int height;
    protected Vector3 position;
    protected GameGridController gameGrid;
    protected GameItemController current;
    protected ObjectType type = ObjectType.OBSTACLE;
    protected GameObject gameGridObject;

    public void Start()
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
            if (Settings.DEBUG_ENABLE)
            {
                Debug.LogError("GameItemController.cs/gameGridObject null");
            }
        }
    }

    protected void SetObjectSize()
    {
        // This block the position of the object in the grid
        Renderer renderer = GetComponent<Renderer>();
        Vector3 bounds = renderer.bounds.size;
        width = Mathf.FloorToInt(bounds.x * (1 * 1 / Settings.GRID_CELL_SIZE));
        height = Mathf.FloorToInt(bounds.y * (1 * 1 / Settings.GRID_CELL_SIZE));
    }

    protected void UpdatePositionInGrid()
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