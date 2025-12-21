using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using Util;

// Parent of the GameGridObject
namespace Game.Grid
{
    /**
     * Problem: Provide shared properties for grid-related objects.
     * Goal: Standardize naming, positions, and tile metadata.
     * Approach: Define an abstract base class with common fields.
     * Time: O(1) per access.
     * Space: O(1).
     */
    public abstract class GameObjectBase
    {
        public string Name { get; set; }

        public SortingGroup SortingLayer { get; set; }

        //In Pathfinding grid
        public Vector3Int GridPosition { get; set; }

        // Local grid position, can be negatice -20,20
        public Vector3Int LocalGridPosition { get; set; }
        public Vector3 WorldPosition { get; set; }
        public ObjectType Type { get; set; }
        public TileType TileType { get; set; }
        private readonly Vector3 _tileOffset = new Vector3(0, 0.25f, 0);
        public TileBase UnityTileBase { get; set; }
        public double Cost { get; set; }

        public Vector3 GetWorldPositionWithOffset()
        {
            return WorldPosition + _tileOffset;
        }
    }
}
