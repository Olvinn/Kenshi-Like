using System;

namespace OldUnits.Commands
{
    public class Command : IDisposable
    {
        public bool IsDirectCommand { get; protected set; }
        public virtual CommandType Type => CommandType.Command;
        public virtual string CommandName => "Command"; 
        public event Action OnDone;
        public Unit Executor { get; private set; }
        public bool IsRunning { get; protected set; } = false;

        public virtual void ExecuteBy(Unit executor)
        {
            Executor = executor;
            IsRunning = true;
        }

        public virtual void Interrupt()
        {
            IsRunning = false;
        }

        protected void Done()
        {
            Interrupt();
            OnDone?.Invoke();
        }

        public virtual void Dispose()
        {
            Executor = null;
            OnDone = null;
        }

        public virtual void Update()
        {
        }
    }

    public enum CommandType
    {
        Command,
        Move,
        Attack,
        Follow
    }
}
