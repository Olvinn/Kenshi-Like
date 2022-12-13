using System;
using UnityEngine;

namespace Units
{
    public class UnitAnimationEventCatcher : MonoBehaviour
    {
        public event Action OnHitComplete, OnGetDamageComplete;

        private void HitComplete()
        {
            OnHitComplete?.Invoke();
        }
        private void GetDamageComplete()
        {
            OnGetDamageComplete?.Invoke();
        }
    }
}
