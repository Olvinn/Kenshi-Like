using Units.Structures;
using UnityEngine;

namespace Units.Appearance
{
    public class UnitAppearanceController : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        
        private void Awake()
        {
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();
        }

        public void SetAppearance(UnitAppearance appearance)
        {
            _renderer.material.color = appearance.baseColor;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();
        }
#endif
    }
}
