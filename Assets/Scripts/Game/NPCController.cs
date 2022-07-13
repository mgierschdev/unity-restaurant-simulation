using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : MonoBehaviour
{
    private Rigidbody2D body;

    [SerializeField]
    private Vector3 velocity;
    private EnergyBar energyBar;
    private Vector3 currentTargetPosition;
    private GameGridController gameGrid;
    private NPCController current;
    private float x;
    private float y;
    private Vector3 position;
    private float currentEnergy = Settings.NPC_DEFAULT_ENERGY;
    private float speed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
    private ObjectType type = ObjectType.NPC;
    //Movement Variables
    private Vector3 nextTarget;
    private Queue pendingMovementQueue;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        current = GetComponent<NPCController>();

        // Energy bar
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBar>();

        // Movement Queue
        nextTarget = Vector3.zero;
        pendingMovementQueue = new Queue();

        //Update NPC initial position
        currentTargetPosition = transform.position;

        // Enery Bar
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
        // EneryBar controller, only if it is active
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

        // Handling player movement through a queue
        if (currentTargetPosition == transform.position && nextTarget != Vector3.zero)
        {
            nextTarget = Vector3.zero;
            if (pendingMovementQueue.Count != 0)
            {
                AddMovement((MoveDirection)pendingMovementQueue.Dequeue());
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

        // Updating position in the Grid
        UpdatePosition();

        if (Settings.DEBUG_ENABLE)
        {
            //MouseOnClick();
        }
    }

    private void MouseOnClick()
    {
        //Only if it is not moving already
        if (Input.GetMouseButtonDown(0) && pendingMovementQueue.Count == 0)
        {
            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector2Int mousePositionVector = Util.GetXYInGameMap(mousePosition);
            List<Node> path = gameGrid.GetPath(new int[] { (int)x, (int)y }, new int[] { mousePositionVector.x, mousePositionVector.y });

            // Just for drawing the path
            if (Settings.DEBUG_ENABLE)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    Vector3 from = gameGrid.GetCellPosition(path[i - 1].GetX(), path[i - 1].GetY(), 1);
                    Vector3 to = gameGrid.GetCellPosition(path[i].GetX(), path[i].GetY(), 1);
                    Debug.DrawLine(from, to, Color.magenta, 10f);
                }
            }

            if(path.Count == 0){
                //Path out of reach
                Debug.Log("Path out of reach");
                return;
            }

            AddPath(path);
        }
    }

    // Adds path to the NPC
    private void AddPath(List<Node> n)
    {
        Vector3 from = new Vector3((int)x, (int)y, 1); // Current NPC pos
        Vector3 to = new Vector3(n[0].GetX(), n[0].GetY(), 1);
        MoveDirection m = Util.GetDirectionFromVector(to - from);
        pendingMovementQueue.Enqueue(m);

        for (int i = 1; i < n.Count; i++)
        {
            from = gameGrid.GetCellPosition(n[i - 1].GetX(), n[i - 1].GetY(), 1);
            to = gameGrid.GetCellPosition(n[i].GetX(), n[i].GetY(), 1);
            m = Util.GetDirectionFromVector(to - from);
            pendingMovementQueue.Enqueue(m);
        }

        if (pendingMovementQueue.Count != 0)
        {
            AddMovement((MoveDirection)pendingMovementQueue.Dequeue());
        }
    }

    private Vector3 GetVectorFromDirection(MoveDirection d)
    {
        //in case it is MoveDirection.IDLE do nothing
        Vector3 dir = transform.position;

        if (d == MoveDirection.LEFT)
        {
            dir = Vector3.left;
        }
        else if (d == MoveDirection.RIGHT)
        {
            dir = Vector3.right;
        }
        else if (d == MoveDirection.UP)
        {
            dir = Vector3.up;
        }
        else if (d == MoveDirection.DOWN)
        {
            dir = Vector3.down;
        }
        else if (d == MoveDirection.DOWNLEFT)
        {
            dir = new Vector3(-1, -1, 0);
        }
        else if (d == MoveDirection.DOWNRIGHT)
        {
            dir = new Vector3(1, -1, 0);
        }
        else if (d == MoveDirection.UPLEFT)
        {
            dir = new Vector3(-1, 1, 0);
        }
        else if (d == MoveDirection.UPRIGHT)
        {
            dir = new Vector3(1, 1, 0);
        }
        return dir;
    }

    private void OnMouseDown()
    {
        AddMovement(MoveDirection.RIGHT);
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

    public void AddMovement(MoveDirection direction)
    {
        Vector3 nextTarget = GetVectorFromDirection(direction) + transform.position;
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

    public void SetGameGridController(GameGridController controller)
    {
        this.gameGrid = controller;
    }

    public int GetX()
    {
        return (int)x;
    }

    public int GetY()
    {
        return (int)y;
    }

    public ObjectType GetType()
    {
        return type;
    }

    public Vector3 GetPosition()
    {
        return position;
    }
}