using UnityEngine;


// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
    void Start()
    {
        // Adding Player object
        GameObject playerObject = Instantiate(Resources.Load(Settings.PREFAB_PLAYER, typeof(GameObject))) as GameObject;
        playerObject.transform.SetParent(gameObject.transform);
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        playerController.SetPosition(new Vector3(0 ,0, 1));

        // Adding NPC object
        GameObject npcObject = Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject))) as GameObject;
        npcObject.transform.SetParent(gameObject.transform);
        NPCController npcController = npcObject.GetComponent<NPCController>();
        npcController.SetPosition(new Vector3(-1,-1,1));
    }
}
