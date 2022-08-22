using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class IsometricNPCController : GameIsometricMovement
{
    [SerializeField]
    private EnergyBarController energyBar;
    [SerializeField]
    private float currentEnergy = Settings.NPC_DEFAULT_ENERGY;
    [SerializeField]
    public NPCState state; // 0 IDLE, 1 Wander
    [SerializeField]
    public string Name { get; set; }
    // Debug attributes
    [SerializeField]
    public string NPCDebug { get; set; }
    [SerializeField]
    private Queue<string> stateHistory;
    [SerializeField]
    private int stateHistoryMaxSize = 10;

    // Wander variables
    [SerializeField]
    private float idleTime = 0;
    [SerializeField]
    private float idleMaxTime = 3f; //in seconds

    private void Start()
    {
        //  body = GetComponent<Rigidbody2D>();
        Type = ObjectType.NPC;
        Speed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
        state = (int)NPCState.IDLE;
        Name = transform.name;

        // Energy bar
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBarController>();

        // Debug parameters
        stateHistory = new Queue<string>();

        // Energy Bar
        if (Settings.NPC_ENERGY_ENABLED)
        {
            energyBar.SetActive();
        }
        else
        {
            energyBar.SetInactive();
        }
    }
    private void FixedUpdate()
    {
        // EnergyBar controller, only if it is active
        if (energyBar.IsActive())
        {
            if (currentEnergy > 0)
            {
                currentEnergy -= Time.deltaTime * 20f;
                energyBar.SetEnergy((int)currentEnergy);
            }
            else
            {
                currentEnergy = 100;
                energyBar.SetEnergy(Settings.NPC_DEFAULT_ENERGY);
            }
        }

        body.angularVelocity = 0;
        body.rotation = 0;

        UpdateTargetMovement();
        // Updating position in the Grid
        UpdatePosition();

        //Go an wander 
        if (state == NPCState.WANDER)
        {
            Wander();
        }
    }

    private void Wander()
    {
        if (!IsMoving())
        {
            // we could add more random by deciding to move or not 
            idleTime += Time.deltaTime;
        }

        if (!IsMoving() && idleTime >= idleMaxTime)
        {
            List<Node> path;
            idleTime = 0;
            Vector3Int position = GameGrid.GetRandomWalkableGridPosition();
            // It should be mostly free, if invalid it will return an empty path
            path = GameGrid.GetPath(new int[] { (int)X, (int)Y }, new int[] { position.x, position.y });
            AddStateHistory("Time: " + Time.fixedTime + " d: " + path.Count + " t: " + position.x + "," + position.y);
            AddPath(path);
        }
    }

    private void ActivateEnergyBar()
    {
        energyBar.SetActive();
    }

    private void AddEnergyBar()
    {
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBarController>();
    }

    public void SetNPCState(NPCState state)
    {
        this.state = state;
    }

    private void SetDebug()
    {
        NPCDebug = "";
        for (int i = 0; i < stateHistory.Count; i++)
        {
            NPCDebug += stateHistory.ElementAt(i) + "<br>";
        }
    }

    private void AddStateHistory(string s)
    {
        stateHistory.Enqueue(s);

        if (stateHistory.Count > stateHistoryMaxSize)
        {
            stateHistory.Dequeue();
        }
        SetDebug();
    }
}