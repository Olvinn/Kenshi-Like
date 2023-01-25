using Damages;
using UnityEngine;

namespace Interfaces
{
    public interface IKillable
    {
        bool IsDead { get; }
        Transform transform { get; }
        void PreGetDamage(IUnit attacker);
        void GetDamage(Damage damage);
        void Die();
    }
}
