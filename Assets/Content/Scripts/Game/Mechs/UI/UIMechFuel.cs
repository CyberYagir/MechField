using System.Collections;
using Content.Scripts.Menu.UIService;
using UnityEngine;

namespace Content.Scripts.Game.Mechs.UI
{
    public class UIMechFuel : UIController<MechUIManager>
    {
        [SerializeField] private Transform value;
        public override void Init(MechUIManager controller)
        {
            base.Init(controller);
            StartCoroutine(Loop());
        }


        IEnumerator Loop()
        {
            while (true)
            {
                yield return null;
                if (Controller.Player.Data.MaxFuel == 0)
                {
                    value.SetYLocalScale(0);
                    yield break;
                }
                value.SetYLocalScale(Controller.Player.Data.Fuel / Controller.Player.Data.MaxFuel);
            }
        }
    }
}
