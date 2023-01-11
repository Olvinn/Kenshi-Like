using UnityEditor;
using UnityEngine;

namespace Data.Editor
{
    [CustomEditor(typeof(Character))] 
    public class CharacterEditor : UnityEditor.Editor
    {
        private Character inst;
        private void OnEnable()
        {
            inst = serializedObject.targetObject as Character;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create random appearance"))
            {
                inst.appearance = Appearance.GetRandomAppearance();
            }
            EditorUtility.SetDirty(inst);
        }
    }
}
