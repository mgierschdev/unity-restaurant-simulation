using UnityEngine;

// Controls NPCs players
// Attached to: NPC Objects
public class NPCController : MonoBehaviour
{
    private Rigidbody2D body;

    [SerializeField]
    private Vector3 velocity;

    //[SerializeField]
   // private int currentDirection;
    //[SerializeField]
    //private float movementSpeed = Settings.NPC_DEFAULT_MOVEMENT_SPEED;
    //[SerializeField]
    //private Vector3[] directions = new Vector3[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down};

    private EnergyBar energyBar;
    private float currentEnergy = Settings.NPC_DEFAULT_ENERGY;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        // Energy bar
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBar>();

        if (Settings.NPC_ENERGY_ENABLED)
        {
            energyBar.SetActive();

        }
        else
        {
            energyBar.SetInactive();
        }
    }

    private void Update()
    {
        // EneryBar controller, only if it is active
        if (energyBar.IsActive())
        {
            if (currentEnergy > 0)
            {
                currentEnergy -= Time.deltaTime * 20f;
                energyBar.SetEnergy((int)currentEnergy);
            }
            else
            {
                currentEnergy = 100;
                energyBar.SetEnergy(Settings.NPC_DEFAULT_ENERGY);
            }
        }

        body.angularVelocity = 0;
        body.rotation = 0;
        Debug.Log(body.rotation );
    }

    public void Move(MoveDirection d)
    {
        //in case it is MoveDirection.IDDLE do nothing
        Vector3 dir = transform.position;

        if (d == MoveDirection.LEFT)
        {
            dir = Vector3.left;
        }else if(d == MoveDirection.RIGHT)
        {
            dir = Vector3.right;
        }else if(d == MoveDirection.UP)
        {
            dir = Vector3.up;
        }else if(d == MoveDirection.DOWN)
        {
            dir = Vector3.down;
        }else if (d == MoveDirection.DOWNLEFT)
        {
            dir = new Vector3(-1, -1, 0);
        }else if(d == MoveDirection.DOWNRIGHT)
        {
             dir = new Vector3(1, -1, 0);
        }else if(d == MoveDirection.UPLEFT)
        {
            dir = new Vector3(-1, 1, 0);
        }else if(d == MoveDirection.UPRIGHT)
        {
            dir = new Vector3(1, 1, 0);
        }
        transform.Translate(dir, Space.World);
    }

    private void OnMouseDown()
    {
        Debug.Log("NPC Reacting on click");
    }

    public Vector3 GetVelocity()
    {
        return this.velocity;
    }

    public void ActivateEnergyBar()
    {
        energyBar.SetActive();
    }

    public void AddEnergyBar()
    {
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBar>();
    }

    public EnergyBar GetEnergyBar()
    {
        return this.energyBar;
    }

     public void SetPosition(Vector3 position){
       transform.position = position;
    }
}