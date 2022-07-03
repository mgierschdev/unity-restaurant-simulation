using UnityEngine;

// Controls player properties
// Attached to: Player Object
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = Settings.PLAYER_MOVEMENT_SPEED;
    private Vector2 movement;
    private Animator animator;
    Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * movementSpeed;
        animator.SetFloat("Speed", Mathf.Abs(movement.magnitude * movementSpeed));
        bool flipped = movement.x < 0;
        //this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        body.angularVelocity = 0;
        body.rotation = 0;
    }

    //private void FixedUpdate()
    //{
    //    if(movement != Vector2.zero)
    //    {
    //        float movementInX = movement.x * movementSpeed * Time.deltaTime;
    //        this.transform.Translate(new Vector3(movementInX, 0), Space.World); // rotate base on the world and not local coordinate system
    //    }

    //    // To avoid rotation


     
    //    // To flip character
    //    //Vector3 characterScale = transform.localScale;
    //    //if(Input.GetAxis("Horizontal") < 0)
    //    //{
    //    //    transform.Rotate(0, 180, 0);
    //    //}
    //    //if (Input.GetAxis("Vertical") > 0)
    //    //{
    //    //    transform.Rotate(0, 180, 0);
    //    //}
    //    //transform.localScale = characterScale;
    //}

    //private void updateStand()
    //{
    //    if(body.velocity.x < 0)
    //    {

    //    }
    //    else
    //    {

    //    }
    //}
    
}
