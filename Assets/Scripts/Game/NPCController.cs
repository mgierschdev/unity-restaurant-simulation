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
    private float startX;
    private float startY;
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

    // Wander variables
    private int distance = 5; //Cell units: How far you should wander from start position
    private float idleTime = 0;
    private float idleMaxTime = 3f; //in seconds

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
        UpdatePosition();
        startX = x;
        startY = y;

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

        //Go an wander 
        Wander();
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

            // we send him back if he goes beyond the distance of the initial potition 
            if (x > startX + distance * 2 || y > startY + distance * 2)
            {
                randx = Mathf.FloorToInt(Random.Range(0, distance) + startX / 2);
                randy = Mathf.FloorToInt(Random.Range(0, distance) + startY / 2);
                path = gameGrid.GetPath(new int[] { (int)x, (int)y }, new int[] { randx, randy });
            }
            else
            {
                randx = Mathf.FloorToInt(Random.Range(0, distance) + x);
                randy = Mathf.FloorToInt(Random.Range(0, distance) + y);

                // It should be mostly free, if invalid it will return an empty path
                path = gameGrid.GetPath(new int[] { (int)x, (int)y }, new int[] { randx, randy });
            }

            AddPath(path);
        }
    }

    private bool IsMoving()
    {
        if (pendingMovementQueue.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    public void UpdateTargetMovement()
    {
        // Handling player movement through a queue
        if (currentTargetPosition == transform.position && nextTarget != Vector3.zero)
        {
            nextTarget = Vector3.zero;
            if (pendingMovementQueue.Count != 0)
            {
                AddMovement();
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

    public void AddPath(List<Node> list)
    {
        Util.AddPath(list, gameGrid, pendingMovementQueue);
        AddMovement(); // To set the first target
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
        position = new Vector3(x, y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }

    public void AddMovement()
    {
        if (pendingMovementQueue.Count == 0)
        {
            return;
        }
        Vector3 direction = (Vector3)pendingMovementQueue.Dequeue();
        Vector3 nextTarget = new Vector3(direction.x, direction.y, Settings.DEFAULT_GAME_OBJECTS_Z);//gameGrid.GetCellPosition(transform.position + direction) ;
        this.nextTarget = nextTarget;
    }

    public void AddMovement(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Vector3 newDirection = direction + transform.position;
            this.nextTarget = new Vector3(newDirection.x, newDirection.y, Settings.DEFAULT_GAME_OBJECTS_Z);
        }
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