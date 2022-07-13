
public class PathNode
{
    public PathNode next; // use to order in the queue
    private PathNode parent; // Another reference comming from
    private int[] position;
    private int GCost; // G cost to the start point
    private int HCost; // H cost to the end point
    private double EuclidianCost;
    private int FCost; // value in the position, 0 free, 1 obstacle, 2 visited
    private int value;

    public PathNode(int[] position)
    {
        this.position = position;
        parent = null;
    }

    public PathNode(int[] position, int FCost)
    {
        this.position = position;
        this.FCost = FCost;
        parent = null;
    }

    public PathNode(int[] position, int costToStart, int costToEnd, PathNode parent)
    {
        this.position = position;
        this.GCost = costToStart;
        this.HCost = costToEnd;
        this.FCost = this.GCost + this.HCost;
        this.parent = parent;
    }

    public string CoordsToString()
    {
        return "[" + position[0] + "," + position[1] + "]";
    }

    public override string ToString()
    {
        string tmp;
        if (parent == null)
        {
            tmp = "null";
        }
        else
        {
            tmp = "[" + parent.GetX() + "," + parent.GetY() + "]";
        }

        return "[" + position[0] + "," + position[1] + "] Fcost: " + FCost + " GCost: " + GCost + " HCost: " + HCost + " Parent: " + tmp;
    }

    public void CalculateFCost()
    {
        this.FCost = this.GCost + this.HCost;
    }

    public int GetX()
    {
        return this.position[0];
    }

    public int GetY()
    {
        return this.position[1];
    }

    public int GetGCost()
    {
        return this.GCost;
    }

    public int GetHCost()
    {
        return this.HCost;
    }

    public double GetEuclidianCost()
    {
        return EuclidianCost;
    }

    public int[] GetPosition()
    {
        return position;
    }

    public void SetParent(PathNode parent)
    {
        this.parent = parent;
    }

    public PathNode GetParent()
    {
        return this.parent;
    }

    // F Cost = G + H
    public int GetFCost()
    {
        return this.FCost;
    }

    public int GetValue()
    {
        return this.value;
    }

    public void SetValue(int value)
    {
        this.value = value;
    }

    public void SetGCost(int costToStart)
    {
        this.GCost = costToStart;
    }

    public void SetHCost(int costToEnd)
    {
        this.HCost = costToEnd;
    }

    public void SetFCost(int total)
    {
        this.FCost = total;
    }

    public void SetEuclidianCost(double EuclidianCost)
    {
        this.EuclidianCost = EuclidianCost;
    }

    public void SetPosition(int[] position)
    {
        this.position = position;
    }

    public Node GetNode()
    {
        return new Node(position[0], position[1]);
    }
}