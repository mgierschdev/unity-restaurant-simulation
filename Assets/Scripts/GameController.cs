using System.Collections.Generic;
using UnityEngine;


// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
    private GameGridController gridController;
    private List<GameObject> gameItems;

    void Start()
    {
        // Getting grid object
        GameObject gameGridObject = gameObject.transform.Find(Settings.CONST_GAME_GRID).gameObject;
        GameGridController gridController = gameGridObject.GetComponent<GameGridController>();

        // Adding Player object
        GameObject playerObject = Instantiate(Resources.Load(Settings.PREFAB_PLAYER, typeof(GameObject))) as GameObject;
        playerObject.transform.SetParent(gameObject.transform);
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        playerController.SetPosition(gridController.GetCellPosition(3 ,25, 1));

        // Adding NPC object
        GameObject npcObject = Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject))) as GameObject;
        npcObject.transform.SetParent(gameObject.transform);
        NPCController npcController = npcObject.GetComponent<NPCController>();
        npcController.SetPosition(gridController.GetCellPosition(3 ,20, 1));

        // Adding Objects / Obstacles
        Vector2Int itemPosition = new Vector2Int(0, 0);
        GameObject obstacleObject = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), gridController.GetCellPosition(itemPosition.x, itemPosition.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
        gameItems.Add(obstacleObject);
        obstacleObject = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), gridController.GetCellPosition(1, 20, 1), Quaternion.identity, gameObject.transform) as GameObject;
    }
}
