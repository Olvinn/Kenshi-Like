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
        
        private void Awake()
        {
            if (camera == null)
                camera = GetComponent<Camera>();
        }

        private void Start()
        {
            InputController.Instance.OnDragCamera += MoveCamera;
        }

        private void MoveCamera(Vector2 mov)
        {
            mov.Normalize();
            mov *= Time.deltaTime * speed;
            transform.position += new Vector3(-mov.x, 0, -mov.y);
        }
    }
}
