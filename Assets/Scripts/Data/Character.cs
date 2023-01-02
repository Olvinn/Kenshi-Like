using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Character")]
    public class Character : ScriptableObject
    {
        [Header("Attributes")]
        [SerializeField] public Attributes attributes;

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

        public float GetAttribute(AttributeType type)
        {
            return attributes.GetAttribute(type);
        }

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

    public enum AttributeType
    {
        HealthPoints = 0,
        Speed = 100,
        Damage = 200
    }

    [Serializable]
    public struct Attributes
    {
        [SerializeField] private  float hp;
        [SerializeField] private  float speed;
        [SerializeField] private  float damage;

        public float GetAttribute(AttributeType type)
        {
            switch (type)
            {
                case AttributeType.HealthPoints:
                    return hp;
                case AttributeType.Speed:
                    return speed;
                case AttributeType.Damage:
                    return damage;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
