using Data;
using Interfaces;
using OldUnits.Commands.SimpleCommands;
using UnityEngine;

namespace OldUnits.Commands.ComplexCommands
{
    public class FightCommand : ComplexCommand
    {
        public override CommandType Type => CommandType.Attack;
        public override string CommandName => $"Attacking: {_currentCommand.CommandName}";
        public IKillable Target;
        
        public FightCommand(IKillable target, bool isDirectCommand)
        {
            _commandQueue.Enqueue(new MoveCommand(target.transform, Vector3.zero, GameContext.Instance.Constants.AttackDistance, isDirectCommand));
            _commandQueue.Enqueue(new AttackCommand(target, isDirectCommand));
            _repeat = true;
            
            IsDirectCommand = isDirectCommand;
            Target = target;
        }

        public override void ExecuteBy(Unit executor)
        {
            base.ExecuteBy(executor);
            
            if (executor == null || Target == null || Target.IsDead)
            {
                Done();
                return;
            }
            
            executor.view.PerformFightReadyAnimation();
            Target.PreGetDamage(executor);
        }

        public override void Dispose()
        {
            if (Executor != null)
            {
                Executor.view.Target = null;
                Executor.view.PerformIdleAnimation();
            }
            
            Target = null;
            base.Dispose();
        }

        public override void Update()
        {
            if (Target == null || Target.IsDead)
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
