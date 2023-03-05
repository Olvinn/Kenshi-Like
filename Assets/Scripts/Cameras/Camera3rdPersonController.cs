using UnityEngine;

namespace Cameras
{
    public class Camera3rdPersonController : MonoBehaviour
    {
        [SerializeField] private Vector3 targetOffset, cameraOffset;
        [SerializeField] private float horizontalSpeed = 10f, verticalSpeed = 5f, lerpSpeed = 5f, scrollSpeed = 3f;
        [SerializeField] private float minAngle = 5, maxAngle = 60;
        
        private Camera _camera;
        private Transform _target;

        private float _x, _y;
        private Quaternion _targetRotation, _currentRotation;

        private void Awake()
        {
            _targetRotation = Quaternion.identity;
        }

        private void LateUpdate()
        {
            if (!_target || !_camera)
                return;

            _currentRotation = Quaternion.Lerp(_currentRotation, _targetRotation, Time.deltaTime * lerpSpeed);
            Vector3 temp = _currentRotation * cameraOffset;
            _camera.transform.position = _target.transform.position + temp;
            float zoomMultiplier = (Mathf.Abs(cameraOffset.z) - 2) * .3f;
            Vector3 zoomedTargetOffset = targetOffset * zoomMultiplier;
            zoomedTargetOffset.y = targetOffset.y;
            _camera.transform.LookAt(_target.transform.position + (Vector3)(_currentRotation * zoomedTargetOffset));
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetCamera(Camera cam)
        {
            _camera = cam;
        }

        public void Rotate(Vector2 axis)
        {
            _x -= axis.y * horizontalSpeed * Time.deltaTime;
            _y += axis.x * verticalSpeed * Time.deltaTime;
            _x = Mathf.Clamp(_x, minAngle, maxAngle);
            _targetRotation = Quaternion.Euler(_x, _y, 0);
        }

        public void Scroll(float scroll)
        {
            cameraOffset.z += scroll * scrollSpeed;
            cameraOffset.z = Mathf.Clamp(cameraOffset.z, -5, -2);
        }
    }
}
