using Inputs;
using UnityEngine;

namespace Cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private Camera camera;
        [SerializeField] private LayerMask raycastMask;

        private Vector3 _pos;
        private Vector3 _center, _camPos;
        private float _scroll, _height;
        private Vector3 _rotation;
        
        private void Awake()
        {
            if (camera == null)
                camera = GetComponent<Camera>();
        }

        private void Start()
        {
            InputController.Instance.OnDragCamera += MoveCamera;
            InputController.Instance.OnScroll += ScrollCamera;
            InputController.Instance.OnMMBDrag += Rotate;
        }

        private void LateUpdate()
        {
            RaycastHit hit;
            Ray ray = new Ray(_pos + Vector3.up * 500, Vector3.down);
            if (Physics.Raycast(ray, out hit, 600, raycastMask))
                _center = hit.point;
            else
                _center = _pos;
            
            _camPos = _center + Vector3.up * 2f;
            _camPos += new Vector3(0, _height, -_scroll * 10);
            
            
            ray = new Ray(hit.point + Vector3.up, Quaternion.Euler(_rotation) * (-hit.point - Vector3.up + _camPos));
            if (Physics.Raycast(ray, out hit, _scroll, raycastMask))
                _camPos = ray.origin + ray.direction * (Vector3.Distance(ray.origin, hit.point) - 1f);
            else
                _camPos = ray.origin + ray.direction * _scroll;
            
            camera.transform.position = _camPos;      
            camera.transform.LookAt(_center);
        }

        public void Warp(Vector3 pos)
        {
            _pos = pos;
        }

        public void SetScroll(float scroll)
        {
            _scroll = scroll;
            _scroll = Mathf.Clamp(_scroll, 2, 100);
        }

        private void MoveCamera(Vector2 mov)
        {
            mov.Normalize();
            mov *= Time.deltaTime * speed * _scroll * .1f;
            _pos += (Vector3)(camera.transform.localToWorldMatrix * new Vector3(mov.x, 0, mov.y));
        }

        private void ScrollCamera(float delta)
        {
            _scroll -= delta * _scroll * .1f;
            _scroll = Mathf.Clamp(_scroll, 2, 50);
            _height = _scroll * _scroll * .2f;
        }
        
        private void Rotate(Vector2 delta)
        {
            delta = new Vector2(delta.y, -delta.x);
            _rotation += (Vector3)delta * (Time.deltaTime * 10);
            _rotation.x = Mathf.Clamp(_rotation.x, -50, 50);
        }
    }
}
