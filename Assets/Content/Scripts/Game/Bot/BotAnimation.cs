using Content.Scripts.Game.Bot;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.Game.Player
{
    public class BotAnimation : MechAnimationBase
    {
        private BotBuilder.BotData botData;
        private Transform cabineLooker;
        private NavMeshAgent agent;

        private Transform target;

        private Vector3 destination;
        private BotBuilder builder;

        private bool rotateToTarget;
        
        public Vector3 GetLookerRotation => cabineLooker.eulerAngles;

        public BotAnimation(
            Animator animator, 
            Rigidbody rb, 
            BotBuilder builder, 
            NavMeshAgent agent, 
            BotBuilder.BotData botData) : base(animator, rb, builder)
        {
            this.builder = builder;
            this.botData = botData;
            this.agent = agent;
            
            cabineLooker = new GameObject("Looker")
                .With(x => x.transform.parent = builder.MechBuilder.transform)
                .With(x => x.transform.localPosition = builder.MechBuilder.PartsData.Plevis.localPosition)
                .transform;

            rotateToTarget = true;
            this.animationSpeed = agent.speed;
            builder.OnChangeTarget += SetTarget;
        }


        public override void Update()
        {
            base.Update();
            RotateToTarget();
        }

        public void SetRotateToTarget(bool state) => rotateToTarget = state;

        private void RotateToTarget()
        {
            if (target == null)
            {
                if (agent.path.corners.Length >= 2)
                {
                    destination = agent.path.corners[1];
                    destination.y += botData.HeightOffcet;

                    if (destination.ToDistance(cabineLooker.position) < 5)
                    {
                        destination.y = cabineLooker.position.y;
                    }
                }
            }
            else
            {
                destination = target.position;
            }


            var targetRotation = Quaternion.LookRotation(destination - cabineLooker.position);

            var parts = builder.MechBuilder.PartsData;
            var dot = Vector3.Dot(parts.Plevis.forward, (destination - cabineLooker.position).normalized);
            if (dot > -0.2f)
            {
                cabineLooker.rotation =
                    Quaternion.Lerp(
                        cabineLooker.rotation,
                        targetRotation,
                        botData.CabineRotateSpeed * Time.deltaTime);
            }

            if (rotateToTarget)
            {
                builder.MechBuilder.PartsData.YRotator.rotation = cabineLooker.rotation;
            }
        }


        public void SetTarget(Transform targ)
        {
            target = targ;
        }

        protected override Vector3 GetLocalDir()
        {
            return animator.transform.InverseTransformDirection(new Vector3(agent.velocity.x, 0, agent.velocity.z));
        }

        public void SetAnimationSpeed(float speed)
        {
            animator.speed = speed;
        }
    }
}