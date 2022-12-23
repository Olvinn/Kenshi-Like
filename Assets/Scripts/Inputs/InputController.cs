using System;
using UnityEngine;

namespace Inputs
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance { get; private set; }
        
        public event Action<Ray> OnRMB, OnLMB, OnShiftRMB, OnShiftLMB;
        public event Action<Vector2> OnDragCamera;

        [SerializeField] private Camera playCamera;

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
                if (Input.GetButtonUp("Fire1"))
                    OnLMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
                if (Input.GetButtonUp("Fire2"))
                    OnRMB?.Invoke(playCamera.ScreenPointToRay(Input.mousePosition));
            }
            
            if (Input.mousePosition.x <= 1 || Input.mousePosition.x >= Screen.width - 1
                || Input.mousePosition.y <= 1 || Input.mousePosition.y >= Screen.height - 1)
                OnDragCamera?.Invoke( (Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) * .5f);
        }
    }
}
