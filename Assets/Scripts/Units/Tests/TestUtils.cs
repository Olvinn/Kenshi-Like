using Units.Structures;
using UnityEngine;

namespace Units.Tests
{
    public static class TestUtils
    {
        public static UnitStats GetTestStats()
        {
            UnitStats stats = new UnitStats()
            {
                healthPoints = 100,
                attackPower = 10,
                speed = 5
            };
            return stats;
        }
        
        public static UnitAppearance GetTestAppearance()
        {
            UnitAppearance appearance = new UnitAppearance()
            {
                skinColor = Color.white,
                baseColor = Color.red,
                secondaryColor = Color.black,
                accentColor = Color.cyan,
                prefab = "Assets/Prefabs/Units/ZombieAppearancePlaceholder.prefab"
            };
            return appearance;
        }
    }
}
