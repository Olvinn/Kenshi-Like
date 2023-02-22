using OldUnits;
using OldUnits.Views;

namespace Interfaces
{
    public interface IUnit : IKillable, IMoving, ICommandExecutor, IAttacking, ISquadMember
    {
        TeamEnum team { get; }
        UnitView view { get; }
        public void SetTeam(TeamEnum team);
        public void InjectView(UnitView view);
        public void Update();
    }
}
