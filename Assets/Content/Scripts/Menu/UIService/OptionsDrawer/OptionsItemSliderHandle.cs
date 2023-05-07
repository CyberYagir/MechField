using System.Collections;
using Content.Scripts.Game.InputService;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Content.Scripts.Menu.UIService.OptionsDrawer
{
    public class OptionsItemSliderHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform popup;

        private TMP_Text text;
        private Slider slider;
        
        private bool down;

        [Inject]
        private void Constructor(IInputService inputService)
        {
            inputService.OnFireEnd += delegate(int i)
            {
                if (i == 0)
                {
                    down = false;
                }
            };
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            PopupUpdate();
            StopAllCoroutines();
            down = true;
            StartCoroutine(Loop());
        }

        private IEnumerator Loop()
        {
            while (down)
            {
                yield return null;
                PopupUpdate();
            }
            popup.gameObject.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }


        private void PopupUpdate()
        {
            if (text == null && slider == null)
            {
                text = popup.transform.GetComponentInChildren<TMP_Text>();
                slider = transform.GetComponentInParent<Slider>();
            }

            popup.transform.position = transform.position;
            text.text = slider.value.ToString("f1");
            popup.gameObject.SetActive(true);
        }
    }
}
