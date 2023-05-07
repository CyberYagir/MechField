using System.Collections;
using Content.Scripts.Menu.UIService;
using UnityEngine;

namespace Content.Scripts.Game.Mechs.UI
{
    public class UIMechHeat : UIController<MechUIManager>
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
                value.SetYLocalScale(Controller.Player.Data.Heat / Controller.Player.Data.MaxHeat);
            }
        }
    }
}
