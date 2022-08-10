using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

// This controlls the isometric tiles on the grid
public class IsometricGridController : MonoBehaviour
{
    //Tilemap
    private Tilemap tilemap;
    private List<GameTile> list;
    private Dictionary<Vector3, GameTile> map;

    void Awake()
    {
        tilemap = GameObject.Find(Settings.TILEMAP_FLOOR_0).GetComponent<Tilemap>();
        map = new Dictionary<Vector3, GameTile>();
        list = new List<GameTile>();

        if (tilemap == null)
        {
            Debug.LogWarning("IsometricGridController/tilemap null");
        }

        foreach (Vector3 pos in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPlace = Vector3Int.FloorToInt(pos);
            Vector3 placeInWorld = tilemap.CellToWorld(localPlace);

            if (tilemap.HasTile(localPlace))
            {
                TileBase tile = tilemap.GetTile(localPlace);
                GameTile gameTile = new GameTile(placeInWorld, Util.GetTileType(tile.name));
                list.Add(gameTile);
                map.TryAdd(gameTile.GridPosition, gameTile);
                Debug.Log("Tile " + gameTile.GridPosition + " " + gameTile.WorldPosition + " " + gameTile.Type+" ("+tile.name+")");
            }
        }
    }
}
