using System.Linq;
using Content.Scripts.Global;
using Content.Scripts.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Content.Scripts.Menu.UIService.SellDrawer
{
    public class UISellWindow : UIListDrawer<GameDataObject.ModuleHolder>
    {
        [System.Serializable]
        public class Description
        {
            [SerializeField] private GameObject holder;
            [SerializeField] private TMP_Text text;
            [SerializeField] private GameObject sellButton, buyButton;
            [SerializeField] private LocalizedString typeLocalized;
            [SerializeField] private LocalizedString countLocalized;
            [SerializeField] private LocalizedString descriptionLocalized;
            [SerializeField] private LocalizedString buyLocalized;
            [SerializeField] private LocalizedString sellLocalized;

            public void Show(ModuleObject module, int count, int sellCost)
            {
                holder.gameObject.SetActive(true);

                text.text = $"<size={text.fontSize * 1.2f}><b>{module.ModuleName}</b></size>";
                text.text += $"\n{typeLocalized.GetLocalizedString()}: <b>{Slot.TypeToString(module.Type)}</b>";
                text.text += $"\n{countLocalized.GetLocalizedString()}: <b>{count}</b>";
                text.text += $"\n{descriptionLocalized.GetLocalizedString()}: <b>{module.ModuleDesc}</b>";

                sellButton.SetActive(count > 0);

                sellButton.GetComponentInChildren<TMP_Text>().text = $"{sellLocalized.GetLocalizedString()} {sellCost} <sprite=0>";
                buyButton.GetComponentInChildren<TMP_Text>().text  = $"{buyLocalized.GetLocalizedString()} {module.ModuleCost} <sprite=0>";

                LayoutRebuilder.ForceRebuildLayoutImmediate(text.rectTransform);
                LayoutRebuilder.MarkLayoutForRebuild(text.rectTransform);
            }

            public void Hide()
            {
                holder.gameObject.SetActive(false);
            }
        }

        [SerializeField] private Description description;
        
        public override void ReInit()
        {
            description.Hide();
            Redraw();
        }

        public override void ResetPreview()
        {
            description.Hide();
        }

        private void Redraw()
        {
            ClearItems();
            
            item.gameObject.SetActive(true);
            
            foreach (var module in Controller.GameData.GetListModules().OrderBy(x=>x.Module.Type))
            {
                var it = Instantiate(item, holder);
                it.Init(module, Controller, OnClick);
                
                if (module == selectedItem)
                {
                    description.Show(
                        module.Module,
                        Controller.LoaderService.PlayerData.GetStorageItemsCount(module.ID),
                        (int) (module.Module.ModuleCost * Controller.GameData.SellCostMultiplier)
                    );
                    it.UpdateSelection();
                }
            
                AddItem(it);
            }
            
            item.gameObject.SetActive(false);
        }

        private void OnClick(GameDataObject.ModuleHolder item, UIListItem<GameDataObject.ModuleHolder> button)
        {
            if (selectedItem != item)
            {
                selectedItem = item;
                Redraw();
            }
        }

        public void BuyItem()
        {
            if (selectedItem != null)
            {
                if (Controller.LoaderService.PlayerData.BuyItem(selectedItem))
                {
                    Redraw();
                }
            }
        }

        public void SellItem()
        {
            if (selectedItem != null)
            {
                if (Controller.LoaderService.PlayerData.SellItem(selectedItem, Controller.GameData) != null)
                {
                    Redraw();
                }
            }
        }
    }
}
