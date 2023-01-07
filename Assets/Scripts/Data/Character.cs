using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Character")]
    public class Character : ScriptableObject
    {
        [Header("Attributes")]
        [SerializeField] public Parameters parameters;

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

        public float GetParameter(ParametersType type)
        {
            return parameters.GetParameter(type);
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

    public enum ParametersType
    {
        HealthPoints = 0,
        Speed = 100,
        Damage = 200,
        AttackRate = 300,
        AttackDelay = 301,
        DodgeChance = 400,
        BlockChance = 501
    }

    [Serializable]
    public struct Parameters
    {
        [SerializeField, Range(100f, 1000f)] private float hp;
        [SerializeField, Range(1f, 10f)] private float speed;
        [SerializeField, Range(1f, 200f)] private float damage;
        [SerializeField, Range(.1f, 1.5f)] private float attackRate;
        [SerializeField, Range(.5f, 5f)] private float attackDelay;
        [SerializeField, Range(0, 1f)] private float chanceToDodge;
        [SerializeField, Range(0, 1f)] private float chanceToBlock;

        public float GetParameter(ParametersType type)
        {
            switch (type)
            {
                case ParametersType.HealthPoints:
                    return hp;
                case ParametersType.Speed:
                    return speed;
                case ParametersType.Damage:
                    return damage;
                case ParametersType.AttackRate:
                    return attackRate;
                case ParametersType.AttackDelay:
                    return attackDelay;
                case ParametersType.DodgeChance:
                    return chanceToDodge;
                case ParametersType.BlockChance:
                    return chanceToBlock;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
