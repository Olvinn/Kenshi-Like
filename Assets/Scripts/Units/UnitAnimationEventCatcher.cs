using System;
using UnityEngine;

namespace Units
{
    public class UnitAnimationEventCatcher : MonoBehaviour
    {
        public event Action OnHitFront, OnGetDamageComplete, OnAttackComplete;

        private void HitFront()
        {
            OnHitFront?.Invoke();
        }
        
        private void AttackComplete()
        {
            OnAttackComplete?.Invoke();
        }
        
        private void GetDamageComplete()
        {
            OnGetDamageComplete?.Invoke();
        }
    }
}
