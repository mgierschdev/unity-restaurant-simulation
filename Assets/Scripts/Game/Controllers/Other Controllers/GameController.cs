using System;
using System.Collections.Generic;
using Game.Controllers.Grid_Objects_Controllers;
using Game.Controllers.NPC_Controllers;
using Game.Grid;
using Game.Players;
using Game.Players.Model;
using UnityEngine;
using Util;
using IEnumerator = System.Collections.IEnumerator;

// This class in charge of loading the game and prefabs
// This handles the actions of all NPCS, cancel actions in case a table/object moves/it is stored
namespace Game.Controllers.Other_Controllers
{
    public class GameController : MonoBehaviour
    {
        private int _npcMaxNumber = Settings.NpcMultiplayer, _npcId;
        private GameObject _gameGridObject, _npcs;
        private GameTile _tileSpawn;
        private HashSet<ClientController> _clientSet;
        private HashSet<EmployeeController> _employeeSet;
        private HashSet<Vector3Int> _playerPositionSet, _employeePlannedTarget;

        private void Start()
        {
            _npcId = 0;
            _clientSet = new HashSet<ClientController>();
            _employeeSet = new HashSet<EmployeeController>();
            _playerPositionSet = new HashSet<Vector3Int>();
            _employeePlannedTarget = new HashSet<Vector3Int>();
            _npcs = GameObject.Find(Settings.TilemapObjects).gameObject;
            UpdateClientNumber();
            LoadUserObjects();
            //Assign tables to attend to employees
            StartCoroutine(AssignTablesToNpCs());
            //Spam new NPC is there is missing npcs
            StartCoroutine(NpcSpam());
        }

        private IEnumerator NpcSpam()
        {
            for (;;)
            {
                try
                {
                    if (_clientSet.Count < _npcMaxNumber)
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

        private IEnumerator AssignTablesToNpCs()
        {
            for (;;)
            {
                try
                {
                    if (GetFreeTable(out GameGridObject table))
                    {
                        var minDistance = double.MaxValue;
                        ClientController minDistanceNpc = null;

                        foreach (ClientController npcController in _clientSet)
                        {
                            if (!npcController.HasTable())
                            {
                                var localMin = Util.Util.EuclideanDistance(table.GridPosition, npcController.Position);

                                if (localMin < minDistance)
                                {
                                    minDistanceNpc = npcController;
                                    minDistance = localMin;
                                }
                            }
                        }

                        if (minDistanceNpc != null)
                        {
                            table.SetUsedBy(minDistanceNpc);
                            minDistanceNpc.SetTable(table);
                        }
                    }

                    if (GetTableWithClient(out GameGridObject tableToAttend))
                    {
                        foreach (EmployeeController employeeController in _employeeSet)
                        {
                            if (!employeeController.IsAttendingTable() &&
                                employeeController.GetNpcState() == NpcState.AtCounter)
                            {
                                // we match an available item  with a client order
                                var clientNpc = tableToAttend.GetUsedBy();

                                if (GetAvailableItem(clientNpc.GetItemToAskFor()) != null)
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
            foreach (KeyValuePair<string, GameGridObject> g in BussGrid.GetGameGridObjectsDictionary())
            {
                if (GameObjectList.GetItemGivenStoreItem(g.Value.GetStoreGameObject().StoreItemType) == item &&
                    g.Value.GetIsItemReady())
                {
                    g.Value.DiableTopInfoObject();
                    return g.Value;
                }
            }

            return null;
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
                if (!obj.isStored)
                {
                    PlaceGameObjectAt(obj);
                }
            }
        }

        private void SpamNpc()
        {
            _tileSpawn = BussGrid.GetRandomSpamPointWorldPosition();
            var spamPosition = _tileSpawn.GetWorldPositionWithOffset();
            spamPosition.z = Util.Util.NpcPositionZ;
            var npcObject = Instantiate(Resources.Load(Settings.PrefabNpcClient, typeof(GameObject)), spamPosition,
                Quaternion.identity) as GameObject;
            npcObject.transform.SetParent(_npcs.transform);
            npcObject.name = _npcId + "-" + Settings.PrefabNpcClient;
            var isometricNpcController = npcObject.GetComponent<ClientController>();
            _clientSet.Add(isometricNpcController);
            _npcId++;
        }

        private void SpamEmployee(GameGridObject counter)
        {
            _tileSpawn = BussGrid.GetRandomSpamPointWorldPosition();
            Vector3 spamPosition = _tileSpawn.GetWorldPositionWithOffset();
            spamPosition.z = Util.Util.NpcPositionZ;
            GameObject employeeObject = Instantiate(Resources.Load(Settings.PrefabNpcEmployee, typeof(GameObject)),
                spamPosition, Quaternion.identity) as GameObject;
            employeeObject.transform.SetParent(_npcs.transform);
            employeeObject.name = _npcId + "-" + Settings.PrefabNpcEmployee;
            EmployeeController employeeController = employeeObject.GetComponent<EmployeeController>();
            employeeController.SetCounter(counter);
            counter.SetAssignedTo(employeeController);
            _employeeSet.Add(employeeController);
            _npcId++;
        }

        public void RemoveNpc(ClientController controller)
        {
            if (controller == null)
            {
                return;
            }

            if (_clientSet.Contains(controller))
            {
                _clientSet.Remove(controller);
            }
            else
            {
                GameLog.LogWarning("GameController/RemoveNPC NPC Controller does not exist");
            }
        }

        public void RemoveEmployee(EmployeeController employeeController)
        {
            _employeeSet.Remove(employeeController);
        }

        public bool PositionOverlapsNpc(Vector3Int position)
        {
            foreach (EmployeeController employeeController in _employeeSet)
            {
                if (position == BussGrid.GetPathFindingGridFromWorldPosition(employeeController.transform.position) ||
                    position == employeeController.GetCoordOfTableToBeAttended())
                {
                    return true;
                }
            }

            foreach (ClientController npcController in _clientSet)
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
            foreach (EmployeeController employeeController in _employeeSet)
            {
                employeeController.RecalculateGoTo();
            }

            foreach (ClientController npcController in _clientSet)
            {
                npcController.RecalculateGoTo();
            }
        }

        public HashSet<ClientController> GetNpcSet()
        {
            return _clientSet;
        }

        public HashSet<EmployeeController> GetEmployeeSet()
        {
            return _employeeSet;
        }

        // Used for debug
        public ClientController GetNpc(string id)
        {
            foreach (ClientController npc in _clientSet)
            {
                if (npc.Name == id)
                {
                    return npc;
                }
            }

            return null;
        }

        // Used for debug
        public EmployeeController GetEmployee(string id)
        {
            foreach (EmployeeController npc in _employeeSet)
            {
                if (npc.Name == id)
                {
                    return npc;
                }
            }

            return null;
        }

        public void PlaceGameObjectAt(DataGameObject obj)
        {
            StoreItemType type = (StoreItemType)obj.id;
            Vector3Int position = new Vector3Int(obj.position[0], obj.position[1]);
            Vector3 worldPosition = BussGrid.GetWorldFromPathFindingGridPosition(position);
            string prefab = GameObjectList.GetPrefab(type);

            if (prefab == "")
            {
                return;
            }

            GameObject newObj = Instantiate(Resources.Load(prefab, typeof(GameObject)),
                new Vector3(worldPosition.x, worldPosition.y, Util.Util.ObjectZPosition), Quaternion.identity,
                BussGrid.TilemapGameFloor.transform) as GameObject;
            BaseObjectController controller = newObj.GetComponent<BaseObjectController>();
            controller.SetDataGameObjectAndInitRotation(obj);
            controller.SetStoreGameObject(GameObjectList.GetStoreObject((StoreItemType)obj.id));
        }

        public void AddPlayerPositions(Vector3Int position)
        {
            _playerPositionSet.Add(position);
        }

        public void RemovePlayerPosition(Vector3Int position)
        {
            _playerPositionSet.Remove(position);
        }

        public HashSet<Vector3Int> GetPlayerPositionSet()
        {
            return _playerPositionSet;
        }

        public void AddEmployeePlannedTarget(Vector3Int position)
        {
            _employeePlannedTarget.Add(position);
        }

        public void RemoveEmployeePlannedTarget(Vector3Int position)
        {
            _employeePlannedTarget.Remove(position);
        }

        public bool IsPathPlannedByEmployee(Vector3Int position)
        {
            return _employeePlannedTarget.Contains(position);
        }

        public void PrintDebugEmployeePlannedPaths()
        {
            string debug = "PrintDebugEmployeePlannedPaths: " + _employeePlannedTarget.Count;
            foreach (Vector3Int pos in _employeePlannedTarget)
            {
                debug += " " + pos + " ";
            }

            GameLog.Log(debug);
        }

        public void PrintDebugPlayerPositionsSet()
        {
            string debug = "PrintDebugPlayerPositionsSet: " + _playerPositionSet.Count;
            foreach (Vector3Int pos in _playerPositionSet)
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
                    tmp.GetUsedBy().GetNpcState() == NpcState.WaitingToBeAttended)
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
                if (g.Value.GetIsItemBought() && !ObjectDraggingHandler.IsThisSelectedObject(g.Key) &&
                    !g.Value.IsItemAssignedTo() && g.Value.Type == ObjectType.NpcCounter)
                {
                    return g.Value;
                }
            }

            return null;
        }


        // Upgrades
        public void UpdateClientNumber()
        {
            _npcMaxNumber = Settings.NpcMultiplayer * (PlayerData.GetUgrade(UpgradeType.NumberClients) == 0
                ? 1
                : PlayerData.GetUgrade(UpgradeType.NumberClients));
        }
    }
}