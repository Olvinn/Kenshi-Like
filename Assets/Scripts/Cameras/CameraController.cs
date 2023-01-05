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
        private float _scroll;
        private Vector3 _rotation;
        
        private void Awake()
        {
            if (camera == null)
                camera = GetComponent<Camera>();
        }

        private void Start()
        {
            InputController.Instance.OnTouchScreenCorners += MoveCamera;
            InputController.Instance.OnMove += MoveCamera;
            InputController.Instance.OnScroll += ScrollCamera;
            InputController.Instance.OnMMBDrag += Rotate;
        }

        //TODO: Add math to this madness:
        //1) calculate camera height and distance.
        //2) correct it with rays.
        //3) calculate diff between actual and expect pos and apply it somehow
        private void LateUpdate()
        {
            RaycastHit hit;
            Ray ray = new Ray(_pos + Vector3.up * 1000, Vector3.down);
            if (Physics.Raycast(ray, out hit, 1100, raycastMask))
                _center = hit.point + Vector3.up;
            else
                _center = _pos;
            
            _camPos = _center + new Vector3(0, 0, -_scroll);

            ray = new Ray(_center, Quaternion.Euler(_rotation) * (-hit.point + _camPos));
            if (Physics.Raycast(ray, out hit, _scroll, raycastMask))
            {
                float temp = (Vector3.Distance(ray.origin, hit.point) * .9f);
                _camPos = ray.origin + ray.direction * temp + Vector3.up * Mathf.Clamp01(temp);
            }
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
            _scroll = Mathf.Clamp(_scroll, 2, 80);
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
        }
        
        private void Rotate(Vector2 delta)
        {
            delta = new Vector2(delta.y, -delta.x);
            _rotation += (Vector3)delta * (Time.deltaTime * 10);
            _rotation.x = Mathf.Clamp(_rotation.x, -30, 45);
        }
    }
}
