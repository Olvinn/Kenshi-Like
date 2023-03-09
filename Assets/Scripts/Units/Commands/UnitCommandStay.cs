using Units.MVC.Controller;
using UnityEngine;

namespace Units.Commands
{
    public class UnitCommandStay : UnitCommand
    {
        private float _time;
        private float _startTime;
        
        public UnitCommandStay(float time)
        {
            _time = time;
        }

        public override void Execute(CommandsUnitController executor)
        {
            base.Execute(executor);
            _startTime = Time.time;
        }

        public override void Update()
        {
            base.Update();
            if (Time.time - _startTime >= _time)
                onComplete?.Invoke();
        }
    }
}
