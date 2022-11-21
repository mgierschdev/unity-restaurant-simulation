using System.Collections.Generic;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;
// This class in charge of loading the game and prefabs
// This handles the actions of all NPCS, cancel actions in case a table/object moves/it is stored
public class GameController : MonoBehaviour
{
    private int NpcMaxNumber = Settings.MaxNpcNumber, npcId;
    private GameObject gameGridObject, NPCS;
    private GameTile tileSpawn;
    private HashSet<NPCController> NpcSet;
    private HashSet<EmployeeController> EmployeeSet;
    private HashSet<Vector3Int> playerPositionSet, employeePlannedTarget;

    private void Start()
    {
        npcId = 0;
        NpcSet = new HashSet<NPCController>();
        EmployeeSet = new HashSet<EmployeeController>();
        playerPositionSet = new HashSet<Vector3Int>();
        employeePlannedTarget = new HashSet<Vector3Int>();
        NPCS = GameObject.Find(Settings.TilemapObjects).gameObject;
        LoadUserObjects();
        //Assign tables to attend to employees
        StartCoroutine(AssignTablesToNPCs());
        //Spam new NPC is there is missing npcs
        StartCoroutine(NPCSpam());
    }

    private IEnumerator NPCSpam()
    {
        for (; ; )
        {
            if (NpcSet.Count < NpcMaxNumber)
            {
                SpamNpc();
            }

            GameGridObject counter = BussGrid.GetFreeCounter();

            if (counter != null)
            {
                SpamEmployee(counter);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator AssignTablesToNPCs()
    {
        for (; ; )
        {
            if (BussGrid.GetFreeTable(out GameGridObject table))
            {
                foreach (NPCController npcController in NpcSet)
                {
                    if (!npcController.HasTable())
                    {
                        table.SetUsedBy(npcController);
                        npcController.SetTable(table);
                        break;
                    }
                }
            }

            if (BussGrid.GetTableWithClient(out GameGridObject tableToAttend))
            {
                foreach (EmployeeController employeeController in EmployeeSet)
                {
                    if (!employeeController.IsAttendingTable())
                    {
                        // we match an available item  with a client order
                        NPCController clientNPC = tableToAttend.GetUsedBy();

                        if (GetAvailableItem(clientNPC.GetItemToAskFor()) != null)
                        {
                            employeeController.SetTableToAttend(tableToAttend);
                            tableToAttend.SetAttendedBy(employeeController);
                            break;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private GameGridObject GetAvailableItem(ItemType item)
    {
        GameGridObject objectFound = null;
        foreach (KeyValuePair<string, GameGridObject> g in BussGrid.BusinessObjects)
        {

            if (MenuObjectList.GetItemGivenDispenser(g.Value.GetStoreGameObject().StoreItemType) == item &&
            g.Value.GetIsItemReady())
            {
                Debug.Log("Item ready found " + g.Value.Name);
                g.Value.DiableTopInfoObject();
                return objectFound = g.Value;
                break;
            }
        }
        return objectFound;
    }

    private void LoadUserObjects()
    {
        // The user can store all the inventory 
        if (PlayerData.GetDataGameUser().OBJECTS == null)
        {
            return;
        }

        foreach (DataGameObject obj in PlayerData.GetDataGameUser().OBJECTS)
        {
            if (!obj.IS_STORED)
            {
                PlaceGameObjectAt(obj);
            }
        }
    }

    private void SpamNpc()
    {
        tileSpawn = BussGrid.GetRandomSpamPointWorldPosition();
        Vector3 spamPosition = tileSpawn.GetWorldPositionWithOffset();
        spamPosition.z = Util.NPCZPosition;
        GameObject npcObject = Instantiate(Resources.Load(Settings.PrefabNpcClient, typeof(GameObject)), spamPosition, Quaternion.identity) as GameObject;
        npcObject.transform.SetParent(NPCS.transform);
        npcObject.name = npcId + "-" + Settings.PrefabNpcClient;
        NPCController isometricNPCController = npcObject.GetComponent<NPCController>();
        NpcSet.Add(isometricNPCController);
        npcId++;
    }

    private void SpamEmployee(GameGridObject counter)
    {
        tileSpawn = BussGrid.GetRandomSpamPointWorldPosition();
        Vector3 spamPosition = tileSpawn.GetWorldPositionWithOffset();
        spamPosition.z = Util.NPCZPosition;
        GameObject employeeObject = Instantiate(Resources.Load(Settings.PrefabNpcEmployee, typeof(GameObject)), spamPosition, Quaternion.identity) as GameObject;
        employeeObject.transform.SetParent(NPCS.transform);
        employeeObject.name = npcId + "-" + Settings.PrefabNpcEmployee;
        EmployeeController employeeController = employeeObject.GetComponent<EmployeeController>();
        employeeController.SetCounter(counter);
        counter.SetAssignedTo(employeeController);
        EmployeeSet.Add(employeeController);
        npcId++;
    }

    public void RemoveNpc(NPCController controller)
    {
        if (controller == null)
        {
            return;
        }

        if (NpcSet.Contains(controller))
        {
            NpcSet.Remove(controller);
        }
        else
        {
            GameLog.LogWarning("GameController/RemoveNPC NPC Controller does not exist");
        }
    }

    public void RemoveEmployee(EmployeeController employeeController)
    {
        EmployeeSet.Remove(employeeController);
    }

    public bool PositionOverlapsNPC(Vector3Int position)
    {
        foreach (EmployeeController employeeController in EmployeeSet)
        {
            if (position == BussGrid.GetPathFindingGridFromWorldPosition(employeeController.transform.position) || position == employeeController.GetCoordOfTableToBeAttended())
            {
                return true;
            }
        }

        foreach (NPCController npcController in NpcSet)
        {
            Vector3Int npcPosition = BussGrid.GetPathFindingGridFromWorldPosition(npcController.transform.position);
            if (npcPosition == position)
            {
                return true;
            }
        }
        return false;
    }

    //Called when we click an object when we edit or when we store an object
    //Recalculates the paths of moving NPCs or they current state depending on whether the grid changed
    public void ReCalculateNpcStates(GameGridObject obj)
    {
        foreach (EmployeeController employeeController in EmployeeSet)
        {
            employeeController.RecalculateGoTo();
        }

        foreach (NPCController npcController in NpcSet)
        {
            npcController.RecalculateGoTo();
        }
    }

    public HashSet<NPCController> GetNpcSet()
    {
        return NpcSet;
    }

    public HashSet<EmployeeController> GetEmployeeSet()
    {
        return EmployeeSet;
    }

    // Used for debug
    public NPCController GetNPC(string ID)
    {
        foreach (NPCController npc in NpcSet)
        {
            if (npc.Name == ID)
            {
                return npc;
            }
        }
        return null;
    }
    // Used for debug
    public EmployeeController GetEmployee(string ID)
    {
        foreach (EmployeeController npc in EmployeeSet)
        {
            if (npc.Name == ID)
            {
                return npc;
            }
        }
        return null;
    }

    public static void PlaceGameObjectAt(DataGameObject obj)
    {
        StoreItemType type = (StoreItemType)obj.ID;
        Vector3Int position = new Vector3Int(obj.POSITION[0], obj.POSITION[1]);
        Vector3 worldPosition = BussGrid.GetWorldFromPathFindingGridPosition(position);
        string prefab = MenuObjectList.GetPrefab(type);

        if (prefab == "")
        {
            return;
        }

        GameObject newObj = Instantiate(Resources.Load(prefab, typeof(GameObject)), new Vector3(worldPosition.x, worldPosition.y, Util.ObjectZPosition), Quaternion.identity, BussGrid.TilemapObjects.transform) as GameObject;
        BaseObjectController controller = newObj.GetComponent<BaseObjectController>();
        controller.SetDataGameObjectAndInitRotation(obj);
        controller.SetStoreGameObject(MenuObjectList.GetStoreObject((StoreItemType)obj.ID));
    }

    public void AddPlayerPositions(Vector3Int position)
    {
        playerPositionSet.Add(position);
    }

    public void RemovePlayerPosition(Vector3Int position)
    {
        playerPositionSet.Remove(position);
    }

    public HashSet<Vector3Int> GetPlayerPositionSet()
    {
        return playerPositionSet;
    }

    public void AddEmployeePlannedTarget(Vector3Int position)
    {
        employeePlannedTarget.Add(position);
    }

    public void RemoveEmployeePlannedTarget(Vector3Int position)
    {
        employeePlannedTarget.Remove(position);
    }

    public bool IsPathPlannedByEmployee(Vector3Int position)
    {
        return employeePlannedTarget.Contains(position);
    }

    public void PrintDebugEmployeePlannedPaths()
    {
        string debug = "PrintDebugEmployeePlannedPaths: " + employeePlannedTarget.Count;
        foreach (Vector3Int pos in employeePlannedTarget)
        {
            debug += " " + pos + " ";
        }
        GameLog.Log(debug);
    }

    public void PrintDebugPlayerPositionsSet()
    {
        string debug = "PrintDebugPlayerPositionsSet: " + playerPositionSet.Count;
        foreach (Vector3Int pos in playerPositionSet)
        {
            debug += " " + pos + " ";
        }
        GameLog.Log(debug);
    }
}