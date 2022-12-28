using System;
using UnityEditor;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Character")]
    public class Character : ScriptableObject
    {
        [Header("Attributes")]
        public float hp;
        public float speed;
        public float damage;

        [Header("Appearance")] 
        [SerializeField] private Color skinColor;
        [SerializeField] private Color eyesColor;
        [SerializeField] private Color hairColor;
        [SerializeField] private Color mustacheColor;
        [SerializeField] private Color beardColor;
        [SerializeField] private Color underwearColor;
        [SerializeField] private Color shirtColor;
        [SerializeField] private Color turtleneckColor;
        [SerializeField] private Color pantsColor;
        [SerializeField] private Color shoesColor;
        [SerializeField] private Color bootsColor;
        [SerializeField] private Color glovesColor;
        [SerializeField] private Color fingersColor;

        public Color GetColor(UnitColorType type)
        {
            switch (type)
            {
                case UnitColorType.Skin:
                    return skinColor;
                case UnitColorType.Eyes:
                    return eyesColor;
                case UnitColorType.Hair:
                    return hairColor;
                case UnitColorType.Mustache:
                    return mustacheColor;
                case UnitColorType.Beard:
                    return beardColor;
                case UnitColorType.Underwear:
                    return underwearColor;
                case UnitColorType.Shirt:
                    return shirtColor;
                case UnitColorType.Turtleneck:
                    return turtleneckColor;
                case UnitColorType.Pants:
                    return pantsColor;
                case UnitColorType.Shoes:
                    return shoesColor;
                case UnitColorType.Boots:
                    return bootsColor;
                case UnitColorType.Gloves:
                    return glovesColor;
                case UnitColorType.Fingers:
                    return fingersColor;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    public enum UnitColorType
    {
        Skin,
        Eyes,
        Hair,
        Mustache,
        Beard,
        Underwear,
        Shirt,
        Turtleneck,
        Pants,
        Shoes,
        Boots,
        Gloves,
        Fingers
    }
}
