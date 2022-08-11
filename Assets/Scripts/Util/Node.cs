using UnityEngine;

public class Node
{
    private int x;
    private int y;

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Node(int[] a)
    {
        this.x = a[0];
        this.y = a[1];
    }

    public override string ToString()
    {
        return "[" + x + "," + y + "]";
    }

    public bool Compare(Node n)
    {
        return x == n.GetX() && y == n.GetY();
    }

    public float GetX()
    {
        return x;
    }

    public float GetY()
    {
        return y;
    }

    public Vector3 GetVector3()
    {
        return new Vector3(x, y, Settings.DEFAULT_GAME_OBJECTS_Z);
    }

    public Vector3Int GetVector3Int()
    {
        return new Vector3Int(x, y);
    }
}