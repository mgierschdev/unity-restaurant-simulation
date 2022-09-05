using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;
// This class in charge of loading the game and prefabs
// the gameBackground Pivot should be in 1,1.
public class GameController : MonoBehaviour
{
    private HashSet<NPCController> npcSet;
    private const int NPC_MAX_NUMBER = 8;
    private int npcId;
    private GridController gridController;
    private GameObject gameGridObject;
    private GameTile tileSpawn;
    private void Start()
    {
        npcSet = new HashSet<NPCController>();
        gameGridObject = gameObject.transform.Find(Settings.GameGrid).gameObject;
        gridController = gameGridObject.GetComponent<GridController>();
        npcId = 0;

        SpamEmployee();
    }

    private void FixedUpdate()
    {
        if (npcSet.Count < NPC_MAX_NUMBER)
        {
            SpamNpc();
        }
    }
    private void SpamNpc()
    {
        tileSpawn = gridController.GetRandomSpamPointWorldPosition();
        GameObject npcObject = Instantiate(Resources.Load(Settings.PrefabNpcClient, typeof(GameObject)), tileSpawn.WorldPosition, Quaternion.identity) as GameObject;
        npcObject.transform.SetParent(gameObject.transform);
        npcObject.name = npcId + "-" + Settings.PrefabNpcClient;
        NPCController isometricNPCController = npcObject.GetComponent<NPCController>();
        npcSet.Add(isometricNPCController);
        npcId++;
    }
    private void SpamEmployee()
    {
        //Adding Employees
        tileSpawn = gridController.GetRandomSpamPointWorldPosition();
        GameObject employeeObject = Instantiate(Resources.Load(Settings.PrefabNpcEmployee, typeof(GameObject)), tileSpawn.WorldPosition, Quaternion.identity) as GameObject;
        employeeObject.transform.SetParent(gameObject.transform);
        employeeObject.name = npcId + "-" + Settings.PrefabNpcEmployee;
        EmployeeController employeeController = employeeObject.GetComponent<EmployeeController>();
        npcId++;
    }
    public void RemoveNpc(NPCController controller)
    {
        if (npcSet.Contains(controller))
        {
            npcSet.Remove(controller);
        }
        else
        {
            GameLog.LogWarning("GameController/RemoveNPC NPC Controller does not exist");
        }
    }
}