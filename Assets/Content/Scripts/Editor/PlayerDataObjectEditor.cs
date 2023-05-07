using System;
using System.IO;
using Content.Scripts.Global;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.Editor
{
    [CustomEditor(typeof(PlayerDataObject))]
    public class PlayerDataObjectEditor : UnityEditor.Editor
    {
        private PlayerDataObject so;
        
        private void OnEnable()
        {
            so = target as PlayerDataObject;
            so.LoadFile();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorUtility.IsDirty(so))
            {
                if (GUILayout.Button("Save"))
                {
                    so.SaveFile();
                    AssetDatabase.SaveAssetIfDirty(so);
                }
            }

            if (GUILayout.Button("Reset"))
            {
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(CreateInstance<PlayerDataObject>()), so);
                EditorUtility.SetDirty(so);
            }
            if (GUILayout.Button("Delete"))
            {
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(CreateInstance<PlayerDataObject>()), so);
                so.DeleteFile();
                EditorUtility.SetDirty(so);
            }
        }
    }
}
