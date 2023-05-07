using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;

namespace Content.Scripts.Menu.UI
{
    public class InformationPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        [SerializeField] private Canvas popup;

        public void OnPointerEnter(PointerEventData eventData)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(popup.GetComponent<RectTransform>());
            LayoutRebuilder.MarkLayoutForRebuild(popup.GetComponent<RectTransform>());
            UpdatePos(eventData);
            popup.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UpdatePos(eventData);
            popup.enabled = false;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            UpdatePos(eventData);
        }

        public void UpdatePos(PointerEventData eventData)
        {
            popup.transform.position = eventData.position;
        }
    }
}
