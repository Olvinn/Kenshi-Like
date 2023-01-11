using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    [CreateAssetMenu(menuName = "Game/Character")]
    public class Character : ScriptableObject
    {
        [Header("Attributes")]
        public Parameters parameters;

        [Header("Appearance")] 
        public Appearance appearance;

        public float GetParameter(ParametersType type)
        {
            return parameters.GetParameter(type);
        }

        public Color GetColor(UnitColorType type)
        {
            return appearance.GetColor(type);
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

    [Serializable]
    public struct Appearance
    {
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
        
        public static Appearance GetRandomAppearance()
        {
            Appearance result = new Appearance();

            result.skinColor = ColorsPresets.instance.SkinColors[Random.Range(0, ColorsPresets.instance.SkinColors.Count)];

            switch (Random.Range(0, 3)) //torso
            {
                case 0: //naked torso
                    result.shirtColor = result.skinColor;
                    result.turtleneckColor = result.skinColor;
                    break;
                case 1: //shirt
                    result.shirtColor = ColorsPresets.instance.ClothesColors[Random.Range(0, ColorsPresets.instance.ClothesColors.Count)];
                    result.turtleneckColor = result.turtleneckColor;
                    break;
                case 2: //turtleneck
                    result.shirtColor = ColorsPresets.instance.ClothesColors[Random.Range(0, ColorsPresets.instance.ClothesColors.Count)];
                    result.turtleneckColor = result.shirtColor;
                    break;
            }

            switch (Random.Range(0, 2)) //legs
            {
                case 0: //only underwear
                    result.underwearColor = ColorsPresets.instance.ClothesColors[Random.Range(0, ColorsPresets.instance.ClothesColors.Count)];
                    result.pantsColor = result.skinColor;
                    break;
                case 1: //pants
                    result.underwearColor = ColorsPresets.instance.ClothesColors[Random.Range(0, ColorsPresets.instance.ClothesColors.Count)];
                    result.pantsColor = result.underwearColor;
                    break;
            }

            switch (Random.Range(0, 3)) //feet
            {
                case 0: //no shoes
                    result.bootsColor = result.skinColor;
                    result.shoesColor = result.skinColor;
                    break;
                case 1: //shoes 
                    result.bootsColor = result.pantsColor;
                    result.shoesColor = ColorsPresets.instance.BootsColors[Random.Range(0, ColorsPresets.instance.BootsColors.Count)];
                    break;
                case 3: //boots
                    result.bootsColor = ColorsPresets.instance.BootsColors[Random.Range(0, ColorsPresets.instance.BootsColors.Count)];
                    result.shoesColor = result.bootsColor;
                    break;
            }

            switch (Random.Range(0,3)) //arms
            {
                case 0: //no gloves
                    result.glovesColor = result.skinColor;
                    result.fingersColor = result.skinColor;
                    break;
                case 1: //short gloves 
                    result.glovesColor = ColorsPresets.instance.ClothesColors[Random.Range(0, ColorsPresets.instance.ClothesColors.Count)];
                    result.fingersColor = result.skinColor;
                    break;
                case 3: //gloves
                    result.glovesColor = ColorsPresets.instance.ClothesColors[Random.Range(0, ColorsPresets.instance.ClothesColors.Count)];
                    result.fingersColor = result.glovesColor;
                    break;
            }
            
            if (Random.Range(0,2) == 0)
                result.hairColor = result.skinColor;
            else
                result.hairColor = ColorsPresets.instance.HairColors[Random.Range(0, ColorsPresets.instance.HairColors.Count)];
            
            if (Random.Range(0,2) == 0)
                result.mustacheColor = result.skinColor;
            else
                result.mustacheColor = result.hairColor;
            
            if (Random.Range(0,2) == 0)
                result.beardColor = result.skinColor;
            else
                result.beardColor = result.hairColor;
            
            result.eyesColor = Color.white;
            
            return result;
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
}
