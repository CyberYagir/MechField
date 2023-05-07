using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Content.Scripts.Menu.UI
{
    public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private List<TMP_Text> text;
        [SerializeField] private List<Image> images;
        [SerializeField] private List<Image> background;
        [SerializeField] private float clickSize;
        [SerializeField] private Color baseColor = Color.white;
        [SerializeField] private Color secondColor = Color.gray;
        private Vector3 size;

        private void Awake()
        {
            size = transform.localScale;
        }

        private float duration = 0.15f;


        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach (var t in text)
            {
                var color = secondColor;
                color.a = t.alpha;
                t.DOColor(color, duration).SetLink(gameObject);
            }

            foreach (var i in images)
            {
                i.DOColor(secondColor, duration).SetLink(gameObject);
            }

            foreach (var b in background)
            {
                b.DOFade(1, duration).SetLink(gameObject);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ToNormal();
        }

        private void OnDisable()
        {
            ToNormal();
        }

        private void ToNormal()
        {
            foreach (var t in text)
            {
                var color = baseColor;
                color.a = t.alpha;
                t.DOColor(color, duration).SetLink(gameObject);
            }

            foreach (var i in images)
            {
                i.DOColor(baseColor, duration).SetLink(gameObject);
            }

            foreach (var b in background)
            {
                b.DOFade(0, duration).SetLink(gameObject);
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (clickSize == 1f) return;
            transform.DOScale(size * clickSize, duration).SetLink(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (clickSize == 1f) return;
            transform.DOScale(size, duration).SetLink(gameObject);
        }

        public void SetBaseColor(Color selectionColor)
        {
            baseColor = selectionColor;
            ToNormal();
        }
    }
}
