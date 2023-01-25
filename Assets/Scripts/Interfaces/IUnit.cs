using Interfaces;
using Units;
using Units.Views;

public interface IUnit : IKillable, IMoving, ICommandExecutor, IAttacking
{
    TeamEnum team { get; }
    UnitView view { get; }
    public void SetTeam(TeamEnum team);
    public void InjectView(UnitView view);
    public void Update();
}
