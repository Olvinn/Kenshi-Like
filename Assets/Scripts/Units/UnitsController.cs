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
    [SerializeField] private UnitView prefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private AIController bot;

    [SerializeField] private HPBarsController hps;
    [SerializeField] private LayerMask mask;

    private List<Unit> _units;
    int _j = 0;

    private void Awake()
    {
        _units = new List<Unit>();
    }

    private void Start()
    {
        player.SetTeam(TeamEnum.Player);
        bot.SetTeam(TeamEnum.EnemyAI);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < 25; i++)
        {
            _units[(i + _j) % _units.Count].Update();
            _units[Random.Range(0, _units.Count)].Update();
        }
    }

    public void CreatePlayerUnit(Character data, Vector3 pos)
    {
        data = ScriptableObject.Instantiate(data);
        var unit = CreateUnit(data, pos);
        player.AddUnit(unit);
    }

    public void CreateAIUnit(Character data, Vector3 pos)
    {
        data = ScriptableObject.Instantiate(data);
        var unit = CreateUnit(data, pos);
        bot.AddUnit(unit);
    }

    Unit CreateUnit(Character data, Vector3 pos)
    {
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
