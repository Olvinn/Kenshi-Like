namespace Units.Commands.SimpleCommands
{
    public class AttackCommand : Command
    {
        public override CommandType Type => CommandType.Attack;
        public override string CommandName => "Hitting";
        public IDamageable Target;
        
        public AttackCommand(IDamageable target, bool isDirectCommand)
        {
            IsDirectCommand = isDirectCommand;
            Target = target;
        }

        public override void ExecuteBy(Unit executor)
        {
            base.ExecuteBy(executor);

            if (!Executor.CanHit(Target))
            {
                Done();
                return;
            }
            
            Target.OnPreHitBy(Executor);
            Executor.Attack(Target);
        }

        public override void Update()
        {
            base.Update();

            if (!Executor.View.IsAttacking())
                Done();
        }

        public override void Dispose()
        {
            Target = null;
            
            base.Dispose();
        }
    }
}
