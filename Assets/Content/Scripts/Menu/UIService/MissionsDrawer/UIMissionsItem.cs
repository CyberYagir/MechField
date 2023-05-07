using System;
using Content.Scripts.Menu.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Content.Scripts.Global.PlayerDataObject.PlayerMissions;

namespace Content.Scripts.Menu.UIService.MissionsDrawer
{
    public class UIMissionsItem : UIListItem<Mission>
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;
        [SerializeField] private ButtonHover hover;
        public override void Init(Mission data, MenuUIService menuUIService, Action<Mission, UIListItem<Mission>> OnClick)
        {
            base.Init(data, menuUIService, OnClick);
            
            var (mapIcon, map) = menuUIService.GameData.GetMapData(data.IconID, data.MapID);

            text.text = map.MapName;
            icon.sprite = mapIcon;

            
            if (data.IsCompleted)
            {
                text.alpha = 0.5f;
                icon.SetAlpha(0.5f);
                icon.color = selection.color;
                hover.SetBaseColor(selection.color);
                icon.GetComponent<Outline>().enabled = false;
            }
        }

        public override void UpdateSelection()
        {
            base.UpdateSelection();
            selection.gameObject.SetActive(true);
        }
    }
}
