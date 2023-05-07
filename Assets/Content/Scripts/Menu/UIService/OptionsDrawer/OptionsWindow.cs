using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Menu.UIService.OptionsDrawer
{
    public class OptionsWindow : UIListDrawer<Transform>
    {
        public override void ResetPreview()
        {
            base.ResetPreview();
            Controller.SettingsService.SaveFile();
        }
    }
}
