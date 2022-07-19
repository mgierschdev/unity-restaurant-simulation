using UnityEngine;
using System.Collections.Generic;

public interface IGameObject
{
    int GetY();
    int GetX();
    ObjectType GetType();
    Vector3 GetPosition();
    void SetTestGameGridController(GameGridController controller);
    void SetSpeed(float speed);
    float[] GetPositionAsArray();
    void AddMovement(MoveDirection direction);
    void AddPath(List<Node> n);
}