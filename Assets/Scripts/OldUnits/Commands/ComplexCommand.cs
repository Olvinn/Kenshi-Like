using System.Collections.Generic;

namespace OldUnits.Commands
{
    public class ComplexCommand : Command
    {
        protected Queue<Command> _commandQueue;
        protected Command _currentCommand;
        protected bool _repeat = false;

        protected ComplexCommand() : base()
        {
            _commandQueue = new Queue<Command>();
        }

        public override void ExecuteBy(Unit executor)
        {
            base.ExecuteBy(executor);

            if (_currentCommand == null && (_commandQueue == null || _commandQueue.Count == 0))
            {
                Done();
                return;
            }

            if (_currentCommand == null)
                DequeueCommand();
            else
                _currentCommand.OnDone += NextCommand;
        }

        public override void Update()
        {
            base.Update();
            
            _currentCommand.Update();
        }

        public override void Interrupt()
        {
            base.Interrupt();
            _currentCommand.OnDone -= NextCommand;
        }

        protected void DequeueCommand()
        { 
            _currentCommand = _commandQueue.Dequeue();
            _currentCommand.OnDone += NextCommand;
            _currentCommand.ExecuteBy(Executor);
        }

        protected void NextCommand()
        {
            _currentCommand.OnDone -= NextCommand;
            if (_repeat)
            {
                _currentCommand.Interrupt();
                _commandQueue.Enqueue(_currentCommand);
            }
            else
            {
                _currentCommand.Dispose();
                _currentCommand = null;
            }
            DequeueCommand();
        }

        public override void Dispose()
        {
            _currentCommand.OnDone -= NextCommand;
            _currentCommand.Dispose();
            _commandQueue = null;
            base.Dispose();
        }
    }
}
