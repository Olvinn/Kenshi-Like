using UnityEngine;

namespace Units.Commands
{
    public class AttackCommand : Command
    {
        public override string CommandName => "Attack"; 
        
        private Unit _attacker, _target;
        
        public AttackCommand(Unit attacker, Unit target)
        {
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

            _attacker.View.PerformFightReadyAnimation();
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
            if (!IsRunning)
                return;
            
            base.Update();
            
            if (_attacker.View.MovementStatus != MovementStatus.Waiting) 
                return;
            
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
            _attacker.MoveTo(_target.View.transform);
        }

        private void Attack()
        {
            if (_target.IsDead)
            {
                _attacker.View.PerformIdleAnimation();
                Done();
                return;
            }

            if (_attacker.View.CanAttack(_target.View))
            {
                _attacker.Attack(_target);
            }
            else
            {
                GetCloser();
            }
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
