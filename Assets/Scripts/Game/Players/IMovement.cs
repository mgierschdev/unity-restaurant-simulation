using UnityEngine;
using System.Collections.Generic;

interface IMovement{
    //Adds a path to the player/Npc 
    public void AddPath(List<Node> path);
    //Gets the shortest path to the objective
    public List<Node> GetPath(int[] from, int[] to);
}