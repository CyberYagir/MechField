using System;
using Content.Scripts.Global;
using Content.Scripts.Weapons;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.Editor
{
    [CustomEditor(typeof(Slot), true)]
    public class SlotEditor : UnityEditor.Editor
    {
        private static GameDataObject gamedata;
        private Slot slot;
        private void OnEnable()
        {
            if (gamedata == null)
            {
                var guid = AssetDatabase.FindAssets("t:GameDataObject")[0];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                gamedata = AssetDatabase.LoadAssetAtPath<GameDataObject>(path);
            }

            slot = target as Slot;
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            slot.SetID(EditorGUILayout.IntField("SlotID: ", slot.SlotID));
            slot.SetType((ModuleObject.ModuleType)EditorGUILayout.EnumPopup("Type: ", slot.Type));
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(slot);
            }
            
            if (gamedata)
            {
                GUI.enabled = false;
                EditorGUILayout.IntField("Module ID: ", slot.ModuleID);
                EditorGUILayout.ObjectField("Item: ", gamedata.GetModuleByID(slot.ModuleID), typeof(ModuleObject));
                GUI.enabled = true;
            }
            else
            {
                GUILayout.Label("Game data not found");
            }
        }
    }
}
