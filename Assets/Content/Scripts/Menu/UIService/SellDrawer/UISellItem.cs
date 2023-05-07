using System;
using Content.Scripts.Global;
using Content.Scripts.Weapons;
using TMPro;
using UnityEngine;

namespace Content.Scripts.Menu.UIService.SellDrawer
{
    public class UISellItem : UIListItem<GameDataObject.ModuleHolder>
    {
        [SerializeField] private TMP_Text typeText, nameText, valueText;
        public override void Init(GameDataObject.ModuleHolder data, MenuUIService menuUIService, Action<GameDataObject.ModuleHolder, UIListItem<GameDataObject.ModuleHolder>> OnClick)
        {
            base.Init(data, menuUIService, OnClick);

            typeText.text = Slot.TypeToString(data.Module.Type);
            nameText.text = data.Module.ModuleName;
            valueText.text = menuUIService.LoaderService.PlayerData.GetStorageItemsCount(data.ID).ToString();
        }

        public override void UpdateSelection()
        {
            base.UpdateSelection();
            selection.gameObject.SetActive(true);
        }
    }
}
