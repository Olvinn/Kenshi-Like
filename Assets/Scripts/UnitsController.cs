using System.Collections.Generic;
using Player;
using UnityEngine;
using Units;
using Units.Commands;
using Random = UnityEngine.Random;

public class UnitsController : MonoBehaviour
{
    [SerializeField] private int unitCount;
    [SerializeField] private bool fightInstantly;
    
    [SerializeField] private UnitView prefab;
    [SerializeField] private PlayerController player;

    private List<Unit> _units;

    private void Start()
    {
        _units = new List<Unit>();

        for (int i = 0; i < unitCount; i++)
        {
            Unit unit;
            if (i % 2 == 0)
            {
                unit = CreatePlayerUnit(new Vector3(-i, 0, 0));
                if (fightInstantly)
                {
                    unit.AddCommand(new AttackCommand(unit, i > 0 ? _units[i - 1] : null));
                    unit.ExecuteCommands();
                }
            }
            else
            {
                unit = CreateAIUnit(new Vector3(i + 2, 0, 0));
                if (fightInstantly)
                {
                    unit.AddCommand(new AttackCommand(unit, i > 0 ? _units[i - 1] : null));
                    unit.ExecuteCommands();
                }
            }
        }
    }

    void Update()
    {
        foreach (var unit in _units)
        {
            unit.Update();
        }
    }

    Unit CreatePlayerUnit(Vector3 pos)
    {
        var unit = CreateUnit(TeamEnum.Player, pos);
        player.AddControlledUnit(unit);
        return unit;
    }

    Unit CreateAIUnit(Vector3 pos)
    {
        var unit = CreateUnit(TeamEnum.EnemyAI, pos);
        return unit;
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
