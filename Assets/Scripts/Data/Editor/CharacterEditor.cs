using UnityEditor;
using UnityEngine;

namespace Data.Editor
{
    [CustomEditor(typeof(Character))] 
    public class CharacterEditor : UnityEditor.Editor
    {
        private Character inst;
        ColorsPreset _preset;
        private void OnEnable()
        {
            inst = serializedObject.targetObject as Character;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            GUILayout.Label("Update via preset");
            _preset = EditorGUILayout.ObjectField(_preset, typeof(ColorsPreset)) as ColorsPreset;
            if (_preset != null && GUILayout.Button("Create random appearance"))
            {
                inst.appearance = Appearance.GetRandomAppearance(_preset);
            }
            EditorUtility.SetDirty(inst);
        }
    }
}
