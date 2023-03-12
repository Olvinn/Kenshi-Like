using AssetsManagement.Structures;
using Units.Structures;
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

        [SerializeField] private AnimationsSet zombie;

        private void Awake()
        {
            if (singleton)
                Destroy(gameObject);
            else
                singleton = this;
        }

        public float GetAttack1HitOffset(AnimationSetType set, int layer)
        {
            Debug.Log($"set: {set}, layer: {layer}");
            switch (set)
            {
                case AnimationSetType.CommonMen:
                    if (layer < 0 || layer > basicCharacterSets.Length - 1)
                        return 0;
            
                    return basicCharacterSets[layer].attack1HitOffset;
                case AnimationSetType.Zombie:
                    return zombie.attack1HitOffset;
                default:
                    return 0;
            }
        }

        public float GetAttack1TimeDuration(AnimationSetType set, int layer)
        {
            switch (set)
            {
                case AnimationSetType.CommonMen:
                    if (layer < 0 || layer > basicCharacterSets.Length - 1)
                        return 0;
            
                    return basicCharacterSets[layer].attack1.length;
                case AnimationSetType.Zombie:
                    return zombie.attack1.length;
                default:
                    return 0;
            }
        }

        public float GetReaction1Duration(AnimationSetType set, int layer)
        {
            switch (set)
            {
                case AnimationSetType.CommonMen:
                    if (layer < 0 || layer > basicCharacterSets.Length - 1)
                        return 0;
            
                    return basicCharacterSets[layer].reaction1.length;
                case AnimationSetType.Zombie:
                    return zombie.reaction1.length;
                default:
                    return 0;
            }
        }
    }
}
