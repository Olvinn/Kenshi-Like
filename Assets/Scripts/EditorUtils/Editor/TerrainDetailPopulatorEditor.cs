using Data;
using UnityEditor;
using UnityEngine;

namespace EditorUtils.Editor
{
    [CustomEditor(typeof(TerrainDetailPopulator))] 
    public class TerrainDetailPopulatorEditor : UnityEditor.Editor
    {
        private TerrainDetailPopulator inst;
        
        private void OnEnable()
        {
            inst = serializedObject.targetObject as TerrainDetailPopulator;
        }
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Populate grass"))
            {
                inst.PopulateGrass();
            }
            EditorUtility.SetDirty(inst);
        }
    }
}
