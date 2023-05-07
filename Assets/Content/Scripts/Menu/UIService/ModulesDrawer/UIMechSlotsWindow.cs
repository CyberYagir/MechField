using System.Collections.Generic;
using System.Linq;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Weapons;
using DG.Tweening;
using UnityEngine;
using static Content.Scripts.Global.ModuleObject;

namespace Content.Scripts.Menu.UIService
{

    public class UIMechSlotsWindow : UIListDrawer<Slot>
    {
        [SerializeField] private Canvas window;
        [SerializeField] private MovedItem moveItem;
        private MechPart selectedPart;
        public bool IsVisible => window.enabled;
        
        public void ReInit(MechPart selectedPart)
        {
            window.enabled = true;
            this.selectedPart = selectedPart;
            Redraw();
        }

        public override void ResetPreview()
        {
            window.enabled = false;
            selectedItem = null;
        }

        private List<Slot> slotsOnPart = new List<Slot>(10);

        private void Redraw()
        {
            ClearItems();

            if (selectedPart == null) return;

            item.gameObject.SetActive(true);
            slotsOnPart.Clear();
            GetAllSlotsInPartRecursive(selectedPart.transform);

            foreach (var slot in slotsOnPart.OrderBy(x => x is WeaponSlot).Reverse())
            {
                var it = Instantiate(item, holder);
                it.Init(slot, Controller, OnClick);
                if (slot == selectedItem)
                {
                    it.UpdateSelection();
                }

                AddItem(it);
            }

            item.gameObject.SetActive(false);
        }


        private void GetAllSlotsInPartRecursive(Transform item)
        {
            var slots = item.GetComponents<Slot>();
            if (slots.Length != 0)
            {
                var parent = slots[0].GetComponentInParent<MechPart>();
                for (int i = 0; i < slots.Length; i++)
                {
                    if (parent == selectedPart)
                    {
                        slotsOnPart.Add(slots[i]);
                    }
                }
            }

            foreach (Transform child in item)
            {
                GetAllSlotsInPartRecursive(child);
            }
        }

        private void OnClick(Slot newSlot, UIListItem<Slot> listItem)
        {

            if(PlaceItem(newSlot)) return;

            GetItem(newSlot);
        }

        public void GetItem(Slot newSlot)
        {
            if (moveItem.Data == null || moveItem.Data.ModuleId == -1)
            {
                if (newSlot.ModuleID != -1)
                {
                    var id = newSlot.ModuleID;
                    newSlot.SetModuleID(-1, ModuleType.Any);
                    GetFromSlot(id);
                    Redraw();
                }
            }
            else if (moveItem.Data != null && moveItem.Data.ModuleId != -1)
            {
                var oldItem = newSlot.ModuleID;
                
                var itemObject = Controller.GameData.GetModuleByID(moveItem.Data.ModuleId);
                if (newSlot.SetModuleID(moveItem.Data.ModuleId, itemObject.Type))
                {
                    Controller.LoaderService.PlayerData.RemoveItem(moveItem.Data.GUID);
                    GetFromSlot(oldItem);
                    Redraw();
                }
            }


            void GetFromSlot(int moduleID)
            {
                var storageItem = Controller.LoaderService.PlayerData.AddItem(moduleID, false);
                moveItem.Init(storageItem, Controller, null);
                Controller.LoaderService.PlayerData.ChangeInventory();
                Controller.LoaderService.PlayerData.SetModule(newSlot);
            }
        }

        public bool PlaceItem(Slot newSlot)
        {
            if (moveItem.Data != null && moveItem.Data.ModuleId != -1)
            {
                if (newSlot.ModuleID == -1)
                {
                    var itemObject = Controller.GameData.GetModuleByID(moveItem.Data.ModuleId);
                    if (newSlot.SetModuleID(moveItem.Data.ModuleId, itemObject.Type))
                    {
                        Controller.LoaderService.PlayerData.RemoveItem(moveItem.Data.GUID);
                        moveItem.Disable();
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    GetItem(newSlot);
                    return true;
                }

                Controller.LoaderService.PlayerData.SetModule(newSlot);
                Redraw();
                return true;
            }

            return false;
        }
        

        public void MoveToY(float positionY, bool animation)
        {
           var rect = window.GetComponent<RectTransform>();
           rect.DOAnchorPosY(positionY, animation ? 0.1f : 0);
        }
    }
}
