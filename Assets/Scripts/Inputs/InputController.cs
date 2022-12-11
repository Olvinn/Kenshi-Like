using System;
using UnityEngine;

namespace Inputs
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance { get; private set; }
        
        public event Action<Ray> OnRMB, OnLMB, OnShiftRMB, OnShiftLMB;

        [SerializeField] private Camera camera;

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
                    OnShiftRMB?.Invoke(camera.ScreenPointToRay(Input.mousePosition));
                if (Input.GetButtonUp("Fire1"))
                    OnShiftLMB?.Invoke(camera.ScreenPointToRay(Input.mousePosition));
            }
            else
            {
                if (Input.GetButtonUp("Fire1"))
                    OnLMB?.Invoke(camera.ScreenPointToRay(Input.mousePosition));
                if (Input.GetButtonUp("Fire2"))
                    OnRMB?.Invoke(camera.ScreenPointToRay(Input.mousePosition));
            }
        }
    }
}
