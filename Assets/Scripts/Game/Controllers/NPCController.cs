using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : GameObjectMovementBase
{
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

    // Wander properties
    [SerializeField]
    private float idleTime = 0;
    [SerializeField]
    private float idleMaxTime = 6f; //in seconds
    private float randMax = 3f;

    //Doing a different activitiy properties
    private bool busy = false;
    GameGridObject table;

    private void Start()
    {
        Type = ObjectType.NPC;
        Speed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
        state = (int)NPCState.IDLE;
        Name = transform.name;

        // Debug parameters
        stateHistory = new Queue<string>();
    }

    private void FixedUpdate()
    {

        body.angularVelocity = 0;
        body.rotation = 0;

        UpdateTargetMovement();
        // Updating position in the Grid
        UpdatePosition();

        //Go and wander if not busy
        if (state == NPCState.WANDER && !busy)
        {
            FindPlace();
            Wander();
        }

        UpdateEnergyBar();
    }

    private bool FindPlace()
    {
        table = GameGrid.GetFreeTable();

        if (table != null)
        {
            busy = true;
            GoTo(table.GridPosition + new Vector3Int(2, 2, 0));// arrive one spot infront
            return true;
        }
        return false;
    }

    private void GoTo(Vector3Int pos)
    {
        List<Node> path = GameGrid.GetPath(new int[] { (int)Position.x, (int)Position.y }, new int[] { pos.x, pos.y });
        AddStateHistory("Time: " + Time.fixedTime + " d: " + path.Count + " t: " + pos.x + "," + pos.y);
        AddPath(path);
    }

    private void Wander()
    {
        if (!IsMoving())
        {
            // we could add more random by deciding to move or not 
            idleTime += Time.deltaTime;
        }

        if (!IsMoving() && idleTime >= randMax)
        {
            List<Node> path;
            idleTime = 0;
            randMax = Random.Range(0, idleMaxTime);
            Vector3Int position = GameGrid.GetRandomWalkableGridPosition();
            // It should be mostly free, if invalid it will return an empty path
            path = GameGrid.GetPath(new int[] { (int)Position.x, (int)Position.y }, new int[] { position.x, position.y });
            AddStateHistory("Time: " + Time.fixedTime + " d: " + path.Count + " t: " + position.x + "," + position.y);
            AddPath(path);
        }
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

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (Settings.DEBUG_ENABLE)
    //     {
    //         //Debug.Log("Colliding with: " + other.GetType() + " " + other.ToString());
    //     }
    // }
}