using System;
using System.Collections.Generic;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;
// This class in charge of loading the game and prefabs
// This handles the actions of all NPCS, cancel actions in case a table/object moves/it is stored
public class GameController : MonoBehaviour
{
    private int NpcMaxNumber = Settings.NpcMultiplayer, npcId;
    private GameObject gameGridObject, NPCS;
    private GameTile tileSpawn;
    private HashSet<NPCController> ClientSet;
    private HashSet<EmployeeController> EmployeeSet;
    private HashSet<Vector3Int> playerPositionSet, employeePlannedTarget;

    private void Start()
    {
        npcId = 0;
        ClientSet = new HashSet<NPCController>();
        EmployeeSet = new HashSet<EmployeeController>();
        playerPositionSet = new HashSet<Vector3Int>();
        employeePlannedTarget = new HashSet<Vector3Int>();
        NPCS = GameObject.Find(Settings.TilemapObjects).gameObject;
        UpdateClientNumber();
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

            Debug.Log("Number NPCs " + ClientSet.Count);

            try
            {
                if (ClientSet.Count < NpcMaxNumber)
                {
                    SpamNpc();
                }

                GameGridObject counter = GetFreeCounter();

                if (counter != null)
                {
                    SpamEmployee(counter);
                }
            }
            catch (Exception e)
            {
                GameLog.LogError("Exception thrown, GameController/NPCSpam(): " + e);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator AssignTablesToNPCs()
    {
        for (; ; )
        {
            try
            {
                if (GetFreeTable(out GameGridObject table))
                {
                    double minDistance = double.MaxValue;
                    NPCController minDistanceNPC = null;

                    foreach (NPCController npcController in ClientSet)
                    {
                        if (!npcController.HasTable())
                        {
                            double localMin = Util.EuclidianDistance(table.GridPosition, npcController.Position);

                            if (localMin < minDistance)
                            {
                                minDistanceNPC = npcController;
                                minDistance = localMin;
                            }
                        }
                    }

                    if (minDistanceNPC != null)
                    {
                        table.SetUsedBy(minDistanceNPC);
                        minDistanceNPC.SetTable(table);
                    }
                }

                if (GetTableWithClient(out GameGridObject tableToAttend))
                {
                    foreach (EmployeeController employeeController in EmployeeSet)
                    {
                        if (!employeeController.IsAttendingTable() && employeeController.GetNpcState() == NpcState.AT_COUNTER)
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
            }
            catch (Exception e)
            {
                GameLog.LogError("Exception thrown, GameController/AssignTablesToNPCs(): " + e);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private GameGridObject GetAvailableItem(ItemType item)
    {
        GameGridObject objectFound = null;
        foreach (KeyValuePair<string, GameGridObject> g in BussGrid.GetGameGridObjectsDictionary())
        {
            if (MenuObjectList.GetItemGivenStoreItem(g.Value.GetStoreGameObject().StoreItemType) == item &&
            g.Value.GetIsItemReady())
            {
                g.Value.DiableTopInfoObject();
                return objectFound = g.Value;
            }
        }
        return objectFound;
    }

    private void LoadUserObjects()
    {
        // The user can store all the inventory 
        if (PlayerData.GerUserObjects() == null)
        {
            return;
        }

        foreach (DataGameObject obj in PlayerData.GerUserObjects())
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
        ClientSet.Add(isometricNPCController);
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

        if (ClientSet.Contains(controller))
        {
            ClientSet.Remove(controller);
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

        foreach (NPCController npcController in ClientSet)
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

        foreach (NPCController npcController in ClientSet)
        {
            npcController.RecalculateGoTo();
        }
    }

    public HashSet<NPCController> GetNpcSet()
    {
        return ClientSet;
    }

    public HashSet<EmployeeController> GetEmployeeSet()
    {
        return EmployeeSet;
    }

    // Used for debug
    public NPCController GetNPC(string ID)
    {
        foreach (NPCController npc in ClientSet)
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

    public void PlaceGameObjectAt(DataGameObject obj)
    {
        StoreItemType type = (StoreItemType)obj.ID;
        Vector3Int position = new Vector3Int(obj.POSITION[0], obj.POSITION[1]);
        Vector3 worldPosition = BussGrid.GetWorldFromPathFindingGridPosition(position);
        string prefab = MenuObjectList.GetPrefab(type);

        if (prefab == "")
        {
            return;
        }

        GameObject newObj = Instantiate(Resources.Load(prefab, typeof(GameObject)), new Vector3(worldPosition.x, worldPosition.y, Util.ObjectZPosition), Quaternion.identity, BussGrid.TilemapGameFloor.transform) as GameObject;
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

    // Returns a free table to the NPC, if there is one 
    public bool GetFreeTable(out GameGridObject result)
    {
        result = null;
        foreach (KeyValuePair<GameGridObject, byte> keyPair in TableHandler.GetBussQueueMap().ToArray())
        {
            GameGridObject tmp = keyPair.Key;

            if (tmp == null)
            {
                continue;
            }

            // GameLog.Log("GetFreeTable(): " +
            // tmp.IsFree() + " " +
            // !tmp.GetIsObjectBeingDragged() + " " +
            // !tmp.GetBusy() + " " +
            // !PlayerData.IsItemStored(tmp.Name) + " " +
            // tmp.Name + " " +
            // PlayerData.IsItemInInventory(tmp) + " " +
            // tmp.GetIsItemBought() + " " +
            // tmp.GetActive());

            if (tmp.IsFree() &&
            !tmp.GetIsObjectBeingDragged() &&
            !tmp.GetBusy() &&
            !PlayerData.IsItemStored(tmp.Name) &&
            PlayerData.IsItemInInventory(tmp) &&
            tmp.GetIsItemBought() &&
            tmp.GetActive() &&
            !tmp.GetIsObjectSelected())
            {
                result = tmp;
                return true;
            }
        }
        return false;
    }

    // Returns a table to the NPC Employee, if there is one 
    public bool GetTableWithClient(out GameGridObject result)
    {
        result = null;
        foreach (KeyValuePair<GameGridObject, byte> keyPair in TableHandler.GetBussQueueMap().ToArray())
        {
            GameGridObject tmp = keyPair.Key;

            if (tmp == null)
            {
                continue;
            }

            // GameLog.Log("GetTableWithClient(): " +
            //  (tmp == null) + " " +
            // !tmp.HasEmployeeAssigned() + " " +
            // tmp.HasClient() + " " +
            // !tmp.GetIsObjectBeingDragged() + " " +
            // !PlayerData.IsItemStored(tmp.Name) + " " +
            // (tmp.GetUsedBy().GetNpcState() == NpcState.WAITING_TO_BE_ATTENDED) + " ");

            if (
            !tmp.HasAttendedBy() &&
            tmp.HasClient() &&
            !tmp.GetIsObjectBeingDragged() &&
            !PlayerData.IsItemStored(tmp.Name) &&
            tmp.GetUsedBy().GetNpcState() == NpcState.WAITING_TO_BE_ATTENDED)
            {
                result = tmp;
                return true;
            }
        }

        return false;
    }

    public static GameGridObject GetFreeCounter()
    {
        foreach (KeyValuePair<string, GameGridObject> g in BussGrid.GetGameGridObjectsDictionary())
        {
            if (g.Value.GetIsItemBought() && !ObjectDraggingHandler.IsThisSelectedObject(g.Key) && !g.Value.IsItemAssignedTo() && g.Value.Type == ObjectType.NPC_COUNTER)
            {
                return g.Value;
            }
        }
        return null;
    }


    // Upgrades
    public void UpdateClientNumber()
    {
        NpcMaxNumber = Settings.NpcMultiplayer * (PlayerData.GetUgrade(UpgradeType.NUMBER_CLIENTS) == 0 ? 1 : PlayerData.GetUgrade(UpgradeType.NUMBER_CLIENTS));
    }

    public void UpgradeClientMaxWaitTime()
    {
        foreach (NPCController client in ClientSet)
        {
            client.UpdateMaxTableWaitingTime();
        }
    }
}