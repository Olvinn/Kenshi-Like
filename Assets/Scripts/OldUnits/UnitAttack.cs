using System.Collections.Generic;
using OldUnits.Views;
using OldUnits.Weapons;
using UnityEngine;

namespace OldUnits
{
    public class UnitAttack : MonoBehaviour
    {
        [SerializeField] private TriggerDetector front;
        
        public List<Views.UnitView> GetUnitViewsFromBasicAttack()
        {
            return front.Views;
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
