using Units.Structures;
using UnityEngine;

namespace Units.Appearance
{
    public class UnitAppearanceController : MonoBehaviour
    {
        [SerializeField] private Renderer[] _renderers;
        protected MaterialPropertyBlock[] _propertyBlocks;
        
        private void Awake()
        {
            if (_renderers == null)
                _renderers = GetComponentsInChildren<Renderer>();

            _propertyBlocks = new MaterialPropertyBlock[_renderers.Length];
            
            for (int i = 0; i < _renderers.Length; i++)
            {
                _propertyBlocks[i] = new MaterialPropertyBlock();
                _renderers[i].GetPropertyBlock(_propertyBlocks[i]);
            }
        }

        public void SetAppearance(UnitAppearance appearance)
        {
            for (int i = 0; i < _propertyBlocks.Length; i++)
            {
                _propertyBlocks[i].SetColor("_BaseColor", appearance.skinColor);
                _renderers[i].SetPropertyBlock(_propertyBlocks[i]);
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
