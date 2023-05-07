using Content.Scripts.Menu.UIService;
using UnityEngine;

namespace Content.Scripts.Game.Mechs.UI
{
    public class UIMechWeapons : UIController<MechUIManager>
    {
        [SerializeField] private UIMechWeaponItem item;

        public override void Init(MechUIManager controller)
        {
            base.Init(controller);

            int id = 0;
            foreach (var w in controller.Player.Weapons)
            {
                var newItem = Instantiate(item, item.transform.parent);
                newItem.Init(w, id + 1, controller.GameData);
                id++;
            }
            item.gameObject.SetActive(false);
        }
    }
}
