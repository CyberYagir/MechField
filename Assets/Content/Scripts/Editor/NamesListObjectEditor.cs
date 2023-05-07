using Content.Scripts.Global;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.Editor
{
    [CustomEditor(typeof(NamesListObject))]
    public class NamesListObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Read File"))
            {
                (target as NamesListObject).LoadFromFile();
                EditorUtility.SetDirty((target as NamesListObject));
                
            }
        }
    }
}
