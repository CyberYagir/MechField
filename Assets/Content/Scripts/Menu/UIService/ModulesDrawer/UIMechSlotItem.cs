using System;
using Content.Scripts.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Content.Scripts.Menu.UIService
{
    
    public class UIMechSlotItem : UIListItem<Slot>
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private TMP_Text typeText;
        [SerializeField] private LocalizedString emptyLocalized;

        public override void Init(Slot data, MenuUIService menuUIService, Action<Slot, UIListItem<Slot>> OnClick)
        {
            base.Init(data, menuUIService, OnClick);

            typeText.text = Slot.TypeToString(data.Type);

            if (data.ModuleID == -1)
            {
                text.text = " " + emptyLocalized.GetLocalizedString();
                text.SetAlpha(0.3f);
                typeText.SetAlpha(0.3f);
            }
            else
            {
                text.text = $" {menuUIService.GameData.GetModuleByID(data.ModuleID).ModuleName}";
            }
            
            selection.gameObject.SetActive(false);
        }

        public override void UpdateSelection()
        {
            base.UpdateSelection();
            selection.gameObject.SetActive(true);
        }
    }
}
