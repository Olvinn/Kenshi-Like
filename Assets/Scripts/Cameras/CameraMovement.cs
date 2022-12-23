using System;
using Inputs;
using UnityEngine;

namespace Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private Camera camera;

        private Vector2 _mov;
        
        private void Awake()
        {
            if (camera == null)
                camera = GetComponent<Camera>();
        }

        private void Start()
        {
            InputController.Instance.OnDragCamera += MoveCamera;
        }

        private void LateUpdate()
        {
            transform.position += new Vector3(_mov.x, 0, _mov.y);
            _mov = Vector2.zero;
        }

        private void MoveCamera(Vector2 mov)
        {
            mov.Normalize();
            mov *= Time.deltaTime * speed;
            _mov = mov;
        }
    }
}
