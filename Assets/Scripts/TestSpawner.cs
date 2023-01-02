using System.Collections.Generic;
using Data;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    [SerializeField] private float spawnerRadius = 10f;
    [SerializeField] private int enemiesCountInCamp = 5;
    
    [SerializeField] private Transform playerPos;
    [SerializeField] private List<Transform> enemyPos;

    [SerializeField] private List<Character> playerSquad;
    [SerializeField] private List<Character> enemies;

    [SerializeField] private UnitsController uc;

    private void Start()
    {
        CreatePlayerSquad();
        CreateEnemies();
    }

    void CreatePlayerSquad()
    {
        foreach (var c in playerSquad)
        {
            Vector3 pos = Random.insideUnitSphere * spawnerRadius + playerPos.position;
            uc.CreatePlayerUnit(c, pos);
        }
    }

    void CreateEnemies()
    {
        foreach (var t in enemyPos)
        {
            for (int i = 0; i < enemiesCountInCamp; i++)
            {
                Vector3 pos = Random.insideUnitSphere * spawnerRadius + t.position;
                uc.CreateAIUnit(enemies[Random.Range(0, enemies.Count)], pos);
            }
        }
    }
}
