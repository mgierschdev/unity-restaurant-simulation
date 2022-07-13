using System.Collections.Generic;
using UnityEngine;

// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
    private GameGridController gridController;
    private NPCController npcController;
    private List<GameObject> gameObjects;

    void Start()
    {
        gameObjects = new List<GameObject>();

        // Getting grid object
        GameObject gameGridObject = gameObject.transform.Find(Settings.PREFAB_GAME_GRID).gameObject;
        GameGridController gridController = gameGridObject.GetComponent<GameGridController>();

        // Adding Player object
        // GameObject playerObject = Instantiate(Resources.Load(Settings.PREFAB_PLAYER, typeof(GameObject)), gridController.GetCellPosition(10, 13, 1), Quaternion.identity) as GameObject;
        // playerObject.transform.SetParent(gameObject.transform);
        // PlayerController playerController = playerObject.GetComponent<PlayerController>();

        // Adding NPC object
        GameObject npcObject = Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject)), gridController.GetCellPosition(7, 8, 1), Quaternion.identity) as GameObject;
        npcObject.transform.SetParent(gameObject.transform);
        npcController = npcObject.GetComponent<NPCController>();
        npcController.SetGameGridController(gridController);

        // // Places all the objects in the world
        // for (int i = 1; i < 4; i++)
        // {
        //     Vector2Int itemPosition = new Vector2Int(i, 7);
        //     Vector3 objPos = gridController.GetCellPositionWithOffset(itemPosition.x, itemPosition.y);
        //     GameObject obstacleObject = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)),new Vector3(objPos.x, objPos.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
        //      obstacleObject.transform.SetParent(gameGridObject.transform);
        //     gameObjects.Add(obstacleObject);
        // }

        // Places all the objects in the world
        for (int i = 7; i < 8; i++)
        {
            Vector2Int itemPosition = new Vector2Int(i, 2);
            Vector3 objPos = gridController.GetCellPositionWithOffset(itemPosition.x, itemPosition.y);
            GameObject obstacleObject = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), new Vector3(objPos.x, objPos.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
            obstacleObject.transform.SetParent(gameGridObject.transform);
            obstacleObject.name = "Obstacle: " + i + "," + 7;
            gameObjects.Add(obstacleObject);
        }
    }

    // Places World obstacles
    // private void BuildWorld()
    // {
    //     // Adding Objects / Obstacles
    //     // gridController.SetObstacle(1, 1);

    //     //  for (int i = 0; i < 3; i++)
    //     //   {
    //     Vector2Int itemPosition = new Vector2Int(1, 1);
    //     GameObject obstacleObject = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), gridController.GetCellPosition(itemPosition.x, itemPosition.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
    //     //gameObjects.Add(Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), gridController.GetCellPosition(itemPosition.x, itemPosition.y, 1), Quaternion.identity, gameObject.transform) as GameObject);
    //     // }
    // }
}