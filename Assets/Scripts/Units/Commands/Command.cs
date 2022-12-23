using System;
using UnityEngine;

namespace Units.Commands
{
    public class Command : IDisposable
    {
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
}
