using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class GameIsometricMovement : GameObjectMovementBase, IMovement
{
    public IsometricGridController GameGrid { get; set; }

    // ClickController for the long click duration for the player
    private ClickController clickController;

    // Al objects in screen should have sorting group component
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sortingLayer = GetComponent<SortingGroup>();

        // Game Grid
        gameGridObject = GameObject.FindGameObjectWithTag(Settings.GAME_GRID);

        if (gameGridObject != null)
        {
            GameGrid = gameGridObject.GetComponent<IsometricGridController>();
        }
        else
        {
            if (Settings.DEBUG_ENABLE)
            {
                Debug.LogWarning("IsometricGridController.cs/gameGridObject null");
            }
        }

        // Movement Queue
        nextTarget = Vector3.zero;
        pendingMovementQueue = new Queue();

        //Update Object initial position
        currentTargetPosition = Vector3.negativeInfinity;
    }

    // Overlap spehre
    private void Start()
    {
        UpdatePosition();

        Type = ObjectType.PLAYER;
        Speed = Settings.PLAYER_MOVEMENT_SPEED;
        GameObject cController = GameObject.FindGameObjectWithTag(Settings.CONST_PARENT_GAME_OBJECT);

        if (cController != null)
        {
            clickController = cController.GetComponent<ClickController>();
        }
        else if (Settings.DEBUG_ENABLE)
        {
            Debug.LogWarning("PlayerController/clickController null");
        }
    }

    public override void UpdateTargetMovement()
    {
        if (currentTargetPosition == transform.position && nextTarget != Vector3.zero)
        {
            nextTarget = Vector3.zero;
            currentTargetPosition = Vector3.zero;

            if (pendingMovementQueue.Count != 0)
            {
                AddMovement();
            }
            else
            {
                if (Settings.DEBUG_ENABLE)
                {
                    //Debug.Log("[Moving] Target Reached: " + transform.name + " " + Position);
                }
            }
        }

        if (nextTarget != Vector3.zero)
        {
            currentTargetPosition = nextTarget;
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, Speed * Time.deltaTime);
        }
    }

    public List<Node> GetPath(int[] from, int[] to)
    {
        return GameGrid.GetPath(from, to);
    }

    private void MovingOnLongtouch()
    {
        if (clickController != null && clickController.IsLongClick)
        {
            ResetMovementIfMoving();

            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector3 delta = mousePosition - transform.position;

            float radians = Mathf.Atan2(delta.x, delta.y);
            float degrees = radians * Mathf.Rad2Deg;

            // normalizing -180-180, 0-360
            if (degrees < 0)
            {
                degrees += 360;
            }

            AddMovement(Util.GetVectorFromDirection(Util.GetDirectionFromAngles(degrees)));

            if (Settings.DEBUG_ENABLE)
            {
                Debug.DrawLine(transform.position, mousePosition, Color.blue);
            }
        }
    }

    public void AddPath(List<Node> path)
    {
        Debug.Log("Adding Path "+path.Count);

        if (path.Count == 0)
        {
            return;
        }

        Debug.Log("Movement queue "+pendingMovementQueue.Count);

        if (pendingMovementQueue.Count != 0)
        {
            path = MergePath(path); // We merge Paths
        }
        Debug.Log("Drawing Path");

        pendingMovementQueue.Enqueue(path[0].GetVector3());

        for (int i = 1; i < path.Count; i++)
        {
            Vector3 from = GameGrid.GetWorldFromPathFindingGridPosition(path[i - 1].GetVector3Int());
            Vector3 to = GameGrid.GetWorldFromPathFindingGridPosition(path[i].GetVector3Int());

            if (Settings.DEBUG_ENABLE)
            {
                Debug.DrawLine(from, to, Color.yellow, 15f);
            }

            pendingMovementQueue.Enqueue(path[i].GetVector3());
        }

        if (path.Count == 0)
        {
            Debug.LogWarning("Path out of reach");
            return;
        }

        AddMovement(); // To set the first target
    }

    public void MouseOnClick()
    {
        if (Input.GetMouseButtonDown(0) && clickController != null && !clickController.IsLongClick)
        {
            UpdatePosition();

            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector3Int mouseInGridPosition = GameGrid.GetPathFindingGridFromWorldPosition(mousePosition);
            List<Node> path = GetPath(new int[] { (int)X, (int)Y }, new int[] { mouseInGridPosition.x, mouseInGridPosition.y });
            AddPath(path);

            if (pendingMovementQueue.Count != 0)
            {
                AddMovement();
            }
        }
    }

    public override void AddMovement()
    {
        if (pendingMovementQueue.Count == 0)
        {
            return;
        }

        Vector3 queuePosition = (Vector3) pendingMovementQueue.Dequeue();
        Vector3 direction = GameGrid.GetWorldFromPathFindingGridPosition(new Vector3Int((int) queuePosition.x, (int) queuePosition.y));
        Vector3 nextTarget = new Vector3(direction.x, direction.y);
        this.nextTarget = nextTarget;
    }

    public void SetClickController(ClickController controller)
    {
        this.clickController = controller;
    }
}