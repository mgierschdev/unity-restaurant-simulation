using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCMovement : MonoBehaviour
{

   
    private InGameMenuController menuUIController;

    private Rigidbody2D body;
    [SerializeField]
    private float movementSpeed = 150;
    [SerializeField]
    private float decisionTimeCount = 0;
    private int timeToChangeMove = 10;
    private Vector2 decisionTime = new Vector2(1, 4);
    private Vector3[] moveDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down};
    private int currentMoveDirection;

    private void Awake()
    {
        menuUIController = GameObject.Find("Canvas UI").GetComponent<InGameMenuController>();
        body = GetComponent<Rigidbody2D>();
        decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);
        ChooseMoveDirection();

    }

    void Update()
    {
        Vector3 direction = moveDirections[currentMoveDirection];
        body.velocity = (direction * movementSpeed * Time.deltaTime).normalized;
        body.angularVelocity = 0;
        body.rotation = 0;

        if (decisionTimeCount > 0){
            decisionTimeCount -= Time.deltaTime;
        }else{
            decisionTimeCount = Random.Range(0, timeToChangeMove);

            ChooseMoveDirection();
        }

    }

    void ChooseMoveDirection()
    {
        currentMoveDirection = Mathf.FloorToInt(Random.Range(0, moveDirections.Length));

    }

    private void OnMouseDown()
    {
        menuUIController.score += 1;
        movementSpeed += 100;
    }
}
