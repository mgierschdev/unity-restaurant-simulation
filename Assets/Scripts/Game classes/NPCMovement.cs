using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCMovement : MonoBehaviour
{
    private Rigidbody2D body;

    [SerializeField]
    private Vector3 velocity;
    [SerializeField]
    private Vector3 direction;
    [SerializeField]
    private Vector3 screenPos;
    [SerializeField]
    private int currentDirection;
    [SerializeField]
    private float timeToComeBack = Settings.NPC_REACTION_TIME;
    [SerializeField]
    private float movementSpeed = Settings.NPC_MOVEMENT_SPEED;

    private Vector3[] directions = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down};
    private float screenHeight = Screen.height;
    private float screenWidth = Screen.width;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        currentDirection = 0;
    }

    void Update()
    {
        direction = directions[currentDirection];
        screenPos = Camera.main.WorldToScreenPoint(this.transform.position);

        velocity = (direction * Time.deltaTime).normalized * movementSpeed;
        body.velocity = velocity;
        body.angularVelocity = 0;
        body.rotation = 0;

        timeToComeBack -= Time.deltaTime;

        if ((screenPos.x < 0 || screenPos.y < 0 || screenPos.x > screenWidth || screenPos.y > screenHeight) && timeToComeBack < 0) // Change directions if going outside the screen
        {
            changeOppositeDirection(currentDirection);
            timeToComeBack = 2f;
        }

    }

    private void changeOppositeDirection(int direction)
    {
        if (direction == 0)
        {
            currentDirection = 1;
        } else if (direction == 1)
        {
            currentDirection = 0;
        }else if( direction == 3)
        {
            currentDirection = 4;
        }
        else
        {
            currentDirection = 3;
        }
    }

    // In case there is an action when the Npc gets clicked
    //private void OnMouseDown()
    //{
    //    //movementSpeed += 100;
    //}
}
