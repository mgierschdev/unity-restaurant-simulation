using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : MonoBehaviour, IGameObject
{
    private Rigidbody2D body;

    [SerializeField]
    private Vector3 velocity;
    private EnergyBar energyBar;
    private GameGridController gameGrid;
    private float x;
    private float y;
    private Vector3 position;
    private float currentEnergy = Settings.NPC_DEFAULT_ENERGY;
    private float speed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
    private ObjectType type = ObjectType.NPC;

    //Movement Variables, in case commanded to move to a direction
    private Vector3 nextTarget;
    private Queue pendingMovementQueue;
    private Vector3 currentTargetPosition;
    private GameObject gameGridObject;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();

        // Energy bar
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBar>();

        // Game Grid
        gameGridObject = GameObject.FindGameObjectWithTag(Settings.PREFAB_GAME_GRID);

        if (gameGridObject != null)
        {
            gameGrid = gameGridObject.GetComponent<GameGridController>();
        }
        else
        {
            Debug.LogWarning("NPCController.cs/gameGridObject null");
        }

        // Movement Queue
        nextTarget = Vector3.zero;
        pendingMovementQueue = new Queue();

        //Update NPC initial position
        currentTargetPosition = transform.position;

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

    private void Update()
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
    }

    public void UpdateTargetMovement()
    {
        // Handling player movement through a queue
        if (currentTargetPosition == transform.position && nextTarget != Vector3.zero)
        {
            nextTarget = Vector3.zero;
            if (pendingMovementQueue.Count != 0)
            {
                AddMovement((Vector3)pendingMovementQueue.Dequeue());
            }
            else
            {
                // Target Reached
            }
        }

        if (nextTarget != Vector3.zero)
        {
            currentTargetPosition = nextTarget;
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, speed * Time.deltaTime);
        }
        // Handling player movement through a queue
    }

    // Adds path to the NPC
    public void AddPath(List<Node> n)
    {
        // Vector3 from = new Vector3((int)x, (int)y, 1); // Current NPC pos
        // Vector3 to = new Vector3(n[0].GetX(), n[0].GetY(), 1);
        // MoveDirection m = Util.GetDirectionFromVector(to - from);
        // pendingMovementQueue.Enqueue(m);

        // for (int i = 1; i < n.Count; i++)
        // {
        //     from = gameGrid.GetCellPosition(n[i - 1].GetX(), n[i - 1].GetY(), 1);
        //     to = gameGrid.GetCellPosition(n[i].GetX(), n[i].GetY(), 1);
        //     Vector3 newDirection = to - from;
        //     pendingMovementQueue.Enqueue(newDirection);
        // }

        // if (pendingMovementQueue.Count != 0)
        // {
        //     AddMovement((Vector3) pendingMovementQueue.Dequeue());
        // }
    }

    private void OnMouseDown()
    {

    }

    private void ActivateEnergyBar()
    {
        energyBar.SetActive();
    }

    private void AddEnergyBar()
    {
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBar>();
    }

    private EnergyBar GetEnergyBar()
    {
        return this.energyBar;
    }

    private Vector3 GetVelocity()
    {
        return this.velocity;
    }

    private void UpdatePosition()
    {
        Vector2Int pos = Util.GetXYInGameMap(transform.position);
        x = pos.x;
        y = pos.y;
        position = new Vector3(x, y, 1);
    }

    public void AddMovement(Vector3 direction)
    {
        Vector3 nextTarget = direction + transform.position;
        this.nextTarget = nextTarget;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    // Only for unit testing
    public void SetTestGameGridController(GameGridController controller)
    {
        this.gameGrid = controller;
    }

    public float GetX()
    {
        return x;
    }

    public float GetY()
    {
        return y;
    }

    public ObjectType GetType()
    {
        return type;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public float[] GetPositionAsArray()
    {
        return new float[] { position.x, position.y };
    }
}