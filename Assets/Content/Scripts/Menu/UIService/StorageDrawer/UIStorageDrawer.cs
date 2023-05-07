using UnityEngine;
using UnityEngine.EventSystems;
using static Content.Scripts.Global.PlayerDataObject.PlayerStorage;

namespace Content.Scripts.Menu.UIService
{

    public class UIStorageDrawer : UIListDrawer<StorageModule>, IPointerClickHandler
    {
        [SerializeField] private Canvas window;
        [SerializeField] private MovedItem movedItem;

        public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            controller.LoaderService.PlayerData.OnStorageUpdate += Redraw;
        }

        public override void ReInit()
        {
            window.enabled = true;
            Redraw();
            movedItem.InitMoving(Controller.InputService);
        }

        public override void ResetPreview()
        {
            window.enabled = false;
            movedItem.Disable();
        }

        private void Redraw()
        {
            ClearItems();

            item.gameObject.SetActive(true);

            foreach (var storageItem in Controller.LoaderService.PlayerData.GetStorageItems())
            {
                var it = Instantiate(item, holder);
                it.Init(storageItem, Controller, OnClick);
                if (movedItem.Data != null && storageItem.GUID == movedItem.Data.GUID)
                {
                    it.gameObject.SetActive(false);
                }

                AddItem(it);
            }

            item.gameObject.SetActive(false);
        }


        private void OnClick(StorageModule module, UIListItem<StorageModule> listItem)
        {
            if(RemoveItem()) return;
            movedItem.Init(module, Controller, null);
            
            if (module.ModuleId == movedItem.Data.ModuleId)
            {
                listItem.gameObject.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            RemoveItem();
        }


        public bool RemoveItem()
        {
            if (movedItem.Data != null)
            {
                foreach (var it in Items)
                {
                    if (it.Data.GUID == movedItem.Data.GUID)
                    {
                        it.gameObject.SetActive(true);   
                        movedItem.Init(new StorageModule(-1), Controller, null);
                        movedItem.Disable();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
