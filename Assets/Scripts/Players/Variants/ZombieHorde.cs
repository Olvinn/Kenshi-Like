using Units.Commands;
using Units.MVC.Model;
using Units.MVC.View;
using UnityEngine;

namespace Players.Variants
{
    public class ZombieHorde : RTSPlayer
    {
        public int unitsCount => _manager.unitsCount;
        
        private int _count;
        private UnitModel[] _models;
        private UnitView _player;
        
        public void CreateHorde(int count, UnitModel[] models, UnitView player)
        {
            _player = player;
            _count = count;
            _models = models;
        }

        public override void Update()
        {
            base.Update();
            if (_manager.unitsCount < _count)
            {
                var temp = _models[Random.Range(0, _models.Length)];
                var model = new UnitModel(temp.GetStats(), temp.GetAppearance());
                model.SetPosition(new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50)));
                _manager.AddUnit(model);
                AttackPlayer(model);
                // SetRandomDestination(model);
                _manager.onUnitCompleteCommands += AttackPlayer;
            }
        }

        private void AttackPlayer(UnitModel unit)
        {
            _manager.AddCommand(unit, new UnitCommandAttack(_player));
        }

        private void SetRandomDestination(UnitModel unit)
        {
            if (Random.Range(0f, 1f) > .5f)
                _manager.AddCommand(unit, new UnitCommandMove(new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50))));
            else
                _manager.AddCommand(unit, new UnitCommandStay(Random.Range(10, 30)));
        }
    }
}
