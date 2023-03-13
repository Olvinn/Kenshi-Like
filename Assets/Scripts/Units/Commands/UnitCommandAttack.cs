using Units.MVC.Controller;
using Units.MVC.View;
using Units.Structures;
using UnityEngine;

namespace Units.Commands
{
    public class UnitCommandAttack : UnitCommand
    {
        private UnitView _target;
        
        public UnitCommandAttack(UnitView target)
        {
            _target = target;
        }

        public override void Execute(CommandsUnitController executor)
        {
            base.Execute(executor);
            executor.MoveTo(_target.transform.position);
        }

        public override void Update()
        {
            base.Update();
            if (Vector3.Distance(_executor.GetViewPosition(), _target.transform.position) < 1.5f)
            {
                _executor.Attack();
                onComplete?.Invoke();
            }
            else
            {
                _executor.MoveTo(_target.transform.position);
            }
        }
    }
}
