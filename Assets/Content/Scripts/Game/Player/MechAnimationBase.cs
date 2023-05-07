using System;
using UnityEngine;

namespace Content.Scripts.Game.Player
{
    public class MechAnimationBase : IUpdateData
    {
        private const float MAX_ANIMATION_SPEED = 20;
        private const float FLY_DIST = 2.5f;
        
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int IsMove = Animator.StringToHash("IsMove");
        private static readonly int Fly = Animator.StringToHash("Fly");
        
        protected MechBuilderBase builder;
        protected Animator animator;
        private Rigidbody rb;
        private LayerMask mask;
        
        private bool enabled = true;
        
        protected float animationSpeed;
        private float rotateLayer;
        private bool isFly;

        public event Action OnLand;
        
        public bool IsFly => isFly;
        public bool Enabled => enabled;

        protected MechAnimationBase(Animator animator, Rigidbody rb, MechBuilderBase builder)
        {
            this.animator = animator;
            this.rb = rb;
            this.builder = builder;
            this.animationSpeed = builder.MechObject.MechValues.LegsParameters.AnimationMaxSpeed;

            mask = LayerMask.GetMask("Default", "Ground");

        }

        
        public void SetEnabled(bool state) => enabled = state;
        
        public virtual void Update()
        {
            var localDir = GetLocalDir();
            
            animator.SetBool(IsMove, localDir.magnitude > 0.1f);
            animator.SetFloat(Vertical, Mathf.Clamp(localDir.z / animationSpeed, -1f, 1f));
            animator.SetFloat(Horizontal, Mathf.Clamp(localDir.x / animationSpeed, -1f, 1f));
            animator.speed = Mathf.Lerp(0.5f, 1f, Mathf.Abs(Mathf.Clamp(localDir.z / MAX_ANIMATION_SPEED, -1f, 1f)));
            
            builder.MechBuilder.Torso.transform.position = builder.MechBuilder.PartsData.PlevisPoint.position;

            isFly = true;
            foreach (var foot in builder.MechBuilder.PartsData.Foots)
            {
                if (Physics.Raycast(foot.position, Vector3.down, out RaycastHit hit, FLY_DIST, mask))
                {
                    isFly = false;
                    break;
                }
            }

            var oldFly = animator.GetBool(Fly);
            animator.SetBool(Fly, IsFly);

            if (oldFly && !isFly)
            {
                OnLand?.Invoke();
            }
        }

        protected virtual Vector3 GetLocalDir()
        {
            return animator.transform.InverseTransformDirection(new Vector3(rb.velocity.x, 0, rb.velocity.z));
        }

        public void Gizmo()
        {
            foreach (var foot in builder.MechBuilder.PartsData.Foots)
            {
                Gizmos.DrawLine(foot.position, foot.position + Vector3.down * FLY_DIST);
            }
        }

        protected virtual void MovingSides(Vector2 moveDir)
        {
            rotateLayer = Mathf.Lerp(rotateLayer, (moveDir.x > 0.1f || moveDir.x < -0.1f) && rb.velocity.magnitude <= 0.01f ? 1 : 0, Time.deltaTime * 10);
            animator.SetLayerWeight(1, rotateLayer);
        }
    }
}