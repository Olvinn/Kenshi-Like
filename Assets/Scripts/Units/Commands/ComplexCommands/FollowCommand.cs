using Units.Commands.SimpleCommands;
using UnityEngine;

namespace Units.Commands.ComplexCommands
{
    public class FollowCommand : ComplexCommand
    {
        public override CommandType Type => CommandType.Follow;
        public override string CommandName => "Follow"; 
        
        private Unit _unitToFollow;
        private Vector3 _offset;

        public FollowCommand(Unit unitToFollow, Vector3 offset, bool isDirectCommand)
        {
            _commandQueue.Enqueue(new MoveCommand(unitToFollow.ViewTransform, offset, .1f, isDirectCommand));
            _repeat = true;
            
            IsDirectCommand = isDirectCommand;
            _unitToFollow = unitToFollow;
            _offset = offset;
        }

        public override void ExecuteBy(Unit executor)
        {
            base.ExecuteBy(executor);
            if (executor.Equals(_unitToFollow))
                Done();
        }

        public override void Update()
        {
            if (!IsRunning)
                return;
            
            base.Update();
        }

        public override void Dispose()
        {
            _unitToFollow = null;
            base.Dispose();
        }

        public override bool Equals(object obj)
        {
            var c = obj as FollowCommand;
            if (c == null)
                return false;
            if (Executor != null && Executor.Equals(c.Executor) &&
                _unitToFollow != null && _unitToFollow.Equals(c._unitToFollow))
                return true;
            return false;
        }
    }
}
