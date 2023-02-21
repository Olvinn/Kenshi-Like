using Interfaces;

namespace Units.Commands.SimpleCommands
{
    public class AttackCommand : Command
    {
        public override CommandType Type => CommandType.Attack;
        public override string CommandName => "Hitting";
        public IKillable Target;
        
        public AttackCommand(IKillable target, bool isDirectCommand)
        {
            IsDirectCommand = isDirectCommand;
            Target = target;
        }

        public override void ExecuteBy(Unit executor)
        {
            base.ExecuteBy(executor);

            if (!Executor.CanAttack(Target))
            {
                Done();
                return;
            }
            
            Target.PreGetDamage(Executor);
            Executor.Attack(Target);
        }

        public override void Update()
        {
            base.Update();

            if (!Executor.view.IsAttacking())
                Done();
        }

        public override void Dispose()
        {
            Target = null;
            
            base.Dispose();
        }
    }
}
