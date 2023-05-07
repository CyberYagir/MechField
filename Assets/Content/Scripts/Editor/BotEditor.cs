using System;
using Content.Scripts.Game.Bot;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.Editor
{
    [CustomEditor(typeof(BotBuilder))]
    public class BotEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            if (Application.isPlaying)
            {

                BotBuilder builder = target as BotBuilder;
                Handles.DrawWireArc(builder.transform.position, Vector3.up, Vector3.forward, 360, builder.BotOptions.SeeDistance);

                Vector3 viewAngleA = builder.Sensors.DirFromAngle(-builder.BotOptions.SeeAngle / 2f, false);
                Vector3 viewAngleB = builder.Sensors.DirFromAngle(builder.BotOptions.SeeAngle / 2f, false);

                Handles.color = builder.Sensors.IsVisible(builder.PlayerTarget.transform) && builder.Sensors.IsPhysicsVisible(builder.PlayerTarget.transform) ? Color.yellow : Color.white;
                
                Handles.DrawLine(builder.transform.position, builder.transform.position + viewAngleA * builder.BotOptions.SeeDistance);
                Handles.DrawLine(builder.transform.position, builder.transform.position + viewAngleB * builder.BotOptions.SeeDistance);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add Heat"))
            {
                (target as BotBuilder).Data.AddHeat(50);
            }
        }
    }
}
