using UnityEngine;

// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
    void Start()
    {
        // Getting grid object
        GameObject gameGridObject = gameObject.transform.Find(Settings.PREFAB_GAME_GRID).gameObject;
        GameGridController gridController = gameGridObject.GetComponent<GameGridController>();

        // Adding NPC object
        GameObject npcObject = Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject)), gridController.GetCellPosition(new Vector3(22, 30, 1)), Quaternion.identity) as GameObject;
        npcObject.transform.SetParent(gameObject.transform);
        npcObject.name = Settings.PREFAB_NPC;
        NPCController npcController = npcObject.GetComponent<NPCController>();
        npcController.WanderOn = true;

       // PlayerController playerController =  GameObject.FindGameObjectWithTag(Settings.PREFAB_PLAYER).gameObject.GetComponent<PlayerController>();
    }
}