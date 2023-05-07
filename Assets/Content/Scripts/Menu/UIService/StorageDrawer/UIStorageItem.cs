using System;
using TMPro;
using UnityEngine;
using static Content.Scripts.Global.PlayerDataObject.PlayerStorage;

namespace Content.Scripts.Menu.UIService
{
    public class UIStorageItem : UIListItem<StorageModule>
    {
        [SerializeField] private TMP_Text text;

        
        public override void Init(StorageModule data, MenuUIService menuUIService, Action<StorageModule, UIListItem<StorageModule>> OnClick)
        {
            base.Init(data, menuUIService, OnClick);
            text.text = menuUIService.GameData.GetModuleByID(data.ModuleId).ModuleName;
        }
    }
}
