using Content.Scripts.Game.InputService;
using UnityEngine;

namespace Content.Scripts.Game.Player
{
    public class PlayerAnimation : MechAnimationBase
    {
        public PlayerAnimation(Animator animator, Rigidbody rb, MechBuilderBase builder, IInputService inputService) : base(animator, rb, builder)
        {
            SetInput(inputService);
        }

        private void SetInput(IInputService inputService)
        {
            if (inputService != null)
            {
                inputService.OnMove += MovingSides;
            }
        }

        public void Disable()
        {
            animator.enabled = false;
        }
    }
}