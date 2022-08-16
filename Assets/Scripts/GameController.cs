using UnityEngine;
using System.Collections.Generic;
// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
    private List<IsometricNPCController> npcList;
    private bool enableWander;

    void Start()
    {
        npcList = new List<IsometricNPCController>();
        enableWander = false;

        // // Getting grid object
        GameObject gameGridObject = gameObject.transform.Find(Settings.GAME_GRID).gameObject;
        IsometricGridController gridController = gameGridObject.GetComponent<IsometricGridController>();

        for(int i = 0; i < 3; i++){
            // Adding NPC object
            Vector3Int initPos = new Vector3Int(19, 10);
            GameObject npcObject = Instantiate(Resources.Load(Settings.PREFAB_ISOMETRIC_NPC, typeof(GameObject)), gridController.GetWorldFromPathFindingGridPosition(initPos), Quaternion.identity) as GameObject;
            npcObject.transform.SetParent(gameObject.transform);
            npcObject.name = i+"-"+Settings.PREFAB_NPC;
            IsometricNPCController isometricNPCController = npcObject.GetComponent<IsometricNPCController>();
            isometricNPCController.Speed = 0.4f;
            this.npcList.Add(isometricNPCController);
        }
   }

    void Update(){
        // we execute only once
        if(!enableWander){
            foreach(IsometricNPCController n in npcList){
                n.state = NPCState.WANDER;
            }
        }
    }

}