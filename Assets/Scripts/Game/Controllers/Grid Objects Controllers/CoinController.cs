using UnityEngine;

public class CoinController : MonoBehaviour
{
    private bool consume;
    private Transform target;
    private float coinSpeed = 0.5f;
    private Vector3 startPoint, targetPosition;
    private Vector3 tangent1 = new Vector3(0, 1), tangent2 = new Vector3(1, 0);
    private float interpolationTime;
    // Helper to build the graph: https://cubic-bezier.com/
    //Debug
    //private Vector3 prevPosition = Vector3.zero;

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
            targetPosition = Camera.main.ScreenToWorldPoint(target.transform.position);

            interpolationTime += Time.deltaTime * coinSpeed;
            transform.position = CubicBezier(interpolationTime,
            startPoint,
            startPoint + tangent1,
            targetPosition + tangent2,
            targetPosition);

            // Vector3 a = new Vector3(transform.position.x, transform.position.y, 0);
            // Vector3 b = new Vector3(targetPosition.x, targetPosition.y, 0);
            //Debug.Log(transform.name + " Target Distance " + Vector3.Distance(a, b));
            // Debug.DrawLine(prevPosition, transform.position, Color.cyan, 15f);
            //prevPosition = transform.position;
            //Vector3.Lerp(transform.position, targetPosition, coinSpeed * Time.fixedDeltaTime);

            CheckIfAtTarget();
        }
        else if (Input.GetMouseButtonDown(0) && IsClickingSelf())
        {
            target = PlayerData.GetMoneyTextTransform();
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

    // returns the value of a Bazier curve given 3 parameters 
    // Timt 1 , and set of point from p0  to p3 (P(n)), where p0 is the start and the last the end of the curve
    // Definition: https://en.wikipedia.org/wiki/Bézier_curve
    // Scale 0 - 1 where (0,0) is the left bot place on the screen and (1,1) the top right corner

    public static Vector3 CubicBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // Cubic Bezier formula
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}