using System.Collections.Generic;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using static Content.Scripts.Global.PlayerDataObject.PlayerMissions;

namespace Content.Scripts.Menu.UIService.MissionsDrawer
{
    public class UIMissionsWindow : UIListDrawer<Mission>
    {
        [System.Serializable]
        public class DataDrawer
        {
            [SerializeField] private GameObject holder;
            [SerializeField] private TMP_Text headerText, bodyText;
            [SerializeField] private Image previewImage;

            private void ChangeState(bool state) => holder.gameObject.SetActive(state);

            public void Hide() => ChangeState(false);

            public void Draw(MapObject map)
            {

                
                headerText.text = map.MapName;
                bodyText.text = map.MapDescription;
                previewImage.sprite = map.MapImage;

                ChangeState(true);
                LayoutRebuilder.MarkLayoutForRebuild(holder.GetComponent<RectTransform>());
            }
        }

        
        [System.Serializable]
        public class DataButton
        {
            [SerializeField] private Button button;

            private MapObject map;
            private MenuUIService service;
            private UIMissionsWindow window;
            public void Init(MenuUIService service, UIMissionsWindow window)
            {
                this.service = service;
                this.window = window;
                
                button.onClick.AddListener(LoadMission);
            }

            private void LoadMission()
            {
                if (map != null && window.selectedItem != null)
                {
                    window.LoadMission();
                }
            }


            public void UpdateSelect(MapObject map)
            {
                this.map = map;
            }
        }

        private void LoadMission()
        {
            var map = Controller.GameData.GetMapData(selectedItem.MapID);
            Controller.LoaderService.PlayerData.SetActiveMission(selectedItem.GUID);
            Controller.GlobalLoader.LoadWithData(new LoaderData("Game", new List<AssetReference>() {map.MapPrefab}, new List<AssetReference>(), LoaderData.LoadType.AllLoad), Controller.GameData);
        }


        [SerializeField] private DataDrawer description;
        [SerializeField] private DataButton button;

        public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            button.Init(controller, this);
            description.Hide();
        }


        public void ReInit()
        {
            Redraw();
        }

        public override void ResetPreview()
        {
            selectedItem = null;
            description.Hide();
        }
        
        
        private void Redraw()
        {
            ClearItems();
            
            item.gameObject.SetActive(true);
            
            foreach (var mission in Controller.LoaderService.PlayerData.GetMissions())
            {
                var it = Instantiate(item, holder);
                it.Init(mission, Controller, OnClick);
                if (mission == selectedItem)
                {
                    it.UpdateSelection();
                    var map = Controller.GameData.GetMapData(mission.MapID);
                    button.UpdateSelect(map);
                    description.Draw(map);
                }
            
                AddItem(it);
            }
            
            item.gameObject.SetActive(false);
        }

        private void OnClick(Mission newMission, UIListItem<Mission> item)
        {
            if (selectedItem != newMission)
            {
                selectedItem = newMission;
                Redraw();
            }
        }
    }
}
