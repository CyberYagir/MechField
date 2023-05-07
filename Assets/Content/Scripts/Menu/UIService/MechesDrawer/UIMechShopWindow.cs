using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using static Content.Scripts.Global.GameDataObject;

namespace Content.Scripts.Menu.UIService
{
    public class UIMechShopWindow : UIListDrawer<MechHolder>
    {
        [System.Serializable]
        public class Description
        {
            [SerializeField] private GameObject holder;
            [SerializeField] private TMP_Text text;
            public void SetText(string str, string mechName)
            {
                holder.gameObject.SetActive(true);
                text.text = $"<b><size={text.fontSize * 2}>{mechName}</size><b>\n{str}";
                LayoutRebuilder.ForceRebuildLayoutImmediate(text.rectTransform.parent.GetComponent<RectTransform>());
            }


            public void Disable()
            {
                holder.gameObject.SetActive(false);
            }
        }
        
        [System.Serializable]
        public class BuyButton
        {
            [SerializeField] private Button applyButton;
            [SerializeField] private TMP_Text buttonText;
            [SerializeField] private LocalizedString changeString;
            [SerializeField] private LocalizedString equippedString;
            public void AddListener(Action action)
            {
                applyButton.onClick.AddListener(delegate { action?.Invoke(); });
            }
            
            public void SetText(MechHolder mechData, bool isHave, bool isIn)
            {
                if (!isHave)
                {
                    buttonText.text = mechData.Cost.ToString() + "<sprite=0>";
                }
                else
                {
                    if (!isIn)
                    {
                        buttonText.text = changeString.GetLocalizedString();
                    }
                    else
                    {
                        buttonText.text = equippedString.GetLocalizedString();
                    }
                }
            }
            
            public bool ButtonDown(MechHolder selectedMech, MenuUIService uiService)
            {
                var playerData = uiService.LoaderService.PlayerData;
                
                if (playerData.GetMechID() == selectedMech.MechObject.ID) return false;

                if (playerData.GetMoney() >= selectedMech.Cost && !playerData.AngarHaveMech(selectedMech.MechObject.ID))
                {
                    return playerData.BuyMech(selectedMech);;
                }

                if (playerData.AngarHaveMech(selectedMech.MechObject.ID))
                {
                    playerData.SetMech(selectedMech.MechObject);
                    return true;
                }
                
                return false;
            }
        }
        [SerializeField] private Description description;
        [SerializeField] private BuyButton buyButton;

        

        public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            buyButton.AddListener(BuyButtonCalculation);
            description.Disable();
        }

        private void BuyButtonCalculation()
        {
            if (buyButton.ButtonDown(selectedItem, Controller))
            {
                Redraw();
            }
            UpdateButtonText();
        }

        public override void ReInit()
        {
            Redraw();
        }

        public override void ResetPreview()
        {
            var playerData = Controller.LoaderService.PlayerData;
            playerData.SetMech(Controller.GameData.GetMechByID(playerData.GetMechID()));
            description.Disable();
        }

        private void Redraw()
        {
            ClearItems();
         
            item.gameObject.SetActive(true);
            
            foreach (var mech in Controller.GameData.GetListMeches())
            {
                var it = Instantiate(item, holder);
                it.Init(mech, Controller, OnMechSelect);
                AddItem(it);
            }
            
            item.gameObject.SetActive(false);
        }

        private void OnMechSelect(MechHolder newSelectedMech, UIListItem<MechHolder> listItem)
        {
            description.SetText(newSelectedMech.MechDesc, newSelectedMech.MechObject.MechName);
            Controller.LoaderService.PlayerData.SetMech(newSelectedMech.MechObject, true);
            
            selectedItem = newSelectedMech;
            
            UpdateButtonText();
        }

        private void UpdateButtonText()
        {
            var playerData = Controller.LoaderService.PlayerData;
            buyButton.SetText(selectedItem, playerData.AngarHaveMech(selectedItem.MechObject.ID), selectedItem.MechObject.ID == playerData.GetMechID());
        }
    }
}
