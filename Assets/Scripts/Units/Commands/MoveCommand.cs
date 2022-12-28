using Units.Views;
using UnityEngine;

namespace Units.Commands
{
    public class MoveCommand : Command
    {
        public override CommandType Type => CommandType.Move;
        public override string CommandName => "Move"; 
        
        private Unit _unit;
        private Vector3 _pos;

        public MoveCommand(Unit unit, Vector3 pos, bool isDirectCommand)
        {
            IsDirectCommand = isDirectCommand;
            _unit = unit;
            _pos = pos;
        }

        public override void Execute()
        {
            base.Execute();
            _unit.MoveTo(_pos);
        }

        public override void Update()
        {
            if (!IsRunning)
                return;
            
            base.Update();
            if (_unit.View.MovementStatus == MovementStatus.Waiting)
                Done();
        }
        
        public override bool Equals(object obj)
        {
            var c = obj as MoveCommand;
            if (c == null)
                return false;
            if (c._unit.Equals(_unit) && c._pos == _pos)
                return true;
            return false;
        }
    }
}
