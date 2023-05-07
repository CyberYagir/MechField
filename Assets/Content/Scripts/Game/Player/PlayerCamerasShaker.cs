using Content.Scripts.Game.Mechs;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Game.Player
{
    public class PlayerCamerasShaker
    {
        [System.Serializable]
        public class ShakeOptions
        {
            [SerializeField] private float scale = 0.2f, duration = 0.3f, vibrato = 10f, randomness = 90f;

            public ShakeOptions(float scale, float duration, float vibrato, float randomness)
            {
                this.scale = scale;
                this.duration = duration;
                this.vibrato = vibrato;
                this.randomness = randomness;
            }

            public ShakeOptions()
            {
                
            }

            public float Randomness => randomness;

            public int Vibrato => (int)vibrato;

            public float Duration => duration;

            public float Scale => scale;
        }

        [SerializeField] private MechBuilder mechBuilder;
        [SerializeField] private Vector3 cabinePos, handsPos;
        public MechBuilder MechBuilder => mechBuilder;

        public PlayerCamerasShaker(MechBuilder mechBuilder)
        {
            this.mechBuilder = mechBuilder;
            cabinePos = MechBuilder.CabineCamera.transform.localPosition;
            handsPos = MechBuilder.HandsCamera.transform.localPosition;
        }


        public void ShakeCamera(ShakeOptions shake)
        {
            MechBuilder.CabineCamera.transform.localPosition = cabinePos;
            MechBuilder.CabineCamera.DOShakePosition(shake.Duration, shake.Scale, shake.Vibrato, shake.Randomness, true);
            
            MechBuilder.HandsCamera.transform.localPosition = handsPos;
            MechBuilder.HandsCamera.DOShakePosition(shake.Duration, shake.Scale, shake.Vibrato, shake.Randomness, true);
        }
    }
}