using System;
using Units.MVC.Controller;

namespace Units.Commands
{
    public class UnitCommand : IDisposable
    {
        public Action onComplete;
        protected NPCUnitController _executor;

        public virtual void Execute(NPCUnitController executor)
        {
            _executor = executor;
        }
        
        public virtual void Update() { }

        public virtual void Dispose()
        {
            onComplete = null;
            _executor = null;
        }
    }
}
