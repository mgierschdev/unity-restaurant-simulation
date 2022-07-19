using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Controls player properties
// Attached to: Player Object
public class PlayerController : MonoBehaviour, IGameObject
{
    private ObjectType type = ObjectType.PLAYER;
    private float speed = Settings.PLAYER_MOVEMENT_SPEED;
    private Vector2 movement;
    private Vector3 position;
    private Rigidbody2D body;
    private GameGridController gameGrid;
    private int x;
    private int y;

    //Movement OnClick
    private Queue pendingMovementQueue;
    private Vector3 nextTarget;
    private Vector3 currentTargetPosition;


    private void Start()
    {
        body = GetComponent<Rigidbody2D>();

        GameObject gameGridObject = GameObject.Find(Settings.PREFAB_GAME_GRID);
        if (gameGridObject != null)
        {
            gameGrid = gameGridObject.GetComponent<GameGridController>();
        }
        else
        {
            Debug.LogWarning("PlayerController.cs/gameGridObject null");
        }

        // Movement Queue
        nextTarget = Vector3.zero;
        pendingMovementQueue = new Queue();

        //Update NPC initial position
        currentTargetPosition = transform.position;
    }

    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed;
        body.angularVelocity = 0;
        body.rotation = 0;

        UpdateTargetMovement();
        MouseOnClick();

        // Updating position in the Grid
        UpdatePosition();
    }

    private void UpdateTargetMovement()
    {
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

            if (path.Count == 0)
            {
                Debug.Log("Path out of reach");
                return;
            }

            AddPath(path);
        }
    }

    public void AddPath(List<Node> n)
    {
        Vector3 from = new Vector3((int)x, (int)y, 1); // Current Player pos
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

    public void AddMovement(MoveDirection direction)
    {
        Vector3 nextTarget = Util.GetVectorFromDirection(direction) + transform.position;
        this.nextTarget = nextTarget;
    }


    private void UpdatePosition()
    {
        Vector2Int pos = Util.GetXYInGameMap(transform.position);
        x = pos.x;
        y = pos.y;
        position = new Vector3(x, y, 1);
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public float[] GetPositionAsArray()
    {
        return new float[] { position.x, position.y };
    }

    // Only for unit testing
    public void SetTestGameGridController(GameGridController controller)
    {
        this.gameGrid = controller;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public ObjectType GetType()
    {
        return type;
    }
}
