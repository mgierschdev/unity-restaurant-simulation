using System;
using System.Collections.Generic;
using UnityEngine;
// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
    private List<IsometricNPCController> npcList;
    private bool enableWander;
    private int npcNumber = 3;
    IsometricGridController gridController;

    void Start()
    {
        npcList = new List<IsometricNPCController>();
        enableWander = false;

        // // Getting grid object
        GameObject gameGridObject = gameObject.transform.Find(Settings.GAME_GRID).gameObject;
        IsometricGridController gridController = gameGridObject.GetComponent<IsometricGridController>();

        for (int i = 0; i < npcNumber; i++)
        {
            // Adding NPC object
            Vector3 spamPoint = gridController.GetRandomSpamPointWorldPosition();
            GameObject npcObject = Instantiate(Resources.Load(Settings.PREFAB_ISOMETRIC_NPC, typeof(GameObject)), spamPoint, Quaternion.identity) as GameObject;
            npcObject.transform.SetParent(gameObject.transform);
            npcObject.name = i + "-" + Settings.PREFAB_ISOMETRIC_NPC;
            IsometricNPCController isometricNPCController = npcObject.GetComponent<IsometricNPCController>();
            isometricNPCController.Speed = 0.4f;//0.4f
            this.npcList.Add(isometricNPCController);
        }
    }

    void Update()
    {
        // we execute only once
        if (!enableWander)
        {
            foreach (IsometricNPCController n in npcList)
            {
                n.state = NPCState.WANDER;
            }

            enableWander = false;
        }
    }
}