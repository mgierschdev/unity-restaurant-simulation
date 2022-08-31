using System.Collections.Generic;
using UnityEngine;
// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
    private HashSet<NPCController> NPCSet;
    private HashSet<EmployeeController> NPCEmployeeSet;
    private int npcMaxNumber = 8;
    private int NPCidx;
    GridController gridController;
    GameObject gameGridObject;
    GameTile tileSpawn;
    void Start()
    {
        NPCSet = new HashSet<NPCController>();
        NPCEmployeeSet = new HashSet<EmployeeController>();
        gameGridObject = gameObject.transform.Find(Settings.GAME_GRID).gameObject;
        gridController = gameGridObject.GetComponent<GridController>();
        NPCidx = 0;

        SpamEmployee();
    }
    private void FixedUpdate()
    {
        if (NPCSet.Count < npcMaxNumber)
        {
            SpamNPC();
        }
    }
    private void SpamNPC()
    {
        tileSpawn = gridController.GetRandomSpamPointWorldPosition();
        GameObject npcObject = Instantiate(Resources.Load(Settings.PREFAB_NPC_CLIENT, typeof(GameObject)), tileSpawn.WorldPosition, Quaternion.identity) as GameObject;
        npcObject.transform.SetParent(gameObject.transform);
        npcObject.name = NPCidx + "-" + Settings.PREFAB_NPC_CLIENT;
        NPCController isometricNPCController = npcObject.GetComponent<NPCController>();

        NPCSet.Add(isometricNPCController);
        NPCidx++;
    }
    private void SpamEmployee()
    {
        //Adding Employees
        tileSpawn = gridController.GetRandomSpamPointWorldPosition();
        GameObject employeeObject = Instantiate(Resources.Load(Settings.PREFAB_NPC_EMPLOYEE, typeof(GameObject)), tileSpawn.WorldPosition, Quaternion.identity) as GameObject;
        employeeObject.transform.SetParent(gameObject.transform);
        employeeObject.name = NPCidx + "-" + Settings.PREFAB_NPC_EMPLOYEE;
        EmployeeController employeeController = employeeObject.GetComponent<EmployeeController>();
        NPCEmployeeSet.Add(employeeController);
        NPCidx++;
    }
    public void RemoveNPC(NPCController controller)
    {
        if (NPCSet.Contains(controller))
        {
            NPCSet.Remove(controller);
        }
        else
        {
            Debug.LogWarning("GameController/RemoveNPC NPC Controller does not exist");
        }
    }
}