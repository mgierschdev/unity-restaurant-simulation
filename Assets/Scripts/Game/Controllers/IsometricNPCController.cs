using System.Collections.Generic;
using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class IsometricNPCController : GameIsometricMovement
{
    [SerializeField]
    public Vector3 Velocity { get; set; }
    [SerializeField]
    public NPCState state; // 0 IDLE, 1 Wander
    [SerializeField]
    private EnergyBarController energyBar;
    [SerializeField]
    private float startX;
    [SerializeField]
    private float startY;
    [SerializeField]
    private float X;
    [SerializeField]
    private float Y;
    [SerializeField]
    private float currentEnergy = Settings.NPC_DEFAULT_ENERGY;

    // Wander variables
    private int distance = 3; //Cell units: How far you should wander from start position
    private float idleTime = 0;
    private float idleMaxTime = 3f; //in seconds

    private void Start()
    {
        //  body = GetComponent<Rigidbody2D>();
        Type = ObjectType.NPC;
        Speed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
        state = (int)NPCState.IDLE;

        // Energy bar
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBarController>();
        // Wandering 

        Vector3Int position = GameGrid.GetLocalGridFromWorldPosition(transform.position);

        startX = (float)position.x;
        startY = (float)position.y;

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

    public override void UpdatePosition()
    {
        Vector3Int pos = GameGrid.GetPathFindingGridFromWorldPosition(transform.position);
        sortingLayer.sortingOrder = pos.y * -1;
        X = pos.x;
        Y = pos.y;
        Position = new Vector3(X, Y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }

    private void Wander()
    {

        if (!IsMoving())
        {
            // we could add more random by deciding to move or not 
            idleTime += Time.deltaTime;
        }

        // Debug.Log("Wandering "+X+" "+Y+" "+!IsMoving()+" "+idleTime+ " "+idleMaxTime);

        if (!IsMoving() && idleTime >= idleMaxTime)
        {
            List<Node> path;
            int randx;
            int randy;
            idleTime = 0;

            Debug.Log("Wander");

            randx = Mathf.FloorToInt(Random.Range(-distance, distance) + X);
            randy = Mathf.FloorToInt(Random.Range(-distance, distance) + Y);

            Debug.Log("Adding path " + X + " " + Y + " to " + randx + " " + randy);
            // It should be mostly free, if invalid it will return an empty path
            path = GameGrid.GetPath(new int[] { (int)X, (int)Y }, new int[] { randx, randy });

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
}