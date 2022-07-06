using UnityEngine;

// This could be any item
public class GameItem : MonoBehaviour
{
    private Vector2Int inGamePosition;

    void Awake() {
        inGamePosition = Util.GetXYInGameMap(transform.position);
    }

    public Vector2Int GetInGamePosition(){
        return inGamePosition;
    }
}