using UnityEngine;

namespace Units.Structures.Factories
{
    public static class UnitAppearanceFactory
    {
        private static readonly  Color[] HumanSkinColors = new[]
        {
            new Color(0.29f, 0.18f, 0.12f), // Dark Brown
            new Color(0.30f, 0.21f, 0.15f), // Brown
            new Color(0.35f, 0.24f, 0.20f), // Light Brown
            new Color(0.38f, 0.24f, 0.20f), // Dark Reddish Brown
            new Color(0.41f, 0.26f, 0.22f), // Reddish Brown
            new Color(0.46f, 0.28f, 0.23f), // Light Reddish Brown
            new Color(0.47f, 0.29f, 0.26f), // Dark Grayish Brown
            new Color(0.51f, 0.35f, 0.28f), // Grayish Brown
            new Color(0.54f, 0.35f, 0.30f), // Light Grayish Brown
            new Color(0.61f, 0.40f, 0.34f), // Dark Gray
            new Color(0.66f, 0.48f, 0.43f), // Gray
            new Color(0.72f, 0.53f, 0.47f), // Light Gray
            new Color(0.78f, 0.60f, 0.54f), // Dark Grayish Pink
            new Color(0.83f, 0.69f, 0.66f), // Grayish Pink
            new Color(0.89f, 0.77f, 0.74f) // Light Grayish Pink
        };
        
        public static UnitAppearance CreateRandomZombie()
        {
            var result = new UnitAppearance()
            {
                skinColor = HumanSkinColors[Random.Range(0, HumanSkinColors.Length)],
                accentColor = Color.green,
                baseColor = Color.green,
                prefab = "Assets/Prefabs/Units/HumanZombie.prefab",
                animationSet = AnimationSetType.Zombie
            };
            return result;
        }
        
        public static UnitAppearance CreateRandomMen()
        {
            var result = new UnitAppearance()
            {
                skinColor = HumanSkinColors[Random.Range(0, HumanSkinColors.Length)],
                accentColor = Color.blue,
                baseColor = Color.blue,
                prefab = "Assets/Prefabs/Units/HumanMale.prefab",
                animationSet = AnimationSetType.GreatSword
            };
            return result;
        }
    }
}
