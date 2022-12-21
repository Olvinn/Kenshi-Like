using System;
using System.Collections.Generic;
using Units.Weapons;
using UnityEngine;

namespace Units
{
    public class UnitAttack : MonoBehaviour
    {
        [SerializeField] private WeaponTriggerDetector front;
        
        public void BroadcastDamageInFront(Action<List<UnitView>> callback)
        {
            callback?.Invoke(front.views);
        }
    }
}
