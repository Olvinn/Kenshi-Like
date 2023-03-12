using System;
using UnityEngine;

namespace AssetsManagement.Structures
{
    [Serializable]
    public class AnimationsSet
    {
        [SerializeField] public string name;
        
        [SerializeField] public AnimationClip attack1;
        [SerializeField] public float attack1HitOffset;

        [SerializeField] public AnimationClip reaction1;
    }
}
