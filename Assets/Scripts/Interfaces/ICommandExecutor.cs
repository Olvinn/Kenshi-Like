using Units.Commands;

namespace Interfaces
{
    public interface ICommandExecutor
    {
        void EnqueueCommand(Command command);
        void ExecuteCommands();
        void ClearCommandQueue();
    }
}
