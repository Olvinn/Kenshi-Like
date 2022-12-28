using Units.Views;

namespace Units.Commands
{
    public class FollowCommand : Command
    {
        public override CommandType Type => CommandType.Follow;
        public override string CommandName => "Follow"; 
        
        private Unit _unit;
        private Unit _unitToFollow;

        public FollowCommand(Unit unit, Unit unitToFollow, bool isDirectCommand)
        {
            IsDirectCommand = isDirectCommand;
            _unit = unit;
            _unitToFollow = unitToFollow;
        }

        public override void Execute()
        {
            base.Execute();
            _unit.MoveTo(_unitToFollow.View.transform);
        }

        public override void Update()
        {
            if (!IsRunning)
                return;
            
            if (_unitToFollow.View.MovementStatus != MovementStatus.Waiting)
                _unit.MoveTo(_unitToFollow.View.transform);
            
            base.Update();
        }
        
        public override bool Equals(object obj)
        {
            var c = obj as FollowCommand;
            if (c == null)
                return false;
            if (c._unit.Equals(_unit) && c._unitToFollow.Equals(_unitToFollow))
                return true;
            return false;
        }
    }
}
