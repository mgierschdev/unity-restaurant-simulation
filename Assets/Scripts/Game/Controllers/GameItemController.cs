using UnityEngine;
using UnityEngine.Rendering;

// Non-Isometric Game Object Controller
// Currently refactoring the entire Grid
public class GameItemController : GameObjectBase
{
    private GameObject gameGridObject;
    private GameItemController current;
    public ObjectType Type { get; set; }
    private int width;
    private int height;

    // // Start since the grid is settedup in Awake
    // public void Start()
    // {
    //     WorldPosition = transform.position;
    //     sortingLayer = GetComponent<SortingGroup>();
    //     current = GetComponent<GameItemController>();
    //     Type = ObjectType.OBSTACLE;

    //     // Getting game grid
    //     gameGridObject = GameObject.FindGameObjectWithTag(Settings.GAME_GRID);

    //     if (gameGridObject != null)
    //     {
    //         gameGrid = gameGridObject.GetComponent<GameGridController>();
    //         UpdatePositionInGrid();
    //         SetObjectSize();
    //         // The SetObjectSize has to be setted before calling the grid
    //         gameGrid.UpdateObjectPosition(current, width, height);
    //     }
    //     else
    //     {
    //         if (Settings.DEBUG_ENABLE)
    //         {
    //             Debug.LogError("GameItemController.cs/gameGridObject null");
    //         }
    //     }
    // }

    // protected void SetObjectSize()
    // {
    //     // This block the position of the object in the grid
    //     Renderer renderer = GetComponent<Renderer>();
    //     Vector3 bounds = renderer.bounds.size;
    //     width = Mathf.FloorToInt(bounds.x * (1 * 1 / Settings.GRID_CELL_SIZE));
    //     height = Mathf.FloorToInt(bounds.y * (1 * 1 / Settings.GRID_CELL_SIZE));
    // }
}