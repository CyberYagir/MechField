using System;
using System.Collections.Generic;
using Content.Scripts.Game.GameStateService;
using Content.Scripts.Game.Mechs;
using Content.Scripts.Game.Player;
using Content.Scripts.Game.PoolService;
using Content.Scripts.Global;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Content.Scripts.Game.Bot
{
    public partial class BotBuilder : MechBuilderBase
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private BotData botData;
        [SerializeField] private float heat;
        [SerializeField] private bool overheat;

        
        
        private LayerMask builderMask;
        private PlayerBuilder playerBuilder;

        private BotAnimation animation;
        private BotStateController controller;
        private MechModules modules;
        private BotWeapons weapons;

        private Transform target;
        private MechData mechData;
        private IStateService stateService;

        public BotData BotOptions => botData;
        public BotEyes Sensors => controller.Eyes;
        public PlayerBuilder PlayerTarget => playerBuilder;
        public MechData Data => mechData;
        
        public event Action OnAttacking;
        

        public event Action<Transform> OnChangeTarget;

        public void Init(
            MechObject mechObject,
            MapData mapData,
            PlayerBuilder playerBuilder,
            GameDataObject gameData,
            IPoolService poolService,
            IStateService stateService,
            MapObject.WorldParameters parameters)
        {
            this.stateService = stateService;
            this.gameData = gameData;
            this.playerBuilder = playerBuilder;
            this.mechObject = mechObject;
            mechInstance = Instantiate(mechObject.Prefab, transform);
            builderMask = LayerMask.GetMask("Default", "Ground");

            GenerateName(gameData);

            mechInstance.LoadModulesAndOverrides(new List<PlayerDataObject.PlayerMech.ModuleData>(), base.mechObject, gameData);

            InitDeath();
            
            mechInstance.PartsList.ForEach(x=>x.OnTakeDamage += delegate(float f)
            {
                OnAttacking?.Invoke();
            });

            mechData = new MechData(base.mechObject, parameters, BotOptions.DamageModify, stateService);
            mechData.OnOverheat += OnOverheat;

            modules = new MechModules(gameData, MechBuilder);
            weapons = new BotWeapons(gameData, this, poolService, stateService);
            animation = new BotAnimation(MechBuilder.LegsAnimator, rb, this, navMeshAgent, botData);
            controller = new BotStateController(this, navMeshAgent, mapData, playerBuilder, weapons);

            stateService.OnChangeState += OnChangeGameState;
        }

        private void OnChangeGameState(GameState state)
        {
            navMeshAgent
                .With(x => x.isStopped = state == GameState.Pause)
                .With(x => x.velocity = Vector3.zero);
                
            animation.SetAnimationSpeed(state == GameState.Pause ? 0 : 1);
        }

        public override void Death()
        {
            base.Death();

            this.
                With(x=>x.animation.SetEnabled(false)).
                With(x=>x.weapons.AllWeaponsStop()).
                With(x=>x.MechBuilder.LegsAnimator.enabled = false).
                With(x=>navMeshAgent.isStopped = true).
                With(x=>x.isDead = true);
            
            ExplodeMech();
        }


        public void SetTarget(Transform target)
        {
            this.target = target;
            OnChangeTarget?.Invoke(target);
        }
        private void GenerateName(GameDataObject gameData)
        {
            var targetName = botData.GetName(gameData);
            if (targetName != null)
            {
                transform.name = targetName + $" {Random.Range(10000, 99999)}";
            }
        }

        public override void OnOverheat(bool state)
        {

            if (state)
            {
                animation.SetRotateToTarget(false);
                mechInstance.PartsData.YRotator.DOLocalRotate(new Vector3(50, 0, 0), 0.5f);
                navMeshAgent.isStopped = true;
            }
            else
            {
                mechInstance.PartsData.YRotator.DORotate(animation.GetLookerRotation, 0.5f).onComplete += () =>
                {
                    navMeshAgent.isStopped = false;
                    animation.SetRotateToTarget(true);
                };
            }
        }

        private void Update()
        {
            if (!IsDead && stateService.IsCanPlay)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, builderMask))
                {
                    MechBuilder.transform.SetYPosition(hit.point.y);
                }

                heat = Data.Heat / Data.MaxHeat;
                overheat = Data.IsOverheated;
                animation.Update();
                controller.Update();
                mechData.Update();
                weapons.Update();
            }
        }

        public void CreateBot()
        {
            MechBuilder.CabineCamera.transform.parent = MechBuilder.Torso;
            MechBuilder.Cameras.ForEach(x => x.enabled = false);
            var cabineLayer = LayerMask.NameToLayer("PlayerCabine");
            foreach (Transform it in MechBuilder.Torso.transform)
            {
                if (it.gameObject.layer == cabineLayer)
                {
                    Destroy(it.gameObject);
                    break;
                }
            }

            MechBuilder.MechUI.gameObject.Destroy();


            rb.isKinematic = true;
            MechBuilder.transform.ChangeLayerWithChilds(LayerMask.NameToLayer("Enemy"));
            MechBuilder.LegsAnimator.gameObject.ChangeLayerWithChilds(LayerMask.NameToLayer("EnemyLegs"));
            MechBuilder.gameObject.layer = LayerMask.NameToLayer("Colliders");
        }

        private void OnDrawGizmos()
        {
            controller?.Gizmo();
        }

    }
}
