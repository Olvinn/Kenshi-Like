using Data;
using Units.Commands.SimpleCommands;
using UnityEngine;

namespace Units.Commands.ComplexCommands
{
    public class FightCommand : ComplexCommand
    {
        public override CommandType Type => CommandType.Attack;
        public override string CommandName => $"Attacking: {_currentCommand.CommandName}";
        public IDamageable Target;
        
        public FightCommand(IDamageable target, bool isDirectCommand)
        {
            _commandQueue.Enqueue(new MoveCommand(target.ViewTransform, Vector3.zero, Constants.instance.AttackDistance, isDirectCommand));
            _commandQueue.Enqueue(new AttackCommand(target, isDirectCommand));
            _repeat = true;
            
            IsDirectCommand = isDirectCommand;
            Target = target;
        }

        public override void ExecuteBy(Unit executor)
        {
            base.ExecuteBy(executor);
            
            if (executor == null || Target == null || Target.IsDestroyed)
            {
                Done();
                return;
            }
            
            executor.View.PerformFightReadyAnimation();
            Target.OnPreAttackBy(executor);
        }

        public override void Dispose()
        {
            if (Executor != null)
            {
                Executor.View.Target = null;
                Executor.View.PerformIdleAnimation();
            }
            
            if (Target != null)
                Target.OnPostAttackBy(Executor);
            
            Target = null;
            base.Dispose();
        }

        public override void Update()
        {
            if (Target == null || Target.IsDestroyed)
            {
                Done();
                return;
            }
            
            if (!IsRunning)
                return;
            
            base.Update();
        }

        public override bool Equals(object obj)
        {
            var c = obj as FightCommand;
            if (c == null)
                return false;
            if (Executor != null && Executor.Equals(c.Executor) &&
                Target != null && Target.Equals(c.Target))
                return true;
            return false;
        }
    }
}
