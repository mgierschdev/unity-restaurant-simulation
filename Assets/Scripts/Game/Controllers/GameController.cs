using System.Collections.Generic;
using UnityEngine;
// This class in charge of loading the game and prefabs
// This handles the actions of all NPCS, cancel actions in case a table/object moves/it is stored
public class GameController : MonoBehaviour
{
    private const int NPC_MAX_NUMBER = 3;
    private const int EMPLOYEE_MAX_NUMBER = 1;
    private int employeeCount = 0;
    private int npcId;
    private GridController Grid;
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
        Grid = gameGridObject.GetComponent<GridController>();
        NPCS = GameObject.Find(Settings.TilemapObjects).gameObject;
    }

    private void Update()
    {
        if (NpcSet.Count < NPC_MAX_NUMBER)
        {
            SpamNpc();
        }
        if (Grid.GetCounter() != null && employeeCount < EMPLOYEE_MAX_NUMBER)
        {
            SpamEmployee();
            employeeCount++;
        }

        CheckBussSpots();
    }

    //Will check if there any free buss spot, so the could be added to the queue
    public void CheckBussSpots()
    {
        Dictionary<string, GameGridObject> tables = Grid.GetBusinessObjects();
        foreach(KeyValuePair<string, GameGridObject> obj in tables){
            GameGridObject gameGridObject =obj.Value;
            if(gameGridObject.Type == ObjectType.NPC_SINGLE_TABLE && !gameGridObject.GetBusy()){
                Grid.AddFreeBusinessSpots(gameGridObject);
            }
        }
    }

    private void SpamNpc()
    {
        tileSpawn = Grid.GetRandomSpamPointWorldPosition();
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
        tileSpawn = Grid.GetRandomSpamPointWorldPosition();
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

    public void RemoveEmployee()
    {
        employeeCount--;
        employeeController = null;
    }

    public bool PositionOverlapsNPC(Vector3Int position)
    {
        // We cannot place on top of the employee
        if (position == Grid.GetPathFindingGridFromWorldPosition(employeeController.transform.position) || position == employeeController.CoordOfTableToBeAttended)
        {
            return true;
        }

        foreach (NPCController npcController in NpcSet)
        {
            Vector3Int npcPosition = Grid.GetPathFindingGridFromWorldPosition(npcController.transform.position);
            if (npcPosition == position)
            {
                return true;
            }
        }
        return false;
    }

    //Recalculates the paths of moving NPCs or they current state depending on whether the grid changed
    public void ReCalculateNpcStates(GameGridObject obj)
    {
        employeeController.RecalculateState(obj);

        foreach (NPCController npcController in NpcSet)
        {
            if (npcController.GetNpcState() == NpcState.WALKING_TO_TABLE)
            {
                // If the current table has been stored, we reset NPC state 
                if (Grid.IsTableStored(npcController.GetTable().Name))
                {
                    npcController.GoToFinalState_4();
                }
                else
                {
                    npcController.RecalculateGoTo();
                }
            }
        }
    }

    public HashSet<NPCController> GetNpcSet()
    {
        return NpcSet;
    }

    public EmployeeController GetEmployeeController()
    {
        return employeeController;
    }
}