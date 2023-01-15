using UnityEngine;
public static class MathExtended
{
    // returns the value of a Bazier curve given 3 parameters 
    // Timt 1 , and set of point from p0  to p3 (P(n)), where p0 is the start and the last the end of the curve
    // Definition: https://en.wikipedia.org/wiki/Bézier_curve
    // Scale 0 - 1 where (0,0) is the left bot place on the screen and (1,1) the top right corner

    public static Vector3 CubicBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
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

    // Generating an ease animation
    public static Vector2 EaseTest(float t, Vector3 start, Vector3 end)
    {
        Debug.Log("Calculating Bazier from " + start + " " + " " + end);
        return CubicBezier(t, start, new Vector3(0, 1), new Vector3(1, 0), end);
    }
}