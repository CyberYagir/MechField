using System.Collections.Generic;
using Content.Scripts.Game.Player;
using PathCreation;
using PathCreation.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Content.Scripts.Game.Bot
{
    public class BotStateAttackPlayer:TypeState<BotStates>
    {
        private BotBuilder botBuilder;
        private readonly PlayerBuilder player;
        private List<PathCreator> pathCreators;

        private float distanceOnPath;
        private float shootTime;
        private int currentPathPoint;
        private float moveDir;
        private bool coolWait;
        
        private int pathID;
        private LayerMask mask;
        private NavMeshAgent agent;
        private BotStateController botStateController;
        private MapData mapData;
        private BotBuilder.BotData data;
        private Transform target;
        private BotWeapons weapons;


        public BotStateAttackPlayer(
            BotStates type, 
            BotBuilder botBuilder, 
            BotWeapons weapons,
            PlayerBuilder player, 
            NavMeshAgent agent, 
            BotStateController botStateController, 
            MapData mapData) : base(type)
        {
            this.weapons = weapons;
            this.data = botBuilder.BotOptions;
            this.mapData = mapData;
            this.botStateController = botStateController;
            this.agent = agent;
            this.botBuilder = botBuilder;
            this.player = player;


            moveDir = Random.value >= 0.5f ? -1 : 1;
            mask = LayerMask.GetMask("Ground");
            pathCreators = botBuilder.BotOptions.SpawnPaths(new GameObject(this.botBuilder.name + " Paths").transform);
            
            
            botBuilder.OnChangeTarget += SetTarget;
            botBuilder.Data.OnOverheat += OnOnOverheat;
        }

        private void OnOnOverheat(bool state)
        {
            if (state) weapons.AllWeaponsStop();
        }

        private void SetTarget(Transform target)
        {
            this.target = target;
        }

        public override void Start()
        {
            base.Start();
            pathID = pathCreators.GetRandomIndex();
            pathCreators.ForEach(x => x.gameObject.SetActive(x == pathCreators[pathID]));
            pathCreators[pathID].transform.position = player.transform.position;

            shootTime = 0;
        }      
        

        public override void Run()
        {
            base.Run();

            
            PathUpdate();
            MoveOnPath();
            Attack();
            
            
            if (player.IsDead)
            {
                FinishState(); return;
            }
        }

        public override void End()
        {
            base.End();
            weapons.AllWeaponsStop();
            botBuilder.SetTarget(null);
        }

        private void Attack()
        {
            shootTime += Time.deltaTime;
            
            MainCameraRotation();

            if (shootTime >= data.ShootDuration)
            {
                var avgHealth = botBuilder.AvgHealth();
                if (!botStateController.HaveBrave() && avgHealth < data.FullBraveHealth)
                {
                    FinishState();
                }
                else
                {
                    if (Random.value >= 0.35f && avgHealth != 1f)
                    {
                        botBuilder.SetTarget(player.GetMostDamagedPart());
                    }
                    else
                    {
                        botBuilder.SetTarget(player.GetRandomPart());
                    }
                }

                shootTime = 0f;
                weapons.RandomWeaponsState();
                weapons.AllWeaponsStop();
            }
            else
            {
                Shoot();
            }
        }

        private void MainCameraRotation()
        {
            var camera = botBuilder.MechBuilder.CabineCamera.transform;
            camera.rotation =
                Quaternion.Lerp(
                    camera.rotation,
                    Quaternion.LookRotation(
                        target != null ? target.position - camera.position : botBuilder.MechBuilder.Torso.forward),
                    Time.deltaTime * botBuilder.BotOptions.MainCameraRotateSpeed);
        }

        public void Shoot()
        {
            ShootCooling();
            
            var torso = botBuilder.MechBuilder.Torso;
            float dot = 0;
            if (target)
            {
                dot = Vector3.Dot(torso.forward, Vector3.Normalize(target.position - torso.position));
            }
            if (target != null && dot >= 0.5f && !coolWait)
            {
                if (botStateController.Brave < 50)
                {
                    if (botBuilder.Data.Heat < botBuilder.Data.MaxHeat * data.MinCoolingToShoot)
                    {
                        weapons.AllWeaponsShoot();
                    }
                    else
                    {
                        coolWait = true;
                    }
                    return;
                }
                else
                {
                    weapons.AllWeaponsShoot();
                    return;
                }
            }
            
            weapons.AllWeaponsStop();
        }

        private void ShootCooling()
        {
            if (coolWait)
            {
                if (botBuilder.Data.Heat < botBuilder.Data.MaxHeat * (data.MinCoolingToShoot / 2f))
                {
                    coolWait = false;
                }
            }
        }

        private void MoveOnPath()
        {
            distanceOnPath += agent.speed * Time.deltaTime * moveDir;
            var point = pathCreators[pathID].path.GetPointAtDistance(distanceOnPath);
            if (mapData.PointIsOnNavmesh(point) && botStateController.CanBuildPath(point))
            {
                botStateController.MoveTo(point);
            }
        }

        private void PathUpdate()
        {
            var path = pathCreators[pathID];
            path.transform.position = Vector3.Lerp(path.transform.position, player.transform.position, Time.deltaTime * data.PathLerpSpeed);
            
            if (currentPathPoint < path.path.localPoints.Length)
            {
                if (Physics.Raycast(path.path.GetPoint(currentPathPoint) + Vector3.up * 200, Vector3.down, out RaycastHit hit, Mathf.Infinity, mask))
                {
                    path.path.localPoints[currentPathPoint] = MathUtility.InverseTransformPoint(hit.point, path.transform, path.path.space);
                }
            }

            currentPathPoint++;

            if (currentPathPoint >= path.path.localPoints.Length)
            {
                currentPathPoint = 0;
            }
        }

        public override void Gizmo()
        {
            base.Gizmo();

            Gizmos.DrawWireSphere(pathCreators[pathID].path.GetPointAtDistance(distanceOnPath), 0.5f);
        }
    }
}