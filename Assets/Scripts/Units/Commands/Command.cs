using System;

namespace Units.Commands
{
    public class Command : IDisposable
    {
        public bool IsDirectCommand { get; protected set; }
        public virtual CommandType Type => CommandType.Command;
        public virtual string CommandName => "Command"; 
        public event Action OnDone;

        public bool IsRunning { get; protected set; } = false;

        public virtual void Execute()
        {
            IsRunning = true;
        }

        public void Interrupt()
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
