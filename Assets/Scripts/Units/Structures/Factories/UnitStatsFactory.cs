using UnityEngine;

namespace Units.Structures.Factories
{
    public static class UnitStatsFactory
    {
        public static UnitStats CreateRandomZombie()
        {
            var result = new UnitStats()
            {
                speed = Random.Range(1f, 5f),
                attackPower = Random.Range(5f, 15f),
                healthPoints = Random.Range(75, 300)
            };
            return result;
        }
    }
}
