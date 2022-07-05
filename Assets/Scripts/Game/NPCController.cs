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
            energyBar.setActive();

        }
        else
        {
            energyBar.setInactive();
        }
    }

    private void FixedUpdate()
    {
        // EneryBar controller, only if it is active
        if (energyBar.isActive())
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
    }

    public void move(MoveDirection d)
    {
        Vector3 dir;

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
            dir = Quaternion.Euler(45, 0, 0) * Vector3.down; // not working
        }else if(d == MoveDirection.DOWNRIGHT)
        {
            dir = Quaternion.Euler(-45, 0, 0) * Vector3.down;
        }else if(d == MoveDirection.UPLEFT)
        {
            dir = Quaternion.Euler(0, 45, 0) * Vector3.forward;
        }else if(d == MoveDirection.UPRIGHT)
        {
            dir = new Vector3(.5f, 0, .5f);
        }
        else
        {
            //in case it is iddle
            dir = new Vector3(0,0,0);
        }

        transform.Translate(dir, Space.World);
    }

    private void OnMouseDown()
    {
        Debug.Log(energyBar.isActive());
        //movementSpeed += Settings.NPC_DEFAULT_MOVEMENT_INCREASE_ON_CLICK;
        move(MoveDirection.UP);

    }

    public Vector3 getVelocity()
    {
        return this.velocity;
    }

    public void ActivateEnergyBar()
    {
        energyBar.setActive();
    }

    public void AddEnergyBar()
    {
        Debug.Log("Trying to instatiate energy bar");
        energyBar = gameObject.transform.Find(Settings.NPC_ENERGY_BAR).gameObject.GetComponent<EnergyBar>();
        Debug.Log(energyBar.isActive());
    }

    public EnergyBar GetEnergyBar()
    {
        return this.energyBar;
    }

}