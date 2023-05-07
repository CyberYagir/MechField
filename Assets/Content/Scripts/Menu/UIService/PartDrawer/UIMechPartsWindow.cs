using Content.Scripts.Game.Mechs;
using EPOOutline;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Menu.UIService
{
    public class UIMechPartsWindow : UIListDrawer<MechPart>
    {
        [System.Serializable]
        public class OutlineOptions
        {
            [SerializeField] private Outlinable.OutlineProperties front;
            public void Set(Outlinable outlinable)
            {
                SetParameters(outlinable.OutlineParameters, front);
                outlinable.RenderStyle = RenderStyle.Single;
            }

            public void SetParameters(Outlinable.OutlineProperties outlineParameters, Outlinable.OutlineProperties change)
            {
                outlineParameters.Color = change.Color;
                outlineParameters.Enabled = change.Enabled;
                outlineParameters.BlurShift = change.BlurShift;
                outlineParameters.DilateShift = change.DilateShift;
                outlineParameters.FillPass.Shader = change.FillPass.Shader;
            }
        }
        
        [SerializeField] private UIMechSlotsWindow slotsDrawer;
        [SerializeField] private UIStorageDrawer storageDrawer;
        [SerializeField] private OutlineOptions outlineParameters;
        private float itemSizeY;
        private Outlinable currentOutline;
        

        public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            itemSizeY = item.GetComponent<RectTransform>().sizeDelta.y;
            slotsDrawer.ResetPreview();
            storageDrawer.ResetPreview();
        }


        public override void ReInit()
        {
            Redraw();
        }

        public override void ResetPreview()
        {
            slotsDrawer.ResetPreview();
            storageDrawer.ResetPreview();
            selectedItem = null;
            Destroy(currentOutline);
        }

        private void Redraw()
        {
            ClearItems();

            item.gameObject.SetActive(true);

            int id = 0;
            var parts = Controller.MechHolder.GetComponentsInChildren<MechPart>();
            foreach (var part in parts)
            {
                var it = Instantiate(item, holder);
                it.Init(part, Controller, OnClick);
                if (part == selectedItem)
                {
                    it.UpdateSelection();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(holder.GetComponent<RectTransform>());
                    slotsDrawer.MoveToY(-id * itemSizeY, slotsDrawer.IsVisible);
                }

                AddItem(it);
                id++;
            }

            item.gameObject.SetActive(false);
            if (selectedItem)
            {
                slotsDrawer.ReInit(selectedItem);
                storageDrawer.ReInit();
            }
        }



        private void OnClick(MechPart obj, UIListItem<MechPart> listItem)
        {
            if (selectedItem != obj)
            {
                
                selectedItem = obj;
                
                if (currentOutline != null)
                {
                    Destroy(currentOutline);
                }
                currentOutline = selectedItem.gameObject.AddComponent<Outlinable>();
                outlineParameters.Set(currentOutline);
                AddOutlineRenderersRecursive(selectedItem.transform);
                
                Redraw();
            }
        }

        private void AddOutlineRenderersRecursive(Transform item)
        {
            foreach (Transform child in item)
            {
                if (child.GetComponent<MechPart>()) continue;
                var mesh = child.GetComponent<Renderer>();
                if (mesh)
                {
                    currentOutline.TryAddTarget(new OutlineTarget(mesh));
                }

                AddOutlineRenderersRecursive(child);
            }
        }
    }
}
