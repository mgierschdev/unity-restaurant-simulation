using UnityEngine;
using UnityEngine.Rendering;

public abstract class GameObjectBase
{
    protected SortingGroup sortingLayer;
    protected IsometricGridController IsometricGrid;
    public Vector3Int GridPosition { get; set; } //In Pathfinding grid
    public Vector3Int LocalGridPosition { get; set; } // Local grid position, can be negatice -20,20
    public Vector3 WorldPosition { get; set; }
    public ObjectType Type { get; set; }
}