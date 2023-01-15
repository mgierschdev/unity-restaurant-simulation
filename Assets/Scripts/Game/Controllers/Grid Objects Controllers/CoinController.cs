using UnityEngine;

public class CoinController : MonoBehaviour
{
    private bool consume;
    private Vector3 targetPosition;
    private float coinSpeed = 0.1f;
    private Vector3 startPoint;
    private float interpolationTime;

    private void Awake()
    {
        consume = false;
        transform.name = BussGrid.GetObjectID(ObjectType.COIN);
        startPoint = transform.position;
        interpolationTime = 0;

    }

    private void Update()
    {
        if (consume)
        {
            interpolationTime += Time.deltaTime * 0.1f;
            transform.position = MathExtended.EaseTest(interpolationTime, startPoint, targetPosition);
            Vector3 a = new Vector3(transform.position.x, transform.position.y, 0);
            Vector3 b = new Vector3(targetPosition.x, targetPosition.y, 0);
            Debug.Log(transform.name + " Target Distance+ " + Vector3.Distance(a, b));

            //Vector3.Lerp(transform.position, targetPosition, coinSpeed * Time.fixedDeltaTime);
            CheckIfAtTarget();
        }
        else if (Input.GetMouseButtonDown(0) && IsClickingSelf())
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(PlayerData.GetMoneyTextPosition());
            targetPosition = target;
            // TODO: Make it follow the reference of the actual gameObject rather than the coord in that way it will follow the coord even is the player moves with the perspective hand
            // Use some kind of lerp or curve easing 
            consume = true;
        }
    }

    private void CheckIfAtTarget()
    {
        Vector3 a = new Vector3(transform.position.x, transform.position.y, 0);
        Vector3 b = new Vector3(targetPosition.x, targetPosition.y, 0);

        if (Vector3.Distance(a, b) <= 0.1)
        {
            //We call here the increase coin method PlayerData
            Destroy(gameObject);
        }
    }

    private bool IsClickingSelf()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(Util.GetMouseInWorldPosition());
        foreach (Collider2D r in hits)
        {
            if (r.name.Contains(name))
            {
                return true;
            }
        }
        return false;
    }
}