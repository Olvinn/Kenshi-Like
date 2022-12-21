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
            _attacker = null;
            _target = null;
        }

        public override void Update()
        {
            base.Update();
            if (_attacker.View.MovementStatus == MovementStatus.Aimless)
            {
                if (_attacker.View.CanAttack(_target.View))
                {
                    if (_attacker.View.FightStatus == FightStatus.Idle)
                        Attack();
                }
                else
                {
                    GetCloser();
                }
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
    }
}
