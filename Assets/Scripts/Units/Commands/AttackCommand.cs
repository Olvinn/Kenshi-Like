using UnityEngine;

namespace Units.Commands
{
    public class AttackCommand : Command
    {
        public override CommandType Type => CommandType.Attack;
        public override string CommandName => "Attack"; 
        
        private Unit _attacker, _target;
        
        public AttackCommand(Unit attacker, Unit target, bool isDirectCommand)
        {
            IsDirectCommand = isDirectCommand;
            _target = target;
            _attacker = attacker;
        }

        public override void Execute()
        {
            base.Execute();
            if (_attacker == null || _target == null || _target.IsDead)
            {
                Done();
                return;
            }

            GetCloser();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_attacker != null)
                _attacker.View.PerformIdleAnimation();
            _attacker = null;
            _target = null;
        }

        public override void Update()
        {
            if (_target.IsDead)
            {
                Done();
                return;
            }
            
            if (!IsRunning)
                return;
            
            base.Update();

            if (_attacker.View.CanAttack(_target.View))
            {
                if (_attacker.View.FightStatus == FightStatus.Waiting)
                    Attack();
            }
            else
            {
                GetCloser();
            }
        }

        private void GetCloser()
        {
            _attacker.MoveTo(_target.Position);
        }

        private void Attack()
        {
            _attacker.Attack(_target);
        }

        public override bool Equals(object obj)
        {
            var c = obj as AttackCommand;
            if (c == null)
                return false;
            if (c._attacker.Equals(_attacker) && c._target.Equals(_target))
                return true;
            return false;
        }
    }
}
