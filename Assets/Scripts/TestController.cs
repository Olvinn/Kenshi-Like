using Player;
using UnityEngine;
using Units;

public class TestController : MonoBehaviour
{
    [SerializeField] private UnitView unitView1, unitView2, unitView3;
    [SerializeField] private PlayerController player;
    
    void Start()
    {
        UnitStats stats = new UnitStats()
        {
            HP = Random.Range(50, 151), Damage = Random.Range(10, 26), Speed = Random.Range(3, 8),
            AttackRate = Random.Range(1f, 2f)
        };
        var unit1 = new Unit(stats);
        unit1.InjectView(unitView1);
        
        stats = new UnitStats()
        {
            HP = Random.Range(50, 151), Damage = Random.Range(10, 26), Speed = Random.Range(3, 8),
            AttackRate = Random.Range(1f, 2f)
        };
        var unit2 = new Unit(stats);
        unit2.InjectView(unitView2);

        stats = new UnitStats()
        {
            HP = Random.Range(50, 151), Damage = Random.Range(10, 26), Speed = Random.Range(3, 8),
            AttackRate = Random.Range(1f, 2f)
        };
        var unit3 = new Unit(stats);
        unit3.InjectView(unitView3);
        
        player.AddControlledUnit(unit1);
        player.AddControlledUnit(unit2);
        player.AddEnemyUnit(unit3);
    }

    void Update()
    {
        
    }
}
