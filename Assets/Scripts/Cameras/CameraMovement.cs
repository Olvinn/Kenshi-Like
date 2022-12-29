using System;
using Inputs;
using UnityEngine;

namespace Cameras
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private Camera camera;
        [SerializeField] private LayerMask mask;

        private Vector3 _pos;
        private Vector3 _center, _camPos;
        private float _scroll, _height;
        
        private void Awake()
        {
            if (camera == null)
                camera = GetComponent<Camera>();
        }

        private void Start()
        {
            InputController.Instance.OnDragCamera += MoveCamera;
            InputController.Instance.OnScroll += ScrollCamera;
        }

        private void LateUpdate()
        {
            RaycastHit hit;
            Ray ray = new Ray(_pos + Vector3.up * 500, Vector3.down);
            if (Physics.Raycast(ray, out hit, 600, mask))
                _center = hit.point;
            else
                _center = _pos;
            
            _camPos = _center + Vector3.up * 2f;
            _camPos += new Vector3(0, _height, -_scroll * 10);
            
            
            ray = new Ray(hit.point + Vector3.up, -hit.point - Vector3.up + _camPos);
            if (Physics.Raycast(ray, out hit, _scroll, mask))
                _camPos = hit.point;
            else
                _camPos = ray.origin + ray.direction * _scroll;
            
            camera.transform.position = _camPos;      
            camera.transform.LookAt(_center);
        }

        private void MoveCamera(Vector2 mov)
        {
            mov.Normalize();
            mov *= Time.deltaTime * speed * _scroll * .1f;
            _pos += new Vector3(mov.x, 0, mov.y);
        }

        private void ScrollCamera(float delta)
        {
            _scroll -= delta * _scroll * .1f;
            _scroll = Mathf.Clamp(_scroll, 2, 100);
            _height = _scroll * _scroll * .2f;
        }
    }
}
