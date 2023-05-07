using TMPro;
using UnityEngine;

namespace Content.Scripts.Menu.UIService.OptionsDrawer
{
    public class OptionsItemLocalization : UIController<MenuUIService>
    {
        public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            var drop = GetComponentInChildren<TMP_Dropdown>();
            drop.value = controller.SettingsService.SettingsObject.Localization;
            drop.onValueChanged.AddListener(delegate(int value)
            {
                controller.SettingsService.SettingsObject.SetLocalization(value);
            });
        }
    }
}
