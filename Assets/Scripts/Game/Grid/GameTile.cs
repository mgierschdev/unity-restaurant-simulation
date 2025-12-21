using UnityEngine;
using UnityEngine.Tilemaps;
using Util;

namespace Game.Grid
{
    /**
     * Problem: Represent a tile in the game grid with positions and type.
     * Goal: Encapsulate tile metadata used by grid logic.
     * Approach: Store grid/world coordinates and Unity tile reference.
     * Time: O(1) for accessors.
     * Space: O(1).
     */
    public class GameTile : GameObjectBase
    {
        public GameTile(Vector3 worldPosition, Vector3Int gridPosition, Vector3Int localGridPosition, TileType name,
            ObjectType type, TileBase unityTileBase)
        {
            // Grid position, first position = 0, 0
            GridPosition = gridPosition;
            // World position on Unity coords
            WorldPosition = worldPosition;
            LocalGridPosition = localGridPosition;
            TileType = name;
            Type = type;
            UnityTileBase = unityTileBase;
        }

        public override string ToString()
        {
            return WorldPosition + " " + GridPosition + " " + LocalGridPosition + " " + Name + " " + Type + " ";
        }
    }
}
