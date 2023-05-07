using Content.Scripts.Game.Player;
using UnityEngine;

namespace Content.Scripts.Game.Mechs
{
    public class ThrusterActivator : MonoBehaviour
    {
        private ParticleSystem particle;
        public void Init(PlayerController playerController)
        {
            particle = GetComponent<ParticleSystem>();
            if (particle != null)
            {
                playerController.OnFlyStart += () => particle.Play(true);
                playerController.OnFlyEnd += () => particle.Stop(true);
            }
        }
    }
}
