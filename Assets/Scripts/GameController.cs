using System;
using System.Collections.Generic;
using UnityEngine;
// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
    private List<NPCController> npcList;
    private bool enableWander;
    private int npcNumber = 3;
    GridController gridController;

    void Start()
    {
        npcList = new List<NPCController>();
        enableWander = false;

        // // Getting grid object
        GameObject gameGridObject = gameObject.transform.Find(Settings.GAME_GRID).gameObject;
        GridController gridController = gameGridObject.GetComponent<GridController>();

        for (int i = 0; i < npcNumber; i++)
        {
            // Adding NPC object
            Vector3 spamPoint = gridController.GetRandomSpamPointWorldPosition();
            GameObject npcObject = Instantiate(Resources.Load(Settings.PREFAB_NPC_CLIENT, typeof(GameObject)), spamPoint, Quaternion.identity) as GameObject;
            npcObject.transform.SetParent(gameObject.transform);
            npcObject.name = i + "-" + Settings.PREFAB_NPC_CLIENT;
            NPCController isometricNPCController = npcObject.GetComponent<NPCController>();
            isometricNPCController.Speed = 0.4f;//0.4f
            this.npcList.Add(isometricNPCController);
        }
    }

    void Update()
    {
        // we execute only once
        if (!enableWander)
        {
            foreach (NPCController n in npcList)
            {
                n.state = NPCState.WANDER;
            }

            enableWander = false;
        }
    }
}