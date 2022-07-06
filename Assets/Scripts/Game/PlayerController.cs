using UnityEngine;

// Controls player properties
// Attached to: Player Object
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = Settings.PLAYER_MOVEMENT_SPEED;
    private Vector2 movement;
    //private Animator animator;
    private Vector3 position;
    Rigidbody2D body;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
       // animator = GetComponent<Animator>();
    }

    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * movementSpeed;
        //animator.SetFloat("Speed", Mathf.Abs(movement.magnitude * movementSpeed));
        //bool flipped = movement.x < 0;
        //this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        body.angularVelocity = 0;
        body.rotation = 0;
    }

    public void SetPosition(Vector3 position){
       transform.position = position;

    }
}
