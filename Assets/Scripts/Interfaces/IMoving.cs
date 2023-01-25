using UnityEngine;

namespace Interfaces
{
    public interface IMoving
    {
        void Move(Vector3 pos);
        void Follow(Transform target);
    }
}
