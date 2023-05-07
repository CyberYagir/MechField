using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.Game.UI
{
    public class GlobalUIOverheating : GlobalUIWindow
    {
        [SerializeField] private MaskableGraphic[] images;
        [SerializeField] private TMP_Text overheatText;
        [SerializeField] private float animationTime = 0.2f;

        private float time, blinkTime = 0.2f;

        public override void Init(GlobalUIManager manager)
        {
            base.Init(manager);
            manager.Player.Data.OnOverheat += OnOverheat;
        }

        public override void UpdateWindow()
        {
            base.UpdateWindow();
            time += Time.deltaTime;
            if (time >= blinkTime)
            {
                overheatText.enabled = !overheatText.enabled;
                time = 0;
            }
        }

        private void OnOverheat(bool state)
        {
            FadeImages(state ? 1 : 0);

            if (state)
            {
                SetState(true);
            }
            else
            {
                DOVirtual.DelayedCall(animationTime, () => SetState(false));
            }
        }

        private void FadeImages(float state)
        {
            foreach (var image in images)
            {
                image.DOFade(state, animationTime);
            }
        }
    }
}
