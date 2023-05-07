using System.Collections;
using Content.Scripts.Menu.UIService;
using TMPro;
using UnityEngine;

namespace Content.Scripts.Game.Mechs.UI
{
    public class UIMechTargetDistance : UIController<MechUIManager>
    {
        private TMP_Text text;
        private LayerMask mask;
        public override void Init(MechUIManager controller)
        {
            base.Init(controller);
            mask = LayerMask.GetMask("Default", "Ground", "Enemy", "EnemyLegs", "DefaultGroundOnly");
            text = GetComponent<TMP_Text>();
            StartCoroutine(Loop());
        }

        IEnumerator Loop()
        {
            while (true)
            {
                yield return null;
                yield return null;
                Physics.Raycast(Controller.Player.MainCamera.transform.position, Controller.Player.MainCamera.transform.forward, out RaycastHit hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore);
                text.text = hit.collider == null ? "-" : hit.distance.ToString("F1");
            }
        }
    }
}
