using System;
using System.Collections.Generic;
using Units.Views;
using Units.Weapons;
using UnityEngine;

namespace Units
{
    public class UnitAttack : MonoBehaviour
    {
        [SerializeField] private TriggerDetector front;
        
        public List<UnitView> GetUnitViewsFromBasicAttack()
        {
            return front.views;
        }

        public void OnDisable()
        {
            front.enabled = false;
        }

        private void OnEnable()
        {
            front.enabled = true;
        }
    }
}
