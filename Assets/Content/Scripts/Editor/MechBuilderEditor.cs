using System.Linq;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Weapons;
using UnityEditor;
using UnityEngine;

namespace Content.Scripts.Editor
{
    [CustomEditor(typeof(MechBuilder))]
    public class MechBuilderEditor : UnityEditor.Editor
    {
        private MechBuilder mechBuilder;
        private void OnEnable()
        {
            mechBuilder = target as MechBuilder;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            if (GUILayout.Button("Configure Slots"))
            {
                mechBuilder.ModuleSlots.Clear();

                var list = mechBuilder.GetComponentsInChildren<Slot>().ToList().OrderBy(x => x is WeaponSlot).ToList();
                list.Reverse();

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].SetID(10000 + i + 1);
                    mechBuilder.ModuleSlots.Add(list[i]);
                    EditorUtility.SetDirty(list[i]);
                }
                EditorUtility.SetDirty(mechBuilder);
            }
        }
    }
}
