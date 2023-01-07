using Units.Views;
using UnityEngine;

namespace Units.Commands
{
    public class MoveCommand : Command
    {
        public override CommandType Type => CommandType.Move;
        public override string CommandName => "Move"; 
        
        private Vector3 _pos;

        public MoveCommand(Vector3 pos, bool isDirectCommand)
        {
            IsDirectCommand = isDirectCommand;
            _pos = pos;
        }

        public override void Do(Unit owner)
        {
            base.Do(owner);
            owner.MoveTo(_pos, 1f);
        }

        public override void Update()
        {
            if (!IsRunning)
                return;
            
            base.Update();
            if (CommandOwner.View.MovementStatus == MovementStatus.Waiting)
                Done();
        }
        
        public override bool Equals(object obj)
        {
            var c = obj as MoveCommand;
            if (c == null)
                return false;
            if (CommandOwner != null && CommandOwner.Equals(c.CommandOwner) && c._pos == _pos)
                return true;
            return false;
        }
    }
}
