using System;
using Data;
using UnityEngine;

namespace Units.Views
{
    public class UnitVisuals : MonoBehaviour
    {
        [SerializeField] private MeshRenderer mr;

        private Texture2D _tex;
        private bool _isColorChanged;

        private void Update()
        {
            if (_isColorChanged)
            {
                _tex.Apply();
                _isColorChanged = false;
            }
        }

        public void SetColor(UnitColorType type, Color color)
        {
            if (_tex == null)
            {
                _tex = new Texture2D(4, 4);
                mr.material.mainTexture = _tex;
            }

            switch (type)
            {
                case UnitColorType.Skin:
                    _tex.SetPixel(0,3,color);
                    break;
                case UnitColorType.Eyes:
                    _tex.SetPixel(1,3,color);
                    break;
                case UnitColorType.Hair:
                    _tex.SetPixel(2,3,color);
                    break;
                case UnitColorType.Mustache:
                    _tex.SetPixel(3,3,color);
                    break;
                case UnitColorType.Beard:
                    _tex.SetPixel(0,2,color);
                    break;
                case UnitColorType.Underwear:
                    _tex.SetPixel(1,2,color);
                    break;
                case UnitColorType.Shirt:
                    _tex.SetPixel(2,2,color);
                    break;
                case UnitColorType.Turtleneck:
                    _tex.SetPixel(3,2,color);
                    break;
                case UnitColorType.Pants:
                    _tex.SetPixel(0,1,color);
                    break;
                case UnitColorType.Shoes:
                    _tex.SetPixel(1,1,color);
                    break;
                case UnitColorType.Boots:
                    _tex.SetPixel(2,1,color);
                    break;
                case UnitColorType.Gloves:
                    _tex.SetPixel(3,1,color);
                    break;
                case UnitColorType.Fingers:
                    _tex.SetPixel(0,0,color);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            _isColorChanged = true;
        }
    }
}
