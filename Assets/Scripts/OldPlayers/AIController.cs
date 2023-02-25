using System.Collections.Generic;
using OldUnits;
using UnityEngine;

namespace OldPlayers
{
    public class AIController : MonoBehaviour, IPlayer
    {
        private List<Unit> _units;
        private TeamEnum _team;

        private void Awake()
        {
            _units = new List<Unit>();
        }

        void Start()
        {
        
        }

        void Update()
        {
        
        }

        public void AddUnit(Unit unit)
        {
            _units.Add(unit);
            unit.SetTeam(_team);
        }

        public void SetTeam(TeamEnum team)
        {
            _team = team;
            foreach (var unit in _units)
            {
                unit.SetTeam(team);
            }
        }
    }
}
