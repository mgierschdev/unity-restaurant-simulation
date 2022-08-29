using System;
using System.Collections.Generic;
using UnityEngine;
// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
    private List<NPCController> npcList;
    private int npcNumber = 6;
    GridController gridController;

    void Start()
    {
        npcList = new List<NPCController>();
        // Getting grid object
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
            this.npcList.Add(isometricNPCController);
        }

        //Adding Employees
        Vector3 spamCoord = gridController.GetRandomSpamPointWorldPosition();
        GameObject employeeObject = Instantiate(Resources.Load(Settings.PREFAB_NPC_EMPLOYEE, typeof(GameObject)), spamCoord, Quaternion.identity) as GameObject;
        employeeObject.transform.SetParent(gameObject.transform);
        employeeObject.name = 0 + "-" + Settings.PREFAB_NPC_EMPLOYEE;
        EmployeeController employeeController = employeeObject.GetComponent<EmployeeController>();
    }
}