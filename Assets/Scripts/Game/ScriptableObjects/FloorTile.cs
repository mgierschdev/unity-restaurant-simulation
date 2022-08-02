using UnityEngine;

// TEST
// This will make it available in the Unity editor
[CreateAssetMenu (fileName = "TileData", menuName = "Tile Data")]
public class FloorTile : ScriptableObject
{
    public TileType Type = TileType.FLOOR;   
}
