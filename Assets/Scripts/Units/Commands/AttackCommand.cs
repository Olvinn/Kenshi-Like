using UnityEngine;

namespace Units.Commands
{
    public class AttackCommand : Command
    {
        public override CommandType Type => CommandType.Attack;
        public override string CommandName => "Attack";
        public Unit Target;
        
        private Unit _attacker;
        
        public AttackCommand(Unit attacker, Unit target, bool isDirectCommand)
        {
            IsDirectCommand = isDirectCommand;
            Target = target;
            _attacker = attacker;
        }

        public override void Execute()
        {
            base.Execute();
            if (_attacker == null || Target == null || Target.IsDead)
            {
                Done();
                return;
            }

            GetCloser();
        }

        public override void Dispose()
        {
            base.Dispose();
            _attacker = null;
            Target = null;
        }

        public override void Update()
        {
            if (Target.IsDead)
            {
                Done();
                return;
            }
            
            if (!IsRunning)
                return;
            
            base.Update();

            if (_attacker.View.CanAttack(Target.View))
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
            _attacker.MoveTo(Target.View.transform);
        }

        private void Attack()
        {
            _attacker.Attack(Target);
        }

        public override bool Equals(object obj)
        {
            var c = obj as AttackCommand;
            if (c == null)
                return false;
            if (c._attacker.Equals(_attacker) && c.Target.Equals(Target))
                return true;
            return false;
        }
    }
}
