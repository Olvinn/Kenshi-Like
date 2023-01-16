using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Data
{ 
    [CreateAssetMenu(menuName = "Game/Colors preset")]
    public class ColorsPreset : ScriptableObject
    {
        [field: Header("Characters")]
        [field: SerializeField] public List<Color> SkinColors { get; private set; }
        [field: SerializeField] public List<Color> HairColors { get; private set; }
        [field: SerializeField] public List<Color> ClothesColors { get; private set; }
        [field: SerializeField] public List<Color> BootsColors { get; private set; }
        
        [field: Header("UI")]
        [field: SerializeField] public Color CommonColor { get; private set; }
        [field: SerializeField] public Color AccentColor { get; private set; }
        [field: SerializeField] public Color AcceptColor { get; private set; }
        [field: SerializeField] public Color DeclineColor { get; private set; }
        [field: SerializeField] public Color AmbientColor { get; private set; }
        [field: SerializeField] public Color BackgroundColor { get; private set; }
    }
}
