using System;
using System.Collections;
using System.Collections.Generic;
using Players;
using UI;
using UnityEngine;
using Units;
using Units.Commands;
using Random = UnityEngine.Random;

public class UnitsController : MonoBehaviour
{
    [SerializeField] private int unitCount;
    
    [SerializeField] private UnitView prefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private AIController bot;

    [SerializeField] private HPBarsController hps;

    private List<Unit> _units;

    private void Start()
    {
        _units = new List<Unit>();
        
        player.SetTeam(TeamEnum.Player);
        bot.SetTeam(TeamEnum.EnemyAI);
        for(int i = 0; i < unitCount; i++)
        {
            if (i % 2 == 0)
            {
                CreatePlayerUnit(new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50)));
            }
            else
            {
                CreateAIUnit(new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50)));
            }
        }
        hps.SetUpUnits(_units);
    }

    int j = 0;
    void FixedUpdate()
    {
        for (int i = 0; i < 25; i++)
        {
            _units[(i + j) % _units.Count].Update();
            _units[Random.Range(0, _units.Count)].Update();
        }
    }

    void CreatePlayerUnit(Vector3 pos)
    {
        var unit = CreateUnit(TeamEnum.Player, pos);
        player.AddUnit(unit);
    }

    void CreateAIUnit(Vector3 pos)
    {
        var unit = CreateUnit(TeamEnum.EnemyAI, pos);
        bot.AddUnit(unit);
    }

    Unit CreateUnit(TeamEnum team, Vector3 pos)
    {
        UnitData data = new UnitData()
        {
            HP = Random.Range(50, 151), Damage = Random.Range(10, 26), Speed = Random.Range(3, 8),
            AttackRate = Random.Range(1f, 2f),
            Color = team == TeamEnum.Player ? Color.cyan : Color.red
        };
        var unit = new Unit(data);
        var view = Instantiate(prefab);
        view.transform.position = pos;
        unit.InjectView(view);
        view.InjectModel(unit);
        _units.Add(unit);
        unit.OnDie += RemoveUnit;
        return unit;
    }

    void RemoveUnit(Unit unit)
    {
        _units.Remove(unit);
        // StartCoroutine(DelayedAction(()=>Destroy(unit.View.gameObject), 10));
    }

    IEnumerator DelayedAction(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
