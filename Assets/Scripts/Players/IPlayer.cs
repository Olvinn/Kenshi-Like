using OldUnits;
using Units;

namespace Players
{
    public interface IPlayer
    {
        void AddUnit(Unit unit);
        void SetTeam(TeamEnum team);
    }
}
