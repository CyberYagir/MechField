using Content.Scripts.Menu.UIService;
using UnityEngine;

namespace Content.Scripts.Game.Mechs.UI
{
    public class UIMechOrientator : UIController<MechUIManager>
    {
        [SerializeField] private RectTransform legsCenter;
        public override void Init(MechUIManager controller)
        {
            base.Init(controller);
            controller.InputService.OnMouse += InputServiceOnOnMouse;
        }

        private void InputServiceOnOnMouse(Vector2 delta)
        {
            var parts = Controller.MechBuilder.PartsData;
            legsCenter.SetZLocalEulerAngles(-(parts.Plevis.localEulerAngles.y - parts.YRotator.localEulerAngles.y));
        }

        private void OnDisable()
        {
            if (Controller == null) return;
            Controller.InputService.OnMouse -= InputServiceOnOnMouse;
        }
    }
}
