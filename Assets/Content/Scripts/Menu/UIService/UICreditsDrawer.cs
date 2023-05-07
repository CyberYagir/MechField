using TMPro;
using UnityEngine;

namespace Content.Scripts.Menu.UIService
{
    public class UICreditsDrawer : UIController<MenuUIService>
    {
        [SerializeField] private TMP_Text text;
        public override void Init(MenuUIService controller)
        {
            base.Init(controller);
            
            text.text = controller.LoaderService.PlayerData.GetMoney().ToString();
            
            controller.LoaderService.PlayerData.OnIncreaseMoney += UpdateMoney;
        }

        private void UpdateMoney(int credits)
        {
            text.text = credits.ToString();
        }
    }
}
