using System.Collections.Generic;
using Player;
using UnityEngine;
using Units;
using Random = UnityEngine.Random;

public class UnitsController : MonoBehaviour
{
    [SerializeField] private UnitView prefab;
    [SerializeField] private PlayerController player;

    private List<Unit> _units;

    private void Start()
    {
        _units = new List<Unit>();
        
        CreatePlayerUnit(new Vector3(-2f, 0, 0));
        CreatePlayerUnit(new Vector3(0, 0, 0));
        
        CreateAIUnit(new Vector3(2f, 0, 0));
        CreateAIUnit(new Vector3(4f, 0, 0));
    }

    void Update()
    {
        foreach (var unit in _units)
        {
            unit.Update();
        }
    }

    void CreatePlayerUnit(Vector3 pos)
    {
        var unit = CreateUnit(TeamEnum.Player, pos);
        player.AddControlledUnit(unit);
    }

    void CreateAIUnit(Vector3 pos)
    {
        var unit = CreateUnit(TeamEnum.EnemyAI, pos);
    }

    Unit CreateUnit(TeamEnum team, Vector3 pos)
    {
        UnitData data = new UnitData()
        {
            HP = Random.Range(50, 151), Damage = Random.Range(10, 26), Speed = Random.Range(3, 8),
            AttackRate = Random.Range(1f, 2f),
            Color = team == TeamEnum.Player ? Color.cyan : Color.red
        };
        var unit = new Unit(data,team);
        var view = Instantiate(prefab);
        view.transform.position = pos;
        unit.InjectView(view);
        view.InjectModel(unit);
        _units.Add(unit);
        return unit;
    }
}
