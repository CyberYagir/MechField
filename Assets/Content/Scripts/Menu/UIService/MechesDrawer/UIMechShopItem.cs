using System;
using Content.Scripts.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Menu.UIService
{
    public class UIMechShopItem : UIListItem<GameDataObject.MechHolder>
    {
        [SerializeField] private RawImage image;
        [SerializeField] private TMP_Text nameText;
        
        private bool waitForImage;



        public override void Init(GameDataObject.MechHolder data, MenuUIService menuUIService, Action<GameDataObject.MechHolder, UIListItem<GameDataObject.MechHolder>> OnClick)
        {
            base.Init(data, menuUIService, OnClick);
            
            if (data.MechPreview == null)
            {
                data.OnSetPreview += OnPreviewSetted;
                waitForImage = true;
            }
            else
            {
                image.texture = data.MechPreview;
            }

            nameText.text = this.Data.MechObject.MechName;

            
            
            var haveMech = this.menuUIService.LoaderService.PlayerData.AngarHaveMech(data.MechObject.ID);
            image.color = haveMech ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);

            UpdateSelection();
        }


        public override void UpdateSelection()
        {
           selection.gameObject.SetActive(this.Data.MechObject.ID == menuUIService.LoaderService.PlayerData.GetMechID());
        }

        private void OnPreviewSetted(RenderTexture obj)
        {
            this.Data.OnSetPreview -= OnPreviewSetted;
            image.texture = obj;
            waitForImage = false;
        }

        private void OnDisable()
        {
            if (waitForImage)
                this.Data.OnSetPreview -= OnPreviewSetted;
        }
    }
}
