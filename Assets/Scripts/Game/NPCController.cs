using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : MonoBehaviour
{
    private Rigidbody2D body;

    [SerializeField]
    private Vector3 velocity;
    [SerializeField]
    private Vector3 direction;
    [SerializeField]
    private int currentDirection;
    [SerializeField]
    private float movementSpeed = Settings.NPC_MOVEMENT_SPEED;
    private Vector3[] directions = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down};


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        currentDirection = 0;
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        direction = directions[currentDirection];
        velocity = (direction * Time.deltaTime).normalized * movementSpeed;
        body.velocity = velocity;
        body.angularVelocity = 0;
        body.rotation = 0;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Settings.TAG_OBSTACLE)
        {
            changeOppositeDirection(currentDirection);
        }
    }

    private void OnMouseDown()
    {
        movementSpeed += Settings.NPC_MOVEMENT_INCREASE_ON_CLICK;
    }
}