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
    private float startX;
    [SerializeField]
    private float startY;
    [SerializeField]
    private float currentEnergy = Settings.NPC_DEFAULT_ENERGY;
    public NPCState state; // 0 IDLE, 1 Wander
    [SerializeField]
    public string Name { get; set; }
    // Debug attributes
    [SerializeField]
    public string Debug { get; set; }
    [SerializeField]
    private Queue<string> stateHistory;
    private int stateHistoryMaxSize = 5;

    // Wander variables
    private int distance = 6; //Cell units: How far you should wander from start position
    private float idleTime = 0;
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

        // Wandering 
        Vector3Int position = GameGrid.GetLocalGridFromWorldPosition(transform.position);

        startX = (float)position.x;
        startY = (float)position.y;

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
        body.velocity = new Vector2(0, 0);
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
            int randx;
            int randy;
            idleTime = 0;

            randx = Mathf.FloorToInt(Random.Range(-distance, distance) + X);
            randy = Mathf.FloorToInt(Random.Range(-distance, distance) + Y);

            // It should be mostly free, if invalid it will return an empty path
            path = GameGrid.GetPath(new int[] { (int)X, (int)Y }, new int[] { randx, randy });
            AddStateHistory(Time.fixedTime + " Moving distance: " + path.Count);
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
        Debug = "";
        for (int i = 0; i < stateHistory.Count; i++)
        {
            Debug += stateHistory.ElementAt(i) + "<br>";
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