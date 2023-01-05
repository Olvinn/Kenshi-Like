using System;
using Data;
using UnityEngine;

namespace Units.Views
{
    public class UnitAppearance : MonoBehaviour
    {
        public event Action OnVisible, OnInvisible;
        
        [SerializeField] private Renderer mr;

        private Texture2D _mainTex, _metallicTex;
        private bool _isColorChanged;

        private void Update()
        {
            if (_isColorChanged)
            {
                _mainTex.Apply();
                _isColorChanged = false;
                _metallicTex.Apply();
                mr.material.SetTexture("_MetallicGlossMap", _metallicTex);
            }
        }

        private void OnBecameInvisible()
        {
            OnInvisible?.Invoke();
        }

        private void OnBecameVisible()
        {
            OnVisible?.Invoke();
        }

        public void SetColor(UnitColorType type, Color color, Color metallic)
        {
            if (_mainTex == null)
            {
                _mainTex = new Texture2D(4, 4);
                mr.material.mainTexture = _mainTex;
            }

            if (_metallicTex == null)
            {
                _metallicTex = new Texture2D(4, 4);
            }

            switch (type)
            {
                case UnitColorType.Skin:
                    _mainTex.SetPixel(0,3,color);
                    _metallicTex.SetPixel(0,3, metallic);
                    break;
                case UnitColorType.Eyes:
                    _mainTex.SetPixel(1,3,color);
                    _metallicTex.SetPixel(1,3, metallic);
                    break;
                case UnitColorType.Hair:
                    _mainTex.SetPixel(2,3,color);
                    _metallicTex.SetPixel(2,3, metallic);
                    break;
                case UnitColorType.Mustache:
                    _mainTex.SetPixel(3,3,color);
                    _metallicTex.SetPixel(3,3, metallic);
                    break;
                case UnitColorType.Beard:
                    _mainTex.SetPixel(0,2,color);
                    _metallicTex.SetPixel(0,2, metallic);
                    break;
                case UnitColorType.Underwear:
                    _mainTex.SetPixel(1,2,color);
                    _metallicTex.SetPixel(1,2, metallic);
                    break;
                case UnitColorType.Shirt:
                    _mainTex.SetPixel(2,2,color);
                    _metallicTex.SetPixel(2,2, metallic);
                    break;
                case UnitColorType.Turtleneck:
                    _mainTex.SetPixel(3,2,color);
                    _metallicTex.SetPixel(3,2, metallic);
                    break;
                case UnitColorType.Pants:
                    _mainTex.SetPixel(0,1,color);
                    _metallicTex.SetPixel(0,1, metallic);
                    break;
                case UnitColorType.Shoes:
                    _mainTex.SetPixel(1,1,color);
                    _metallicTex.SetPixel(1,1, metallic);
                    break;
                case UnitColorType.Boots:
                    _mainTex.SetPixel(2,1,color);
                    _metallicTex.SetPixel(2,1, metallic);
                    break;
                case UnitColorType.Gloves:
                    _mainTex.SetPixel(3,1,color);
                    _metallicTex.SetPixel(3,1, metallic);
                    break;
                case UnitColorType.Fingers:
                    _mainTex.SetPixel(0,0,color);
                    _metallicTex.SetPixel(0,0, metallic);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            _isColorChanged = true;
        }
    }
}
