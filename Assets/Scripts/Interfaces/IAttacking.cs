using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public interface IAttacking
{
    bool CanAttack(IKillable target);
    void Attack(IKillable target);
}
