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
        BussGrid.TilemapBusinessFloor = GameObject.Find(Settings.TilemapBusinessFloor).GetComponent<Tilemap>();
        BussGrid.CameraController = GameObject.FindGameObjectWithTag(Settings.MainCamera).GetComponent<CameraController>();
        GameObject gameObj = GameObject.Find(Settings.ConstParentGameObject);
        BussGrid.GameController = gameObj.GetComponent<GameController>();
        BussGrid.ControllerGameObject = gameObject;
        BussGrid.Init();
    }
}