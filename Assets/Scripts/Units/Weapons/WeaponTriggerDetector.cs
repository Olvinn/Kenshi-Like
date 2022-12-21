using System;
using System.Collections.Generic;
using UnityEngine;

namespace Units.Weapons
{
    public class WeaponTriggerDetector : MonoBehaviour
    {
        public List<UnitView> views;
        
        private void OnTriggerEnter(Collider other)
        {
            var unit = other.GetComponent<UnitView>();
            if (unit)
                views.Add(unit);
        }

        private void OnTriggerExit(Collider other)
        {
            var unit = other.GetComponent<UnitView>();
            if (unit && views.Contains(unit))
                views.Remove(unit);
        }
    }
}
