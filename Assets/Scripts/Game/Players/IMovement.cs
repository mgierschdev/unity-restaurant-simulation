using UnityEngine;
using System.Collections.Generic;

interface IMovement{
    //Adds a path to the player/Npc 
    public void AddPath(List<Node> path);
    //Gets the shortest path to the objective
    public List<Node> GetPath(int[] from, int[] to);
    //Gets a Vector3 depending on the cardinal direction LEFT/RIGHT/DOWN
    public Vector3 GetVectorFromDirection(MoveDirection d);
}