using OldUnits;

namespace OldPlayers
{
    public interface IPlayer
    {
        void AddUnit(Unit unit);
        void SetTeam(TeamEnum team);
    }
}
