using UnityEngine;

public class NPCMovement : MonoBehaviour
{

    private Rigidbody2D body;
    [SerializeField]
    private float movementSpeed = 150;
    [SerializeField]
    internal float decisionTimeCount = 0;
    private int timeToChangeMove = 10;
    public Vector2 decisionTime = new Vector2(1, 4);
    internal Vector3[] moveDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down};
    internal int currentMoveDirection;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);
        ChooseMoveDirection();

    }

    void Update()
    {
        Vector3 direction = moveDirections[currentMoveDirection];
        body.velocity = (direction * movementSpeed * Time.deltaTime).normalized;

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
}
