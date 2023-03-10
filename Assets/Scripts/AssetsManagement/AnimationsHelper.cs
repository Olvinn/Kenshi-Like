using AssetsManagement.Structures;
using UnityEngine;

namespace AssetsManagement
{
    public class AnimationsHelper : MonoBehaviour
    {
        public static AnimationsHelper singleton;
        
        [Header("Note that indexes of array must be the same as in animator controller")]
        [SerializeField] private AnimationsSet[] sets;

        private void Awake()
        {
            if (singleton)
                Destroy(gameObject);
            else
                singleton = this;
        }

        public float GetAttack1HitOffset(int layer)
        {
            if (layer < 0 || layer > sets.Length - 1)
                return 0;
            
            return sets[layer].hitOffset;
        }

        public float GetAttack1TimeDuration(int layer)
        {
            if (layer < 0 || layer > sets.Length - 1)
                return 0;
            
            return sets[layer].attack1.length;
        }
    }
}
