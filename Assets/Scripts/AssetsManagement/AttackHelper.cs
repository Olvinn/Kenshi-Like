using Units.Structures;
using UnityEngine;

namespace AssetsManagement
{
    public class AttackHelper : MonoBehaviour
    {
        public static AttackHelper singleton;

        [Header("Note that indexes of array must be the same as in animator controller")]
        [SerializeField] private AttackPoint[] _attackPoints;

        private void Awake()
        {
            if (singleton)
                Destroy(gameObject);
            else
                singleton = this;
        }

        public AttackPoint GetAttack1Data(int layer)
        {
            if (layer < 0 || layer > _attackPoints.Length - 1)
                return _attackPoints[0];
            
            return _attackPoints[layer];
        }
    }
}
