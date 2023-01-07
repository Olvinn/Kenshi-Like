using System;
using UnityEngine;

namespace Inputs
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance { get; private set; }
        
        public event Action<Ray> OnRMB, OnLMB, OnShiftRMB, OnShiftLMB;
        public event Action<Vector2> OnTouchScreenCorners, OnMove;
        public event Action<Vector2, Vector2> OnDrawBox, OnBoxSelect, OnShiftBoxSelect;
        public event Action<float> OnScroll;
        public event Action<Vector2> OnMMBDrag;

        [SerializeField] private Camera playCamera;

        private Vector2 _startBox;
        private bool _isDrawingBox;
        private Vector3 _savedMMB;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        private void Update()
        {
            MouseClickInput();
            BoxSelectionInput();
            CameraMovementInput();
            KeyboardMovementInput();
        }

        void MouseClickInput()
        {
            if (_isDrawingBox)
                return;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetButtonUp("Fire1"))
                    OnShiftLMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
                else if (Input.GetButtonUp("Fire2"))
                    OnShiftRMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
            }
            else
            {
                if (Input.GetButtonUp("Fire1") && !_isDrawingBox)
                    OnLMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
                else if (Input.GetButtonUp("Fire2"))
                    OnRMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
            }
        }
        
        void BoxSelectionInput()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                _startBox = Input.mousePosition;
            }
                
            if (Input.GetButton("Fire1"))
                if (Vector3.Distance(_startBox, Input.mousePosition) >= 3)
                {
                    var end = Input.mousePosition;
                    Vector2 pos = new Vector2(end.x < _startBox.x ? end.x : _startBox.x, end.y < _startBox.y ? end.y : _startBox.y);
                    Vector2 size = new Vector2(Mathf.Abs(end.x - _startBox.x), Mathf.Abs(end.y - _startBox.y));
                    OnDrawBox?.Invoke(pos, size);
                    _isDrawingBox = true;
                }

            if (Input.GetButtonUp("Fire1") && _isDrawingBox)
            {
                var end = Input.mousePosition;
                Vector2 pos = new Vector2(end.x < _startBox.x ? end.x : _startBox.x, end.y < _startBox.y ? end.y : _startBox.y);
                Vector2 size = new Vector2(Mathf.Abs(end.x - _startBox.x), Mathf.Abs(end.y - _startBox.y));
                if (Input.GetKey(KeyCode.LeftShift))
                    OnShiftBoxSelect?.Invoke(pos, size);
                else
                    OnBoxSelect?.Invoke(pos, size);
                _isDrawingBox = false;
            }
        }

        void CameraMovementInput()
        {
            if (Input.GetButtonDown("Fire3"))
            {
                _savedMMB = Input.mousePosition;
            }
            else if (Input.GetButton("Fire3"))
            {
                OnMMBDrag?.Invoke(_savedMMB - Input.mousePosition);
                _savedMMB = Input.mousePosition;
            }
            else if (Input.mousePosition.x <= 1 || Input.mousePosition.x >= Screen.width - 1
                                                || Input.mousePosition.y <= 1 || Input.mousePosition.y >= Screen.height - 1)
            {
                OnTouchScreenCorners?.Invoke( (Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) * .5f);
            }
            
            if (Input.mouseScrollDelta != Vector2.zero)
                OnScroll?.Invoke(Input.mouseScrollDelta.y);
        }

        void KeyboardMovementInput()
        {
            var move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (move != Vector2.zero)
                OnMove?.Invoke(move);
        }
    }
}
