using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Players;
using UI;
using UnityEngine;
using Units;
using Units.Views;
using Random = UnityEngine.Random;

public class UnitsController : MonoBehaviour
{
    [SerializeField] private List<Character> charactersPresets;
    [SerializeField] private int unitCount;
    
    [SerializeField] private UnitView prefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private AIController bot;

    [SerializeField] private HPBarsController hps;
    [SerializeField] private LayerMask mask;

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
        var unit = CreateUnit(pos);
        player.AddUnit(unit);
    }

    void CreateAIUnit(Vector3 pos)
    {
        var unit = CreateUnit(pos);
        bot.AddUnit(unit);
    }

    Unit CreateUnit(Vector3 pos)
    {
        Character data = charactersPresets[Random.Range(0, charactersPresets.Count)];
        var unit = new Unit(data);
        var view = Instantiate(prefab);
        
        RaycastHit hit;
        Ray ray = new Ray(pos + Vector3.up * 1000, Vector3.down);
        if (Physics.Raycast(ray, out hit, 2000f, mask))
        {
            view.SetPosition(hit.point + Vector3.up);
        }
        
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
