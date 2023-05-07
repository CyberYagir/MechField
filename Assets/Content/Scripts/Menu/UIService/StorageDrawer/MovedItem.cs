using System;
using Content.Scripts.Game.InputService;
using TMPro;
using UnityEngine;
using static Content.Scripts.Global.PlayerDataObject.PlayerStorage;

namespace Content.Scripts.Menu.UIService
{
    public class MovedItem : UIListItem<StorageModule>
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private TMP_Text text;
        [SerializeField] private RectTransform handle;
        
        public override void Init(StorageModule data, MenuUIService menuUIService, Action<StorageModule, UIListItem<StorageModule>> OnClick)
        {
            base.Init(data, menuUIService, OnClick);
            canvas.enabled = true;

            var item = menuUIService.GameData.GetModuleByID(data.ModuleId);
            if (item != null)
            {
                text.text = item.ModuleName;
            }
            else
            {
                Disable();
            }
        }

        public void InitMoving(IInputService inputService)
        {
            inputService.OnMousePosChange += MoveObject;
        }

        private void MoveObject(Vector2 pos)
        {
            handle.position = pos;
        }

        public void Disable()
        {
            data = null;
            canvas.enabled = false;
        }
    }
}
