using UnityEngine;
using UnityEngine.Tilemaps;

//Will handle all GetComponent and Find Calls and cache the items for future reference
public class GridComponentsController : MonoBehaviour
{
    public void Awake()
    {
        //Grid Components
        BussGrid.TilemapPathFinding = GameObject.Find(Settings.PathFindingGrid).GetComponent<Tilemap>();
        BussGrid.TilemapFloor = GameObject.Find(Settings.TilemapFloor0).GetComponent<Tilemap>();
        BussGrid.TilemapColliders = GameObject.Find(Settings.TilemapColliders).GetComponent<Tilemap>();
        BussGrid.TilemapObjects = GameObject.Find(Settings.TilemapObjects).GetComponent<Tilemap>();
        BussGrid.TilemapWalkingPath = GameObject.Find(Settings.TilemapWalkingPath).GetComponent<Tilemap>();
        GameObject gameObj = GameObject.Find(Settings.ConstParentGameObject);
        BussGrid.GameController = gameObj.GetComponent<GameController>();
        BussGrid.ControllerGameObject = gameObject;
        //Buss TileFloor, returns it depending on the PLayer GridSize
        BussGrid.TilemapGameFloor = GameObject.Find(PlayerData.GetTileBussFloor()).GetComponent<Tilemap>();
        BussGrid.Init();
    }
}