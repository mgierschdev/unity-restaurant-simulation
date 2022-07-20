using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Controls player properties
// Attached to: Player Object
public class PlayerController : MonoBehaviour, IGameObject
{

    private ObjectType type = ObjectType.PLAYER;
    [SerializeField]
    private float speed = Settings.PLAYER_MOVEMENT_SPEED;
    private Vector2 movement;
    private Vector3 position;
    private Rigidbody2D body;
    private GameGridController gameGrid;
    private int x;
    private int y;

    //Movement OnClick, vars
    private Queue pendingMovementQueue;
    private Vector3 nextTarget;
    private Vector3 currentTargetPosition;
    private GameObject gameGridObject;

    //MovingOnLongtouch(), Long click or touch vars

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        gameGridObject = GameObject.FindGameObjectWithTag(Settings.PREFAB_GAME_GRID);

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
        body.angularVelocity = 0;
        body.rotation = 0;

        // In case of keyboard
        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed;
        if (Input.anyKeyDown == true)
        {
            Debug.Log("anyKeyDown - > ResetMovementQueue");
            ResetMovementQueue();
        }

        // Player Movement
        MouseOnClick();

        // Player Moving on long click/touch
        MovingOnLongtouch();

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
    }

    public void AddPath(List<Node> list)
    {
        Util.AddPath(list, gameGrid, pendingMovementQueue);
        AddMovement(); // To set the first target
    }

    private void MovingOnLongtouch()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector3 delta = mousePosition - transform.position;
            
            float radians = Mathf.Atan2(delta.x, delta.y);
            float degrees = radians * Mathf.Rad2Deg; 

            // normalizing -180-180, 0-360
            if(degrees < 0){
                degrees += 360;
            }

            if (Settings.DEBUG_ENABLE)
            {
                Debug.Log("Angle: "+ degrees +" direction "+Util.GetDirectionFromAngles(degrees));
                Debug.DrawLine(transform.position, mousePosition, Color.blue);
            }
        }
    }

    private void MouseOnClick()
    {
        UpdateTargetMovement();
        if (Input.GetMouseButtonDown(0))
        {
            // If the player is moving, we change direction and empty the previous queue
            if (pendingMovementQueue.Count != 0)
            {
                ResetMovementQueue();
            }

            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector2Int mousePositionVector = Util.GetXYInGameMap(mousePosition);
            List<Node> path = gameGrid.GetPath(new int[] { (int)x, (int)y }, new int[] { mousePositionVector.x, mousePositionVector.y });
            Util.AddPath(path, gameGrid, pendingMovementQueue);

            if (pendingMovementQueue.Count != 0)
            {
                AddMovement();
            }
        }
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

    private void UpdatePosition()
    {
        Vector2Int pos = Util.GetXYInGameMap(transform.position);
        x = pos.x;
        y = pos.y;
        position = new Vector3(x, y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }

    // Resets the planned Path
    private void ResetMovementQueue()
    {
        nextTarget = Vector3.zero;
        pendingMovementQueue = new Queue();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // In case of collading stop moving
        ResetMovementQueue();
    }

    public float GetX()
    {
        return x;
    }

    public float GetY()
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