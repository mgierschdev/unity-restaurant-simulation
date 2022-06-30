using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{

    private Rigidbody2D body;
    [SerializeField]
    private float movementSpeed = 100;

    public Vector2 decisionTime = new Vector2(1, 4);
    internal Vector3[] moveDirections = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.zero, Vector3.zero };
    internal int currentMoveDirection;
    internal float decisionTimeCount = 0;


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
            // Choose a random time delay for taking a decision ( changing direction, or standing in place for a while )
            decisionTimeCount = Random.Range(decisionTime.x, decisionTime.y);

            // Choose a movement direction, or stay in place
            ChooseMoveDirection();
        }


    }

    void ChooseMoveDirection()
    {
        currentMoveDirection = Mathf.FloorToInt(Random.Range(0, moveDirections.Length));

    }
}
