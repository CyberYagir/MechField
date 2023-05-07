using System.Collections.Generic;
using Content.Scripts.Boot;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Content.Scripts.Game.UI
{
    public class GlobalUIEscapeWindow : GlobalUIWindow
    {
        [SerializeField] private Loader loader;
        public override void SetState(bool state)
        {
            base.SetState(state);
            if (state)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }


        public void ToAngar()
        {
            loader.SetLoadData(new LoaderData(loader.Data.Scene, loader.Data.Assets, new List<AssetReference>() { Manager.CreateService.CurrentMap.MapPrefab}, loader.Data.Type));
            loader.Load(Manager.GameData);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}