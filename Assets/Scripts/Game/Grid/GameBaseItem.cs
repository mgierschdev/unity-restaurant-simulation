using UnityEngine;
using UnityEngine.Rendering;

public abstract class GameBaseItem : MonoBehaviour
{
    public float X { get; set; }
    public float Y { get; set; }
    public ObjectType Type { get; set; }
    public int width;
    public int height;
    public Vector3 position;
    public ObjectType type = ObjectType.OBSTACLE;
    public GameItemController current;
    protected GameGridController gameGrid;
    protected GameObject gameGridObject;

    // Sprite level ordering
    protected SortingGroup sortingLayer;

    // Start since the grid is settedup in Awake
    public void Start()
    {
        sortingLayer = GetComponent<SortingGroup>();
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
        sortingLayer.sortingOrder = pos.y * -1;
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