using System;
using System.Collections.Generic;
using Content.Scripts.Game.Mechs;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Content.Scripts.Menu.UIService
{
    public class UIMechPartItem : UIListItem<MechPart>
    {
        [System.Serializable]
        public class NameToLocalizedText
        {
            [SerializeField] private MechPart.PartType partType;
            [SerializeField] private LocalizedString localizedString;

            public LocalizedString LocalizedString => localizedString;

            public MechPart.PartType PartType => partType;
        }

        [SerializeField] private TMP_Text text;

        public override void Init(MechPart data, MenuUIService menuUIService, Action<MechPart, UIListItem<MechPart>> OnClick)
        {
            base.Init(data, menuUIService, OnClick);

            selection.gameObject.SetActive(false);

            var find = menuUIService.GameData.MechPartsNames.Find(x => x.PartType == data.Type);
            if (find != null)
            {
                text.text = find.LocalizedString.GetLocalizedString();
            }
            else
            {
                text.text = data.Type.ToString().ToSpaceCapLetters();
            }

        }

        public override void UpdateSelection()
        {
            base.UpdateSelection();
            selection.gameObject.SetActive(true);
        }
    }
}
