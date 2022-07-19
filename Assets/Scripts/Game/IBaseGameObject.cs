using UnityEngine;
using System.Collections.Generic;

public interface IGameObject
{
    float GetY();
    float GetX();
    ObjectType GetType();
    Vector3 GetPosition();
    void SetTestGameGridController(GameGridController controller);
    void SetSpeed(float speed);
    float[] GetPositionAsArray();
    void AddMovement(Vector3 direction);
    void AddPath(List<Node> n);
}