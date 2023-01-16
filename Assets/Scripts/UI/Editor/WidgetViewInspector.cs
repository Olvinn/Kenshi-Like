using Data;
using UnityEditor;
using UnityEngine;

namespace UI.Editor
{
    [CustomEditor(typeof(WidgetView))] 
    public class WidgetViewInspector : UnityEditor.Editor
    {
        private WidgetView inst;
        ColorsPreset _preset;
        private void OnEnable()
        {
            inst = serializedObject.targetObject as WidgetView;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            GUILayout.Label("Update via preset");
            _preset = EditorGUILayout.ObjectField(_preset, typeof(ColorsPreset)) as ColorsPreset;
            if (_preset != null && GUILayout.Button("Create random appearance"))
            {
                inst.UpdateColors(_preset);
            }
            EditorUtility.SetDirty(inst);
        }
    }
}
