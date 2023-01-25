namespace Interfaces
{
    public interface IAttacking
    {
        bool CanAttack(IKillable target);
        void Attack(IKillable target);
    }
}
