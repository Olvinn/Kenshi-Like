using System;
using UnityEngine;

namespace Units.Structures
{
    [Serializable]
    public struct UnitAppearance
    {
        public Color skinColor;
        public Color baseColor;
        public Color secondaryColor;
        public Color accentColor;
        public string prefab;
        public AnimationSetType animationSet;
    }
}
