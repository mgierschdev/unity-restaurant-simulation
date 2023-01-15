using UnityEngine;
public static class MathExtended
{


    // returns the value of a Bazier curve given 3 parameters 
    // The parameter  a/b start/finish and c/d tangent c/d
    // Definition: https://en.wikipedia.org/wiki/Bézier_curve
    // Scale 0 - 1 where (0,0) is the left bot place on the screen and (1,1) the top right corner

    public static Vector2 CubicBazier(float time, Vector3 start, Vector2 end, Vector3 tangent1, Vector3 tangent2)
    {
        // x = (1 - t) * (1 - t) * p[0].x + 2 * (1 - t) * t * p[1].x + t * t * p[2].x;
        // y = (1 - t) * (1 - t) * p[0].y + 2 * (1 - t) * t * p[1].y + t * t * p[2].y;
        float cx = 3 * (tangent1.x - start.x);
        float cy = 3 * (tangent1.y - start.y);

        float bx = 3 * (tangent2.x - tangent1.x) - cx;
        float by = 3 * (tangent2.y - tangent1.y) - cy;

        float ax = end.x - start.x - cx - bx;
        float ay = end.y - start.y - cy - by;

        float Cube = time * time * time;
        float Square = time * time;

        float resX = (ax * Cube) + (bx * Square) + (cx * time) + start.x;
        float resY = (ay * Cube) + (by * Square) + (cy * time) + start.y;

        return new Vector2(resX, resY);
    }

    // Generating an ease animation
    public static Vector2 EaseTest(float t, Vector3 start, Vector3 end)
    {
        return CubicBazier(t, start, end, new Vector3(0, 1), new Vector3(1, 0));
    }


}