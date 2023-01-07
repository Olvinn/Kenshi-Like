using System;
using UnityEngine;

namespace Units
{
    public class UnitAnimationEventCatcher : MonoBehaviour
    {
        public event Action OnHitBasic, OnGetDamageComplete, OnAttackComplete, OnDodgingComplete;

        private void HitBasic()
        {
            OnHitBasic?.Invoke();
        }
        
        private void AttackComplete()
        {
            OnAttackComplete?.Invoke();
        }
        
        private void GetDamageComplete()
        {
            OnGetDamageComplete?.Invoke();
        }
        
        private void DodgingComplete()
        {
            OnDodgingComplete?.Invoke();
        }
    }
}
