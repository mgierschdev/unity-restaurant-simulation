using UnityEngine;
using System.Collections.Generic;
// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
     private List<NPCController> npcList;
    private bool enableWander;

    void Start()
    {
        npcList = new List<NPCController>();
        enableWander = false;

        // Getting grid object
        GameObject gameGridObject = gameObject.transform.Find(Settings.PREFAB_GAME_GRID).gameObject;
        GameGridController gridController = gameGridObject.GetComponent<GameGridController>();

        for(int i = 0; i < 3; i++){
            // Adding NPC object
            Vector3 initPos = new Vector3(22, 29, 1);
            GameObject npcObject = Instantiate(Resources.Load(Settings.PREFAB_NPC, typeof(GameObject)), gridController.GetCellPosition(initPos), Quaternion.identity) as GameObject;
            npcObject.transform.SetParent(gameObject.transform);
            npcObject.name = i+"-"+Settings.PREFAB_NPC;
            NPCController controllerNPC = npcObject.GetComponent<NPCController>();
            controllerNPC.Speed = 0.4f;
            this.npcList.Add(controllerNPC);
        }
   }

    void Update(){
        // we execute only once
        if(!enableWander){
            foreach(NPCController n in npcList){
                n.state = NPCState.WANDER;
            }
        }
    }

}