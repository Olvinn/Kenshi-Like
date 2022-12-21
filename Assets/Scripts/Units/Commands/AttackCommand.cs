using UnityEngine;

namespace Units.Commands
{
    public class AttackCommand : Command
    {
        private Unit _attacker, _target;
        
        public AttackCommand(Unit attacker, Unit target)
        {
            _target = target;
            _attacker = attacker;
        }

        public override void Execute()
        {
            Debug.Log($"Executing attack command to target {_target}");
            _attacker.View.PerformFightReadyAnimation();
            base.Execute();
            GetCloser();
        }

        public override void Dispose()
        {
            base.Dispose();
            _attacker.View.PerformIdleAnimation();
            _attacker.View.OnReachDestination = null;
            _attacker = null;
            _target = null;
        }

        private void GetCloser()
        {
            _attacker.MoveTo(_target.View.transform);
            _attacker.View.OnReachDestination += Attack;
        }

        private void Attack()
        {
            _attacker.View.OnReachDestination = null;
            if (_target.IsDead)
            {
                _attacker.View.PerformIdleAnimation();
                Done();
                return;
            }

            if (_attacker.View.CanAttack(_target.View))
            {
                _attacker.Attack(_target, Attack);
            }
            else
            {
                GetCloser();
            }
        }
    }
}
