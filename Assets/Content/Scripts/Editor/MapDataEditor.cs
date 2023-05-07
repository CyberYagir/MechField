using Content.Scripts.Game;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.Editor
{
    [CustomEditor(typeof(MapData))]
    public class MapDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Set Lightning"))
            {
                (target as MapData).SetLightning();
            }
        }
    }
}
