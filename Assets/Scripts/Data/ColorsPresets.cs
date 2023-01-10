using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Data
{ 
    [CreateAssetMenu(menuName = "Game/Colors preset")]
    public class ColorsPresets : ScriptableSingleton<ColorsPresets>
    {
        [field: Header("Characters")]
        [field: SerializeField] public List<Color> SkinColors { get; private set; }
        [field: SerializeField] public List<Color> HairColors { get; private set; }
        [field: SerializeField] public List<Color> ClothesColors { get; private set; }
        [field: SerializeField] public List<Color> BootsColors { get; private set; }

        public Color GetRandomSkinColor()
        {
            return SkinColors[Random.Range(0, SkinColors.Count)];
        }
        public Color GetRandomHairColor()
        {
            return HairColors[Random.Range(0, HairColors.Count)];
        }
        public Color GetRandomClothesColor()
        {
            return ClothesColors[Random.Range(0, ClothesColors.Count)];
        }
        public Color GetRandomBootsColor()
        {
            return BootsColors[Random.Range(0, BootsColors.Count)];
        }
    }
}
