using System;
using UnityEngine;

namespace Units.Weapons
{
    public class WeaponTriggerDetector : MonoBehaviour
    {
        public Action<UnitView> OnTriggerWithUnit;
        private void OnTriggerEnter(Collider other)
        {
            var unit = other.GetComponent<UnitView>();
            if (unit)
                OnTriggerWithUnit?.Invoke(unit);
        }
    }
}
