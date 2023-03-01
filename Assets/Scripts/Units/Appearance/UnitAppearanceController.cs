using Units.Structures;
using UnityEngine;

namespace Units.Appearance
{
    public class UnitAppearanceController : MonoBehaviour
    {
        [SerializeField] private Renderer[] _renderers;
        
        private void Awake()
        {
            if (_renderers == null)
                _renderers = GetComponentsInChildren<Renderer>();
        }

        public void SetAppearance(UnitAppearance appearance)
        {
            foreach (var renderer in _renderers)
            {
                renderer.material.color = appearance.baseColor;
            }
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_renderers == null)
                _renderers = GetComponentsInChildren<Renderer>();
        }
#endif
    }
}
