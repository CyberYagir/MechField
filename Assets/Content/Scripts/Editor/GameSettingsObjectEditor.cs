using Content.Scripts.Global;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.Editor
{
    [CustomEditor(typeof(GameSettingsObject))]
    public class GameSettingsObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var so = target as GameSettingsObject;
            if (GUILayout.Button("Reset"))
            {
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(CreateInstance<GameSettingsObject>()), so);
                so.SaveFile();
            }
            if (GUILayout.Button("Save"))
            {
                so.SaveFile();
            }
            if (GUILayout.Button("Load"))
            {
                so.LoadFile();
            }
            EditorUtility.SetDirty(so);
        }
    }
}
