using System.Collections.Generic;

namespace Interfaces
{
    public interface ISquad : IMoving, IAttacking, ICommandExecutor
    {
        void AddMembers(IEnumerable<ISquadMember> members);
        void AddMember(ISquadMember member);
        void RemoveMember(ISquadMember member);
        void RemoveAllMembers();
    }
}
