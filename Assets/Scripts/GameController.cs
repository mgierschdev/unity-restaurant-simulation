using System.Collections.Generic;
using Game.Players;
using TMPro;
using UnityEngine;
// This class in charge of loading the game and prefabs
public class GameController : MonoBehaviour
{
    public HashSet<NPCController> NpcSet {get; set;}
    public GameObject EmployeeObject {get; set;}
    private const int NPC_MAX_NUMBER = 10;
    private int npcId;
    private GridController gridController;
    private GameObject gameGridObject;
    private GameTile tileSpawn;
    private PlayerData playerData;
    GameObject NPCS;

    private void Start()
    {
        npcId = 0;
        NpcSet = new HashSet<NPCController>();
        gameGridObject = gameObject.transform.Find(Settings.GameGrid).gameObject;
        gridController = gameGridObject.GetComponent<GridController>();
        gridController.PlayerData = playerData;
        // Setting up Current money
        GameObject topResourcePanelMoney = GameObject.Find(Settings.ConstTopMenuDisplayMoney);
        TextMeshProUGUI moneyText = topResourcePanelMoney.GetComponent<TextMeshProUGUI>();
        playerData = new PlayerData(20000, moneyText);
        gridController.PlayerData = playerData;
        NPCS = GameObject.Find(Settings.NPCS).gameObject;
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
        EmployeeObject = Instantiate(Resources.Load(Settings.PrefabNpcEmployee, typeof(GameObject)), tileSpawn.WorldPosition, Quaternion.identity) as GameObject;
        EmployeeObject.transform.SetParent(NPCS.transform);
        EmployeeObject.name = npcId + "-" + Settings.PrefabNpcEmployee;
        //EmployeeController employeeController = employeeObject.GetComponent<EmployeeController>();
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
}