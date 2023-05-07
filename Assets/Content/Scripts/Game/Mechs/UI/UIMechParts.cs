using Content.Scripts.Menu.UIService;
using UnityEngine;

namespace Content.Scripts.Game.Mechs.UI
{
    public class UIMechParts : UIController<MechUIManager>
    {
        [SerializeField] private UIMechPartsItem item;

        public override void Init(MechUIManager controller)
        {
            base.Init(controller);

            foreach (var part in controller.MechBuilder.GetComponentsInChildren<MechPart>())
            {
                var it = Instantiate(item, item.transform.parent);
                it.Init(part, controller.GameData);
            }
            item.gameObject.SetActive(false);
            
        }
    }
}
    
