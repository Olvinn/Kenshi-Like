using AssetsManagement.Structures;
using UnityEngine;
using UnityEngine.Serialization;

namespace AssetsManagement
{
    public class AnimationsHelper : MonoBehaviour
    {
        public static AnimationsHelper singleton;
        
        [FormerlySerializedAs("sets")]
        [Header("Note that indexes of array must be the same as in animator controller")]
        [SerializeField] private AnimationsSet[] basicCharacterSets;

        private void Awake()
        {
            if (singleton)
                Destroy(gameObject);
            else
                singleton = this;
        }

        public float GetAttack1HitOffset(int layer)
        {
            if (layer < 0 || layer > basicCharacterSets.Length - 1)
                return 0;
            return basicCharacterSets[layer].attack1HitOffset;
        }

        public float GetAttack1TimeDuration(int layer)
        {
            if (layer < 0 || layer > basicCharacterSets.Length - 1)
                return 0;
            
            return basicCharacterSets[layer].attack1.length;
        }

        public float GetReaction1Duration(int layer)
        {
            if (layer < 0 || layer > basicCharacterSets.Length - 1)
                return 0;
            
            return basicCharacterSets[layer].reaction1.length;
        }
    }
}
