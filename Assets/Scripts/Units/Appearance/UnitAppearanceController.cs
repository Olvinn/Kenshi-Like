using System;
using Units.Structures;
using UnityEngine;

namespace Units.Appearance
{
    public class UnitAppearanceController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;
        private int _frames, _savedSec;

        private void Awake()
        {
            if (_renderer == null)
                _renderer = GetComponent<MeshRenderer>();
        }

        public void SetAppearance(UnitAppearance appearance)
        {
            _renderer.material.color = appearance.baseColor;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_renderer == null)
                _renderer = GetComponent<MeshRenderer>();
        }
#endif
    }
}
