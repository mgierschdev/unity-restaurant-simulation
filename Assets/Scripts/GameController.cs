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


        // GameObject topGameMenu = gameObject.transform.Find(Settings.CONST_TOP_GAME_MENU).gameObject;
        // topGameMenu.active = false; // disabling top menu tmp

        // Getting grid object
        GameObject gameGridObject = gameObject.transform.Find(Settings.PREFAB_GAME_GRID).gameObject;
        GameGridController gridController = gameGridObject.GetComponent<GameGridController>();

        // Adding Player object
        // GameObject playerObject = Instantiate(Resources.Load(Settings.PREFAB_PLAYER, typeof(GameObject)), gridController.GetCellPosition(10, 13, 1), Quaternion.identity) as GameObject;
        // playerObject.transform.SetParent(gameObject.transform);
        // PlayerController playerController = playerObject.GetComponent<PlayerController>();

        // Adding NPC object
        GameObject npcObject = Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject)), gridController.GetCellPosition(3, 11, 1), Quaternion.identity) as GameObject;
        npcObject.transform.SetParent(gameObject.transform);
        npcController = npcObject.GetComponent<NPCController>();
        npcController.SetGameGridController(gridController);

        // // // Places all the objects in the world
        // for (int i = 1; i < 4; i++)
        // {
        //     Vector2Int itemPosition = new Vector2Int(i, 6);
        //     Vector3 objPos5 = gridController.GetCellPositionWithOffset(itemPosition.x, itemPosition.y);
        //     GameObject obstacleObject5 = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)),new Vector3(objPos5.x, objPos5.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
        //      obstacleObject5.transform.SetParent(gameGridObject.transform);
        //     gameObjects.Add(obstacleObject5);
        // }

        // // Places all the objects in the world
        // for (int i = 7; i < 15; i++) 
        // {
        //     Vector2Int itemPosition = new Vector2Int(i, 6);
        //     Vector3 objPos2 = gridController.GetCellPositionWithOffset(itemPosition.x, itemPosition.y);
        //     GameObject obstacleObject2 = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), new Vector3(objPos2.x, objPos2.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
        //     obstacleObject2.transform.SetParent(gameGridObject.transform);
        //     obstacleObject2.name = "Obstacle: " + i + "," + 7;
        //     gameObjects.Add(obstacleObject2);
        // }

        // // Places all the objects in the world
        // for (int i = 5; i < 9; i++) 
        // {
        //     Vector2Int itemPosition = new Vector2Int(i, 9);
        //     Vector3 objPos6 = gridController.GetCellPositionWithOffset(itemPosition.x, itemPosition.y);
        //     GameObject obstacleObject6 = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), new Vector3(objPos6.x, objPos6.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
        //     obstacleObject6.transform.SetParent(gameGridObject.transform);
        //     obstacleObject6.name = "Obstacle: " + i + "," + 7;
        //     gameObjects.Add(obstacleObject6);
        // }

        // // Places all the objects in the world
        // for (int i = 3; i < 11; i++) 
        // {
        //     Vector2Int itemPosition = new Vector2Int(i, 11);
        //     Vector3 objPos4 = gridController.GetCellPositionWithOffset(itemPosition.x, itemPosition.y);
        //     GameObject obstacleObject4 = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), new Vector3(objPos4.x, objPos4.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
        //     obstacleObject4.transform.SetParent(gameGridObject.transform);
        //     obstacleObject4.name = "Obstacle: " + i + "," + 7;
        //     gameObjects.Add(obstacleObject4);
        // }
            
        //     Vector2Int itemPosition2 = new Vector2Int(8, 8);
        //     Vector3 objPos = gridController.GetCellPositionWithOffset(itemPosition2.x, itemPosition2.y);
        //     GameObject obstacleObject = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), new Vector3(objPos.x, objPos.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
        //     obstacleObject.transform.SetParent(gameGridObject.transform);
        //     gameObjects.Add(obstacleObject);

        //     Vector2Int itemPosition3 = new Vector2Int(8, 7);
        //     Vector3 objPos3 = gridController.GetCellPositionWithOffset(itemPosition3.x, itemPosition3.y);
        //     GameObject obstacleObject3 = Instantiate(Resources.Load(Settings.PREFAB_OBSTACLE, typeof(GameObject)), new Vector3(objPos.x, objPos.y, 1), Quaternion.identity, gameObject.transform) as GameObject;
        //     obstacleObject.transform.SetParent(gameGridObject.transform);
        //     gameObjects.Add(obstacleObject);
            

        
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