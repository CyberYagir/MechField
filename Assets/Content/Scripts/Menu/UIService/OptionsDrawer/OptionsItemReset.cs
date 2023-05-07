using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Menu.UIService.OptionsDrawer
{
    public class OptionsItemReset : UIController<MenuUIService>
    {
        public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            
            GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
                Controller.SettingsService.SettingsObject.DeleteFile();
                Process.Start(Application.dataPath + $"/../{Application.productName}.exe"); 
                Application.Quit();
            });
        }
    }
}
