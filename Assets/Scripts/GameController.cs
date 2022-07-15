using UnityEngine;

// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
    private GameGridController gridController;
    private NPCController npcController;

    void Start()
    {
        // Getting grid object
        GameObject gameGridObject = gameObject.transform.Find(Settings.PREFAB_GAME_GRID).gameObject;
        GameGridController gridController = gameGridObject.GetComponent<GameGridController>();

        // Adding NPC object
        GameObject npcObject = Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject)), gridController.GetCellPosition(3, 11, 1), Quaternion.identity) as GameObject;
        npcObject.transform.SetParent(gameObject.transform);
        npcController = npcObject.GetComponent<NPCController>();
        npcController.SetGameGridController(gridController);

        //     Vector2Int itemPosition3 = new Vector2Int(8, 7);
        //     Vector3 objPos3 = gridController.GetCellPositionWithOffset(itemPosition3.x, itemPosition3.y);
        //     GameObject obstacleObject3 = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), new Vector3(objPos.x, objPos.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
        //     obstacleObject.transform.SetParent(gameGridObject.transform);
        //     gameObjects.Add(obstacleObject);
    }
}