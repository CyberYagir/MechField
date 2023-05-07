using TMPro;
using UnityEngine;

namespace Content.Scripts.Menu.UIService.OptionsDrawer
{
    public class OptionsItemQuality : UIController<MenuUIService>
    {
        public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            var drop = GetComponentInChildren<TMP_Dropdown>();
            drop.value = controller.SettingsService.SettingsObject.QualityLevel;
            drop.onValueChanged.AddListener(delegate(int value)
            {
                controller.SettingsService.SettingsObject.SetQuality(value);
            });
        }
    }
}
