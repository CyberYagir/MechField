using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Menu.UIService.OptionsDrawer
{
    public class OptionsItemMouseX : UIController<MenuUIService>
    {
        public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            var slider = GetComponentInChildren<Slider>();
            slider.value = controller.SettingsService.SettingsObject.MouseSensitivity.x;
            slider.onValueChanged.AddListener(delegate(float value)
            {
                controller.SettingsService.SettingsObject.SetMouseX(value);
            });
        }
    }
}
