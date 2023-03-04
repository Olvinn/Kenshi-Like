using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorUtils
{
    #if UNITY_EDITOR
    public class MeshCombiner : EditorWindow
    {
        public SkinnedMeshRenderer source;
        public SkinnedMeshRenderer target;
        
        [MenuItem ("Utils/Mesh helper")]

        public static void  ShowWindow () {
            EditorWindow.GetWindow(typeof(MeshCombiner));
        }
    
        void OnGUI ()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Source: ");
            GUILayout.Label("Target: ");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            source = EditorGUILayout.ObjectField(source, typeof(SkinnedMeshRenderer), true, null) as SkinnedMeshRenderer;
            target = EditorGUILayout.ObjectField(target, typeof(SkinnedMeshRenderer), true, null) as SkinnedMeshRenderer;
            GUILayout.EndHorizontal();
            if (source != null && target != null)
            {
                if (GUILayout.Button("Attach target bones to as in source")) 
                    CopyBonesFrom();
            }
        }

        private void CopyBonesFrom () {

            target.bones = source.bones;
            
//create a dictionary referencing source bones by name
            Dictionary<string, Transform> boneMap = new Dictionary<string, Transform> ();
            foreach (Transform bone in source.bones) 
            {
                boneMap [bone.name] = bone;
            }

//match each bone name of the target to a bone from the source
            for (int i = 0; i < target.bones.Length; ++i) 
            {
                string boneName = target.bones[i].name;
                if (!boneMap.TryGetValue (boneName, out target.bones [i])) {
                    Debug.LogError (target.name + " failed to get the bone, " + boneName + ", from " + source.name);
                    Debug.Break ();
                }
            }
        }
    }
    #endif
}
