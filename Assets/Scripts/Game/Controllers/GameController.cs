using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;
// This class in charge of loading the game and prefabs
// This handles the actions of all NPCS, cancel actions in case a table/object moves/it is stored
public class GameController : MonoBehaviour
{
    private int NPC_MAX_NUMBER = Settings.MaxNpcNumber,
    EMPLOYEE_MAX_NUMBER = 1,
    employeeCount = 0,
    npcId;
    private GameObject gameGridObject;
    private GameTile tileSpawn;
    private GameObject NPCS;
    private HashSet<NPCController> NpcSet;
    private HashSet<Vector3Int> playerPositionSet;
    private EmployeeController employeeController;

    private void Start()
    {
        npcId = 0;
        NpcSet = new HashSet<NPCController>();
        playerPositionSet = new HashSet<Vector3Int>();
        NPCS = GameObject.Find(Settings.TilemapObjects).gameObject;
        LoadUserObjects();
        StartCoroutine(AssignTables());
        StartCoroutine(NPCSpam());
    }

    private IEnumerator NPCSpam()
    {
        for (; ; )
        {
            if (NpcSet.Count < NPC_MAX_NUMBER)
            {
                SpamNpc();
            }

            if (BussGrid.GetFreeCounter() != null && employeeCount < EMPLOYEE_MAX_NUMBER)
            {
                SpamEmployee();
                employeeCount++;
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator AssignTables()
    {
        for (; ; )
        {
            GameGridObject table = null;

            if (BussGrid.GetFreeTable(out table))
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
            yield return new WaitForSeconds(5f);
        }
    }


    // private void FixedUpdate()
    // {
    //     //not longer required since we assign the tables 
    //     // We check that 2 npc dont have the same table
    //     // HashSet<string> set = new HashSet<string>();

    //     // foreach (NPCController npc in NpcSet)
    //     // {
    //     //     if (npc.HasTable())
    //     //     {
    //     //         if (set.Contains(npc.GetTable().Name))
    //     //         {
    //     //             npc.SetTableMoved();
    //     //         }
    //     //         else
    //     //         {
    //     //             set.Add(npc.GetTable().Name);
    //     //         }
    //     //     }
    //     // }
    // }

    private void LoadUserObjects()
    {
        // The user can store all the inventory 
        if (PlayerData.GetFirebaseGameUser().OBJECTS == null)
        {
            return;
        }

        foreach (FirebaseGameObject obj in PlayerData.GetFirebaseGameUser().OBJECTS)
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
        tileSpawn = BussGrid.GetRandomSpamPointWorldPosition();
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
        if (employeeController != null)
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
        if (employeeController != null && obj.GetAttendedBy() != null)
        {
            employeeController.RecalculateGoTo();
        }

        foreach (NPCController npcController in NpcSet)
        {
            if (npcController.GetNpcState() == NpcState.WALKING_TO_TABLE)
            {
                // If not in any of the previous cases, we recalculate the path 
                // since the grid might be changing
                npcController.RecalculateGoTo();
            }
        }
    }

    public HashSet<NPCController> GetNpcSet()
    {
        return NpcSet;
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

    public EmployeeController GetEmployeeController()
    {
        return employeeController;
    }

    public static void PlaceGameObjectAt(FirebaseGameObject obj)
    {
        StoreItemType type = (StoreItemType)obj.ID;
        Vector3Int position = new Vector3Int(obj.POSITION[0], obj.POSITION[1]);
        Vector3 worldPosition = BussGrid.GetWorldFromPathFindingGridPosition(position);
        string prefab = MenuObjectList.GetPrefab(type);

        if (prefab == "")
        {
            return;
        }

        GameObject newObj = Instantiate(Resources.Load(prefab, typeof(GameObject)), new Vector3(worldPosition.x, worldPosition.y, 1), Quaternion.identity, BussGrid.TilemapObjects.transform) as GameObject;
        BaseObjectController controller = newObj.GetComponent<BaseObjectController>();
        controller.SetFirebaseGameObjectAndInitRotation(obj);
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