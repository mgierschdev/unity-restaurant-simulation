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

        //Adding obstacles 
        //gridController.SetHorizontalObstaclesInGrid(3, 3, 10);
    }
}