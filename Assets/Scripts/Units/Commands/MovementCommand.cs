using UnityEngine;

namespace Units.Commands
{
    public class MovementCommand : Command
    {
        private Unit _unit;
        private Vector3 _pos;

        public MovementCommand(Unit unit, Vector3 pos)
        {
            _unit = unit;
            _pos = pos;
        }

        public override void Execute()
        {
            Debug.Log($"Executing moving command to pos {_pos}");
            _unit.MoveTo(_pos);
        }

        public override void Dispose()
        {
            Debug.Log($"Disposing moving command {_pos}");
            base.Dispose();
        }

        public override void Update()
        {
            base.Update();
            if (_unit.View.MovementStatus == MovementStatus.Waiting)
                Done();
        }
    }
}
