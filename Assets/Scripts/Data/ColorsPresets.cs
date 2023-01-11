using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Data
{ 
    [CreateAssetMenu(menuName = "Game/Colors preset")]
    public class ColorsPresets : ScriptableObject
    {
        [field: Header("Characters")]
        [field: SerializeField] public List<Color> SkinColors { get; private set; }
        [field: SerializeField] public List<Color> HairColors { get; private set; }
        [field: SerializeField] public List<Color> ClothesColors { get; private set; }
        [field: SerializeField] public List<Color> BootsColors { get; private set; }
    }
}
