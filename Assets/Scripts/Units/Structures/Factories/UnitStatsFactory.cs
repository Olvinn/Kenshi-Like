using UnityEngine;

namespace Units.Structures.Factories
{
    public static class UnitStatsFactory
    {
        public static UnitStats CreateRandomZombie()
        {
            var result = new UnitStats()
            {
                speed = Random.Range(1f, 3f),
                attackPower = Random.Range(5f, 15f),
                healthPoints = Random.Range(75, 300)
            };
            return result;
        }
        
        public static UnitStats CreateRandomMen()
        {
            var result = new UnitStats()
            {
                speed = Random.Range(2f, 5f),
                attackPower = Random.Range(50f, 100f),
                healthPoints = Random.Range(100, 200)
            };
            return result;
        }
    }
}
