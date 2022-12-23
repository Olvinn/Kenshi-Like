using System;
using UnityEngine;

namespace Inputs
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance { get; private set; }
        
        public event Action<Ray> OnRMB, OnLMB, OnShiftRMB, OnShiftLMB;
        public event Action<Vector2> OnDragCamera;
        public event Action<Vector2, Vector2> OnDrawBox, OnBoxSelect;

        [SerializeField] private Camera playCamera;

        private Vector2 _start;
        private bool _isDrawingBox;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetButtonUp("Fire2"))
                    OnShiftRMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
                if (Input.GetButtonUp("Fire1"))
                    OnShiftLMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    _start = Input.mousePosition;
                    _isDrawingBox = true;
                }
                
                if (Input.GetButton("Fire1"))
                    if (Vector3.Distance(_start, Input.mousePosition) >= 3)
                    {
                        var end = Input.mousePosition;
                        Vector2 pos = new Vector2(end.x < _start.x ? end.x : _start.x, end.y < _start.y ? end.y : _start.y);
                        Vector2 size = new Vector2(Mathf.Abs(end.x - _start.x), Mathf.Abs(end.y - _start.y));
                        OnDrawBox?.Invoke(pos, size);
                    }

                if (Input.GetButtonUp("Fire1"))
                {
                    if (Vector3.Distance(_start, Input.mousePosition) < 3)
                        OnLMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
                    else
                    {
                        var end = Input.mousePosition;
                        Vector2 pos = new Vector2(end.x < _start.x ? end.x : _start.x, end.y < _start.y ? end.y : _start.y);
                        Vector2 size = new Vector2(Mathf.Abs(end.x - _start.x), Mathf.Abs(end.y - _start.y));
                        OnBoxSelect?.Invoke(pos, size);
                    }
                    _isDrawingBox = false;
                }

                if (Input.GetButtonUp("Fire2"))
                    OnRMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
            }

            if (Input.mousePosition.x <= 1 || Input.mousePosition.x >= Screen.width - 1
                                           || Input.mousePosition.y <= 1 || Input.mousePosition.y >= Screen.height - 1)
            {
                OnDragCamera?.Invoke( (Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) * .5f);
            }
        }
    }
}
