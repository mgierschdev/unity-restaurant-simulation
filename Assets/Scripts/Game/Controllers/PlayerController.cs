using UnityEngine;
using System.Collections.Generic;

// Controls player properties
// Attached to: Player Object
public class PlayerController : GameObjectMovementBase
{
    // ClickController for the long click duration for the player
    private ClickController clickController;

    // Overlap spehre
    private void Start()
    {
        Type = ObjectType.PLAYER;
        Speed = Settings.PLAYER_MOVEMENT_SPEED;
        GameObject cController = GameObject.FindGameObjectWithTag(Settings.CONST_PARENT_GAME_OBJECT);
        clickController = cController.GetComponent<ClickController>();
    }

    // For Handling non-physics related objects
    private void Update()
    {
        // Player Movement on click
        if (Settings.PLAYER_WALK_ON_CLICK)
        {
            MouseOnClick();
        }

        // Player Moving on long click/touch
        MovingOnLongtouch();
    }

    // Called every physics step, Update called every frame
    private void FixedUpdate()
    {
        body.angularVelocity = 0;
        body.rotation = 0;

        // In case of keyboard
        if (Settings.PLAYER_WALK_WITH_KEYBOARD)
        {
            body.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Speed;
        }

        // Moves the character depending on the pendingQueue and next target
        UpdateTargetMovement();

        // Updating position in the Grid
        UpdatePosition();
    }

    private void MovingOnLongtouch()
    {
        if (clickController.IsLongClick)
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

    private void MouseOnClick()
    {
        if (Input.GetMouseButtonDown(0) && !clickController.IsLongClick)
        {
            UpdatePosition();

            Vector3 mousePosition = Util.GetMouseInWorldPosition();
            Vector2Int mouseInGridPosition = Util.GetXYInGameMap(mousePosition);
            List<Node> path = GameGrid.GetPath(new int[] { (int)X, (int)Y }, new int[] { mouseInGridPosition.x, mouseInGridPosition.y });
            AddPath(path);

            if (pendingMovementQueue.Count != 0)
            {
                AddMovement();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (Settings.DEBUG_ENABLE)
        {
            Debug.Log("Colliding with: " + other.GetType() + " " + other.ToString());
        }
    }
}