using System.Collections.Generic;
using UnityEngine;
// This class in charge of loading the game and prefabs
// This handles the actions of all NPCS, cancel actions in case a table/object moves/it is stored
public class GameController : MonoBehaviour
{
    private const int NPC_MAX_NUMBER = 4;
    private const int EMPLOYEE_MAX_NUMBER = 1;
    private int employeeCount = 0;
    private int npcId;
    private GameObject gameGridObject;
    private GameTile tileSpawn;
    private GameObject NPCS;
    private HashSet<NPCController> NpcSet;
    private EmployeeController employeeController;

    private void Start()
    {
        npcId = 0;
        NpcSet = new HashSet<NPCController>();
        NPCS = GameObject.Find(Settings.TilemapObjects).gameObject;
        LoadUserObjects();
        //SetItemsActive();//This will start the game
    }

    private void Update()
    {
        if (NpcSet.Count < NPC_MAX_NUMBER)
        {
            SpamNpc();
        }
        if (BussGrid.GetCounter() != null && employeeCount < EMPLOYEE_MAX_NUMBER)
        {
            SpamEmployee();
            employeeCount++;
        }
    }

    private void FixedUpdate()
    {
        // We check that 2 npc dont have the same table
        HashSet<string> set = new HashSet<string>();

        foreach (NPCController npc in NpcSet)
        {
            if (npc.HasTable())
            {
                if (set.Contains(npc.GetTable().Name))
                {
                    npc.GoToFinalState();
                }
                else
                {
                    set.Add(npc.GetTable().Name);
                }
            }
        }
    }

    private void LoadUserObjects()
    {
        if(PlayerData.GetFirebaseGameUser().OBJECTS == null){
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
        if (employeeController != null) // Employee could not exist
        {
            // We cannot place on top of the employee
            if (position == BussGrid.GetPathFindingGridFromWorldPosition(employeeController.transform.position) || position == employeeController.CoordOfTableToBeAttended)
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
            employeeController.RecalculateState(obj);
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

    public EmployeeController GetEmployeeController()
    {
        return employeeController;
    }

    public static void PlaceGameObjectAt(FirebaseGameObject obj)
    {
        StoreItemType type = (StoreItemType)obj.ID;
        ObjectRotation rotation = (ObjectRotation)obj.ROTATION;
        Vector3Int position = new Vector3Int(obj.POSITION[0], obj.POSITION[1]);
        Vector3 worldPosition = BussGrid.GetWorldFromPathFindingGridPosition(position);
        string prefab = MenuObjectList.GetPrefab(type);

        Debug.Log("Placing object " + type + " " + prefab);

        if (prefab == "")
        {
            return;
        }

        GameObject newObj = Instantiate(Resources.Load(prefab, typeof(GameObject)), new Vector3(worldPosition.x, worldPosition.y, 1), Quaternion.identity, BussGrid.TilemapObjects.transform) as GameObject;
        BaseObjectController controller = newObj.GetComponent<BaseObjectController>();
        controller.SetGameGridObjectRotationAndFirebaseGameObject(obj, rotation);
    }
}