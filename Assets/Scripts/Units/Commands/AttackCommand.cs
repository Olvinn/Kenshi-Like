using UnityEngine;

namespace Units.Commands
{
    public class AttackCommand : Command
    {
        private Unit _attacker, _target;

        private MovementCommand _movCommand;
        
        public AttackCommand(Unit attacker, Unit target)
        {
            _target = target;
            _attacker = attacker;
        }

        public override void Execute()
        {
            Debug.Log($"Executing attack command to target {_target}");
            base.Execute();
            _attacker.View.SetProvoked();
            _movCommand = new MovementCommand(_attacker, _target.Position);
            _movCommand.OnDone += Attack;
            _movCommand.Execute();
        }

        private void Attack()
        {
            _movCommand.Dispose();
            
            if (_target.IsDead)
            {
                _attacker.View.SetIdle();
                Done();
                return;
            }

            if (_attacker.View.CanAttack(_target.View))
            {
                _attacker.Attack(_target, Attack);
            }
            else
            {
                _movCommand = new MovementCommand(_attacker, _target.Position);
                _movCommand.OnDone += Attack;
            }
        }
    }
}
