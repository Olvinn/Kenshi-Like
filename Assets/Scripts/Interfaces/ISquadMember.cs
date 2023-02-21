namespace Interfaces
{
    public interface ISquadMember
    {
        ISquad squad { get; }
        void SetSquad(ISquad squad);
        void EnableIndividualSense();
        void DisableIndividualSense();
    }
}
