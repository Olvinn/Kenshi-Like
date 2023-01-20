using System;
using Connections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inputs
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance { get; private set; }

        public Action OnEscape;
        public Action<Ray> OnRMB, OnLMB;
        public Action<Vector2> OnTouchScreenCorners, OnMove;
        public Action<Vector2, Vector2> OnDrawBox, OnBoxSelect;
        public Action<float> OnScroll;
        public Action<Vector2> OnMMBDrag;
        public bool isAdditiveModifierApplied;

        [SerializeField] private Camera playCamera;

        private Vector2 _startBox;
        private bool _isDrawingBox, _isDrawingStarted;
        private Vector3 _savedMMB;
        private bool _isOverUI;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        private void Update()
        {
            _isOverUI = EventSystem.current.IsPointerOverGameObject();
            KeyboardMovementInput();
            KeyboardActionsInput();
            MouseClickInput();
            CameraMovementInput();
            BoxSelectionInput();
        }

        void KeyboardActionsInput()
        {
            isAdditiveModifierApplied = Input.GetKey(KeyCode.LeftShift);
            if (Input.GetKeyDown(KeyCode.Escape))
                OnEscape?.Invoke();
        }

        void MouseClickInput()
        {
            if (_isDrawingBox || _isOverUI)
                return;

            if (Input.GetButtonUp("Fire1") && !_isDrawingBox)
            {
                OnLMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
                _isDrawingStarted = false;
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                OnRMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
                _isDrawingStarted = false;
            }
        }
        
        void BoxSelectionInput()
        {
            _isDrawingBox = _isDrawingStarted && Vector3.Distance(_startBox, Input.mousePosition) >= 3;
            
            if (Input.GetButtonDown("Fire1") && !_isOverUI)
            {
                _startBox = Input.mousePosition;
                _isDrawingStarted = true;
                return;
            }
            
            var end = Input.mousePosition;
            Vector2 pos = new Vector2(end.x < _startBox.x ? end.x : _startBox.x, end.y < _startBox.y ? end.y : _startBox.y);
            Vector2 size = new Vector2(Mathf.Abs(end.x - _startBox.x), Mathf.Abs(end.y - _startBox.y));
            
            if (Input.GetButtonUp("Fire1") && _isDrawingBox && _isDrawingStarted)
            {
                OnBoxSelect?.Invoke(pos, size);
                _isDrawingStarted = false;
                return;
            }
            
            if (Input.GetButton("Fire1") && _isDrawingBox && _isDrawingStarted)
            {
                OnDrawBox?.Invoke(pos, size);
            }
        }

        void CameraMovementInput()
        {
            if (_isOverUI)
                return;
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
