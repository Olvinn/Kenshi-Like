namespace Units.Commands
{
    public class AttackCommand : Command
    {
        public override CommandType Type => CommandType.Attack;
        public override string CommandName => "Attack";
        public Unit Target;
        
        public AttackCommand(Unit target, bool isDirectCommand)
        {
            IsDirectCommand = isDirectCommand;
            Target = target;
        }

        public override void Do(Unit owner)
        {
            base.Do(owner);
            
            if (owner == null || Target == null || Target.IsDead)
            {
                Done();
                return;
            }
            
            owner.View.Target = Target.View;
            owner.View.PerformFightReadyAnimation();

            GetCloser();
        }

        public override void Dispose()
        {
            if (CommandOwner != null)
            {
                CommandOwner.View.Target = null;
                CommandOwner.View.PerformIdleAnimation();
            }
            Target = null;
            base.Dispose();
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

            if (CommandOwner.CanAttack(Target))
            {
                Attack();
            }
            else
            {
                GetCloser();
            }
        }

        private void GetCloser()
        {
            CommandOwner.MoveTo(Target.View.transform, 2f);
        }

        private void Attack()
        {
            CommandOwner.Attack(Target);
        }

        public override bool Equals(object obj)
        {
            var c = obj as AttackCommand;
            if (c == null)
                return false;
            if (CommandOwner != null && CommandOwner.Equals(c.CommandOwner) &&
                Target != null && Target.Equals(c.Target))
                return true;
            return false;
        }
    }
}
