using System.Collections.Generic;
using System.Linq;
using Content.Scripts.Global;
using Content.Scripts.Weapons;
using UnityEngine;
using static Content.Scripts.Global.ModuleObject;
using static Content.Scripts.Global.PlayerDataObject.PlayerMech;

namespace Content.Scripts.Game.Mechs
{
    public class MechBuilder : MonoBehaviour
    {
        [System.Serializable]
        public class MechPartsData
        {
            [SerializeField] private Transform torso;
            [SerializeField] private Transform plevis;
            [SerializeField] private Transform plevisPoint;
            [SerializeField] private Transform yRotator;
            [Space] [SerializeField] private List<Transform> foots;
            [SerializeField] private List<ThrusterActivator> thrusters;

            public Transform Plevis => plevis;
            public Transform Torso => torso;
            public Transform YRotator => yRotator;
            public Transform PlevisPoint => plevisPoint;
            public List<Transform> Foots => foots;
            public List<ThrusterActivator> Thrusters => thrusters;
        }

        [SerializeField] private Camera cabineCamera;
        [SerializeField] private Camera handsCamera;
        [SerializeField] private MechUIManager mechUI;
        [SerializeField] private Animator legsAnimator;
        [SerializeField] private Transform previewPoint;
        [Space] 
        [SerializeField] private MechPartsData parts;
        [SerializeField] private List<Camera> camerasStack;
        [SerializeField] private List<Slot> moduleSlots;

        private List<MechPart> mechParts;

        public MechPartsData PartsData => parts;
        public Animator LegsAnimator => legsAnimator;
        public Camera CabineCamera => cabineCamera;
        public List<Camera> Cameras => camerasStack;
        public Transform PreviewPoint => previewPoint;
        public List<Slot> ModuleSlots => moduleSlots;
        public MechUIManager MechUI => mechUI;
        public List<MechPart> PartsList => mechParts;
        public Transform Torso => PartsData.Torso;

        public Camera HandsCamera => handsCamera;


        public void LoadModulesAndOverrides(List<ModuleData> overrideModules, MechObject mechObject, GameDataObject gameData)
        {
            mechParts = GetComponentsInChildren<MechPart>().ToList();
            mechParts.ForEach(x=>x.Init());
            for (int i = 0; i < ModuleSlots.Count; i++)
            {
                var slotOverride = overrideModules.Find(x => x.SlotID == ModuleSlots[i].SlotID);
                if (slotOverride != null)
                {
                    ModuleSlots[i].SetModuleID(slotOverride.ModuleID, ModuleType.Any);
                }
                else
                {
                    var slot = mechObject.ModuleHolder.Find(x => x.ID == ModuleSlots[i].SlotID);
                    if (slot != null)
                    {
                        ModuleSlots[i].SetModuleID(gameData.GetModuleID(slot.Module), ModuleType.Any);
                    }
                    else
                    {
                        ModuleSlots[i].SetModuleID(-1, ModuleType.Any);
                    }
                }
            }
        }
    }
}
