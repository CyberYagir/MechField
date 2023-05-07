using Content.Scripts.Game.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.Game.Bot
{
    public enum BotStates
    {
        CheckSector, MovingToSector, AttackPlayer, RunAway
    }

    public class BotStateController : StateMachine<BotStates>
    {

        private BotEyes botEyes;
        
        private NavMeshAgent navMeshAgent;
        private BotBuilder botBuilder;
        private MapData mapData;
        private PlayerBuilder player;

        private float bravery;
        private float missTime = 0;
        private float runAwayTime = 0;
        
        public Transform Transform => botBuilder.transform;

        public BotEyes Eyes => botEyes;
        public float Brave => bravery;


        public BotStateController(BotBuilder botBuilder, NavMeshAgent navMeshAgent, MapData mapData, PlayerBuilder player, BotWeapons botWeapons)
        {
            this.player = player;
            this.mapData = mapData;
            this.botBuilder = botBuilder;
            this.navMeshAgent = navMeshAgent;

            botBuilder.OnAttacking += delegate
            {
                StateSet(BotStates.AttackPlayer);
            };
            
            bravery = Random.value * 100;
            
            botEyes = new BotEyes(botBuilder.BotOptions.SeeDistance, botBuilder.BotOptions.SeeAngle, botBuilder.MechBuilder.PartsData.YRotator.transform);
            
            StatesInit(BotStates.CheckSector, 
                new BotStateCheckSector(BotStates.CheckSector, this, mapData),
                new BotStateMoveToSector(BotStates.MovingToSector, this, mapData),
                new BotStateRunAway(BotStates.RunAway,this, mapData),
                new BotStateAttackPlayer(BotStates.AttackPlayer, botBuilder, botWeapons, player, this.navMeshAgent, this, mapData)
                );
            OnStateFinished += StatesController;
        }

        private void StatesController(BotStates lastState)
        {
            switch (lastState)
            {
                case BotStates.CheckSector:
                    StateSet(BotStates.MovingToSector);
                    break;
                case BotStates.MovingToSector:
                    StateSet(Random.value < 0.5f ? BotStates.MovingToSector : BotStates.CheckSector);
                    break;
                case BotStates.AttackPlayer:
                    StateSet(BotStates.MovingToSector);
                    runAwayTime = 0;
                    break;
                case BotStates.RunAway:
                    runAwayTime = 0;
                    StateSet(Random.value < 0.5f ? BotStates.MovingToSector : BotStates.CheckSector);
                    break;
            }
        }

        public void Update()
        {
            if (botBuilder.Data.IsOverheated) return;
            StateRun();
            
            var playerVisible = botEyes.IsVisible(player.MechBuilder.Torso) || player.MechBuilder.Torso.position.ToDistance(botBuilder.transform.position) <= botBuilder.BotOptions.MinAgrDistance;
            if (!player.IsDead)
            {
                if (playerVisible && ActiveState != BotStates.AttackPlayer && ActiveState != BotStates.RunAway)
                {
                    if (botEyes.IsPhysicsVisible(player.MechBuilder.Torso))
                    {
                        if (botBuilder.IsCriticalDamage())
                        {
                            StateSet(HaveBrave() ? BotStates.AttackPlayer : BotStates.RunAway);
                        }
                        else
                        {
                            StateSet(BotStates.AttackPlayer);
                        }
                    }
                }
                else if (ActiveState == BotStates.AttackPlayer)
                {
                    if (botEyes.IsPhysicsVisible(player.MechBuilder.Torso) && playerVisible)
                    {
                        missTime = 0;
                    }

                    missTime += Time.deltaTime;

                    if (botBuilder.IsCriticalDamage())
                    {
                        runAwayTime += Time.deltaTime;
                        if (runAwayTime >= 5)
                        {
                            if (!HaveBrave())
                            {
                                StateSet(BotStates.RunAway);
                                return;
                            }

                            runAwayTime = 0;
                        }
                    }

                    if (missTime >= botBuilder.BotOptions.MissTargetTime)
                    {
                        StateSet(Random.value < 0.5f ? BotStates.MovingToSector : BotStates.CheckSector);
                    }
                }
            }
        }

        public bool HaveBrave()
        {
            return bravery > Random.value * 100f;
        }
        

        public void MoveTo(Vector3 pointPos)
        {
            navMeshAgent.SetDestination(pointPos);
        }

        public bool IsArrive() => navMeshAgent.remainingDistance < 3f;

        public bool CanBuildPath(Vector3 point)
        {
            var path = new NavMeshPath();
            navMeshAgent.CalculatePath(point, path);
            return path.status == NavMeshPathStatus.PathComplete;
        }

        public override void Gizmo()
        {
            base.Gizmo();
            for (int i = 0; i < navMeshAgent.path.corners.Length; i++)
            {
                if (i == 0)
                {
                    Gizmos.DrawLine(navMeshAgent.path.corners[i], botBuilder.transform.position);
                }
                else
                {
                    Gizmos.DrawLine(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i - 1]);
                }
            }
            Gizmos.DrawSphere(navMeshAgent.destination, 0.5f);
            
        }
    }
}