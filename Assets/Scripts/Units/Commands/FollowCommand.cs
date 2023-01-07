using Units.Views;

namespace Units.Commands
{
    public class FollowCommand : Command
    {
        public override CommandType Type => CommandType.Follow;
        public override string CommandName => "Follow"; 
        
        private Unit _unitToFollow;

        public FollowCommand(Unit unitToFollow, bool isDirectCommand)
        {
            IsDirectCommand = isDirectCommand;
            _unitToFollow = unitToFollow;
        }

        public override void Do(Unit owner)
        {
            base.Do(owner);
            if (owner.Equals(_unitToFollow))
            {
                Done();
                return;
            }
            owner.MoveTo(_unitToFollow.View.transform, 3f);
        }

        public override void Update()
        {
            if (!IsRunning)
                return;
            
            if (_unitToFollow.View.MovementStatus != MovementStatus.Waiting)
                CommandOwner.MoveTo(_unitToFollow.View.transform, 3f);
            
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
            if (CommandOwner != null && CommandOwner.Equals(c.CommandOwner) &&
                _unitToFollow != null && _unitToFollow.Equals(c._unitToFollow))
                return true;
            return false;
        }
    }
}
