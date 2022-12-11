using System;
using UnityEngine;

namespace Units.Commands
{
    public class Command : IDisposable
    {
        public event Action OnDone;
        public CommandType Type { get; protected set; }
        public virtual void Execute() { }

        protected void Done()
        {
            OnDone?.Invoke();
        }

        public virtual void Dispose()
        {
            OnDone = null;
        }
    }

    public enum CommandType
    {
        Movement
    }
}
