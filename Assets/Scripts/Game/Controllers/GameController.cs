using System.Collections.Generic;
using UnityEngine;
// This class in charge of loading the game and prefabs
// This handles the actions of all NPCS, cancel actions in case a table/object moves/it is stored
public class GameController : MonoBehaviour
{
    private const int NPC_MAX_NUMBER = 5;
    private int npcId;
    private GridController gridController;
    private GameObject gameGridObject;
    private GameTile tileSpawn;
    private GameObject NPCS;
    private HashSet<NPCController> NpcSet;
    private EmployeeController employeeController;

    private void Start()
    {
        npcId = 0;
        NpcSet = new HashSet<NPCController>();
        gameGridObject = gameObject.transform.Find(Settings.GameGrid).gameObject;
        gridController = gameGridObject.GetComponent<GridController>();
        NPCS = GameObject.Find(Settings.TilemapObjects).gameObject;
        SpamEmployee();
    }

    private void Update()
    {
        if (NpcSet.Count < NPC_MAX_NUMBER)
        {
            SpamNpc();
        }
    }
    private void SpamNpc()
    {
        tileSpawn = gridController.GetRandomSpamPointWorldPosition();
        GameObject npcObject = Instantiate(Resources.Load(Settings.PrefabNpcClient, typeof(GameObject)), tileSpawn.WorldPosition, Quaternion.identity) as GameObject;
        npcObject.transform.SetParent(NPCS.transform);
        npcObject.name = npcId + "-" + Settings.PrefabNpcClient;
        NPCController isometricNPCController = npcObject.GetComponent<NPCController>();
        NpcSet.Add(isometricNPCController);
        npcId++;
    }
    private void SpamEmployee()
    {
        //Adding Employees
        tileSpawn = gridController.GetRandomSpamPointWorldPosition();
        GameObject employeeObject = Instantiate(Resources.Load(Settings.PrefabNpcEmployee, typeof(GameObject)), tileSpawn.WorldPosition, Quaternion.identity) as GameObject;
        employeeObject.transform.SetParent(NPCS.transform);
        employeeObject.name = npcId + "-" + Settings.PrefabNpcEmployee;
        employeeController = employeeObject.GetComponent<EmployeeController>();
        npcId++;
    }
    public void RemoveNpc(NPCController controller)
    {
        if (NpcSet.Contains(controller))
        {
            NpcSet.Remove(controller);
        }
        else
        {
            GameLog.LogWarning("GameController/RemoveNPC NPC Controller does not exist");
        }
    }

    public bool PositionOverlapsNPC(Vector3Int position)
    {
        // We cannot place on top of the employee
        if (position == gridController.GetPathFindingGridFromWorldPosition(employeeController.transform.position) || position == employeeController.CoordOfTableToBeAttended)
        {
            return true;
        }

        foreach (NPCController npcController in NpcSet)
        {
            Vector3Int npcPosition = gridController.GetPathFindingGridFromWorldPosition(npcController.transform.position);
            if (npcPosition == position)
            {
                return true;
            }
        }
        return false;
    }

    //Recalculates the paths of moving NPCs or they current state depending on whether the grid changed
    public void ReCalculateNpcStates()
    {
        foreach (NPCController npcController in NpcSet)
        {
            if (npcController.GetNpcState() == NpcState.WALKING_TO_TABLE_1)
            {

                //Debug.Log("NPCs on walking state " + npcController.Name + " waking to " + npcController.GetTable().Name);
                if (npcController.CheckIfTableIsStored())
                {
                    //Debug.Log("Stored: " + npcController.Name + " " + npcController.GetNpcState() + " " + (npcController.GetNpcState() == NpcState.WALKING_TO_TABLE_1));
                }
                else
                {

                    //Debug.Log("Recalculating Goto ");
                    npcController.RecalculateGoTo();
                }
            }
            else
            {
                //Debug.Log("NPC " + npcController.Name + " " + npcController.GetNpcState() + " " + (npcController.GetNpcState() == NpcState.WALKING_TO_TABLE_1));
            }
        }
    }
}