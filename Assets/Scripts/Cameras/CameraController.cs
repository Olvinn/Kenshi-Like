using System;
using Data;
using Inputs;
using UnityEngine;

namespace Cameras
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera bounds")]
        [SerializeField] private Vector3 center;
        [SerializeField] private Vector3 size;
        
        [SerializeField] private float speed = 10f;
        [SerializeField] private Camera camera;
        [SerializeField] private LayerMask raycastMask;

        [SerializeField] private Vector3 _pos;
        private Vector3 _center, _camPos;
        private float _scroll;
        private Vector3 _rotation;
        
        private void Awake()
        {
            if (camera == null)
                camera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            InputController.Instance.OnTouchScreenCorners += MoveCamera;
            InputController.Instance.OnMove += MoveCamera;
            InputController.Instance.OnScroll += ScrollCamera;
            InputController.Instance.OnMMBDrag += Rotate;
        }

        private void OnDisable()
        {
            InputController.Instance.OnTouchScreenCorners -= MoveCamera;
            InputController.Instance.OnMove -= MoveCamera;
            InputController.Instance.OnScroll -= ScrollCamera;
            InputController.Instance.OnMMBDrag -= Rotate;
        }

        private void LateUpdate()
        {
            var constants = GameContext.Instance.Constants;
            
            _pos.y = 0;
            
            RaycastHit hit;
            Ray ray = new Ray(_pos + Vector3.up * constants.RayCastHeight, Vector3.down);
            if (Physics.Raycast(ray, out hit, constants.RayLength, raycastMask))
                _center = hit.point + Vector3.up * constants.CameraCenterHeight;
            else
                _center = _pos;
            
            _camPos = _center + Quaternion.Euler(_rotation) * new Vector3(0, 0, -_scroll);
            
            ray = new Ray(_center, _camPos - _center);
            if (Physics.SphereCast(ray, camera.nearClipPlane + .1f, out hit, _scroll, raycastMask))
            {
                float dist = Vector3.Distance(_center, hit.point) * .9f;
                _camPos = _center + ray.direction * dist;
            }

            camera.transform.position = _camPos;      
            camera.transform.LookAt(_center);
        }

        public void Warp(Vector3 pos)
        {
            _pos = pos;
            _pos.y = 0;
            ClampPos();
        }

        public void SetScroll(float scroll)
        {
            _scroll = scroll;
            _scroll = Mathf.Clamp(_scroll, 2, 80);
        }

        private void ClampPos()
        {
            _pos.x = Mathf.Clamp(_pos.x, center.x - size.x + .1f, center.x + size.x - .1f);
            _pos.z = Mathf.Clamp(_pos.z, center.z - size.z + .1f, center.z + size.z - .1f);
        }

        private void MoveCamera(Vector2 mov)
        {
            mov.Normalize();
            mov *= Time.unscaledDeltaTime * speed * _scroll * .1f;
            _pos += (Vector3)(camera.transform.localToWorldMatrix * new Vector3(mov.x, 0, mov.y));
            ClampPos();
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
