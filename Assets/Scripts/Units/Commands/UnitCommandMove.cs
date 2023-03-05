using Units.MVC.Controller;
using Units.Structures;
using UnityEngine;

namespace Units.Commands
{
    public class UnitCommandMove : UnitCommand
    {
        private Vector3 _destination;
        private float _threshold;
        
        public UnitCommandMove(Vector3 destination, float threshold = .5f)
        {
            _destination = destination;
            _threshold = threshold;
        }

        public override void Execute(UnitController executor)
        {
            base.Execute(executor);
            
            _executor.MoveTo(_destination);
        }

        public override void Update()
        {
            base.Update();
            
            if (_executor != null && _executor.state == UnitState.Idle)
            {
                _executor.MoveTo(_destination);
            }
            
            if (Vector3.Distance(_executor.GetViewPosition(), _destination) <= _threshold)
            {
                onComplete?.Invoke();
            }
        }
    }
}
