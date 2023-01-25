using UnityEngine;

public interface IMoving
{
    void Move(Vector3 pos);
    void Follow(Transform target);
}
