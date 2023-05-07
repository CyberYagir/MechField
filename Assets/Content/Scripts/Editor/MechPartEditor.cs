using System.Collections.Generic;
using Content.Scripts.Game.Mechs;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.Editor
{
    [CustomEditor(typeof(MechPart))]
    public class MechPartEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            
            GUILayout.Label("Debug damage:");
            
            GUILayout.BeginHorizontal();
            {
                List<int> list = new List<int> {15, 50, 100};

                foreach (var damage in list)
                {
                    if (GUILayout.Button(damage.ToString()))
                    {
                        (target as MechPart).TakeDamage(damage);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
