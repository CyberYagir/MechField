using Content.Scripts.Global;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.Editor
{
    [CustomEditor(typeof(GameDataObject))]
    public class GameDataObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                GUILayout.Space(20);
                GUILayout.HorizontalSlider(-1, 0, 0);
                GUILayout.Space(20);
                var list = (target as GameDataObject).TempData.GetLoadedList();
                GUILayout.Label($"Loaded objects: {list.Count}");
                foreach (var obj in list)
                {
                    GUILayout.Label(obj.Key + " | " + obj.Value.name);
                }
            }
        }
    }
}
