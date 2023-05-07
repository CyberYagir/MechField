using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Boot
{
    public class CrossSceneFader : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image image;
        [SerializeField] private float time = 0.2f;

        public event Action OnBlackScreen;
        
        public float FadeTime => time;


        public void Fade()
        {
            canvas.enabled = true;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            image.SetAlpha(0);
            StartCoroutine(FadeTo());
        }

        IEnumerator FadeTo()
        {
            while (image.GetAlpha() <= 1)
            {
                yield return null;
                image.SetAlpha(image.GetAlpha() + Time.deltaTime * 2f);
            }
            OnBlackScreen?.Invoke();
            StartCoroutine(FadeBack());
            
        }

        IEnumerator FadeBack()
        {
            while (image.GetAlpha() > 0)
            {
                yield return null;
                image.SetAlpha(image.GetAlpha() - Time.deltaTime * 2f);
            }
            Destroy(gameObject);
        }
    }
}
